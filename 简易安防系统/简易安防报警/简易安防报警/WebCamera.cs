using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace 简易安防报警
{
    public class ShowVideo
    {
        [DllImport("avicap32.dll")]
        public static extern IntPtr capCreateCaptureWindowA(byte[] lpszWindowName, int dwStyle, int x, int y, int nWidth, int nHeight, IntPtr hWndParent, int nID);
        [DllImport("avicap32.dll")]
        public static extern bool capGetDriverDescriptionA(short wDriver, byte[] lpszName, int cbName, byte[] lpszVer, int cbVer);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, short wParam, int lParam);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, short wParam, FrameEventHandler lParam);
        [DllImport("User32.dll")]
        public static extern bool SendMessage(IntPtr hWnd, int wMsg, int wParam, ref BITMAPINFO lParam);
        [DllImport("User32.dll")]
        public static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
        [DllImport("avicap32.dll")]
        public static extern int capGetVideoFormat(IntPtr hWnd, IntPtr psVideoFormat, int wSize);
        
        public const int WM_USER = 0x400;
        public const int WS_CHILD = 0x40000000;
        public const int WS_VISIBLE = 0x10000000;
        public const int SWP_NOMOVE = 0x2;
        public const int SWP_NOZORDER = 0x4;
        public const int WM_CAP_DRIVER_CONNECT = WM_USER + 10;
        public const int WM_CAP_DRIVER_DISCONNECT = WM_USER + 11;
        public const int WM_CAP_SET_CALLBACK_FRAME = WM_USER + 5;
        public const int WM_CAP_SET_PREVIEW = WM_USER + 50;
        public const int WM_CAP_SET_PREVIEWRATE = WM_USER + 52;
        public const int WM_CAP_SET_VIDEOFORMAT = WM_USER + 45;

        //结构体
        [StructLayout(LayoutKind.Sequential)]
        public struct VIDEOHDR
        {
            [MarshalAs(UnmanagedType.I4)]
            public int lpData;
            [MarshalAs(UnmanagedType.I4)]
            public int dwBufferLength;
            [MarshalAs(UnmanagedType.I4)]
            public int dwBytesUsed;
            [MarshalAs(UnmanagedType.I4)]
            public int dwTimeCaptured;
            [MarshalAs(UnmanagedType.I4)]
            public int dwUser;
            [MarshalAs(UnmanagedType.I4)]
            public int dwFlags;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public int[] dwReserved;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFOHEADER
        {
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biSize;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biWidth;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biHeight;
            [MarshalAs(UnmanagedType.I2)]
            public short biPlanes;
            [MarshalAs(UnmanagedType.I2)]
            public short biBitCount;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biCompression;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biSizeImage;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biXPelsPerMeter;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biYPelsPerMeter;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biClrUsed;
            [MarshalAs(UnmanagedType.I4)]
            public Int32 biClrImportant;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct BITMAPINFO
        {
            [MarshalAs(UnmanagedType.Struct, SizeConst = 40)]
             public BITMAPINFOHEADER bmiHeader;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
            public Int32[] bmiColors;
        }

        public delegate void FrameEventHandler(IntPtr lwnd, IntPtr lpVHdr);

        //  公用方法
        public static object GetStructure(IntPtr ptr, ValueType structure)
        {
            return Marshal.PtrToStructure(ptr, structure.GetType());
        }

        public static object GetStructure(int ptr, ValueType structure)
        {
            return GetStructure(new IntPtr(ptr), structure);
        }

        public static void Copy(IntPtr ptr, byte[] data)
        {
            Marshal.Copy(ptr, data, 0, data.Length);
        }

        public static void Copy(int ptr, byte[] data)
        {
            Copy(new IntPtr(ptr), data);
        }

        public static int SizeOf(object structure)
        {
            return Marshal.SizeOf(structure);
        }

    }
    public class WebCamera
    {
        /// <summary>
        /// 调用摄像头
        /// </summary>
        /// <param name="handle">显示图像的句柄</param>
        /// <param name="width">图像显示的宽度</param>
        /// <param name="height">图像显示的高度</param>
        public WebCamera(IntPtr handle, int width, int height)
        {
            mControlPtr = handle;
            mWidth = width;
            mHeight = height;
        }

        //  窗体委托回调方法
        public delegate void RecievedFrameEventHandler(byte[] data);
        public event RecievedFrameEventHandler RecievedFrame;

        private IntPtr lwndC;  //  Holds  the  unmanaged  handle  of  the  control
        private IntPtr mControlPtr;  //  Holds  the  managed  pointer  of  the  control
        private int mWidth;
        private int mHeight;

        private ShowVideo.FrameEventHandler mFrameEventHandler;  //  Delegate  instance  for  the  frame  callback  -  must  keep  alive!  gc  should  NOT  collect  it

        /// <summary>
        /// 关闭摄像头
        /// </summary>
        public void CloseWebcam()
        {
            this.capDriverDisconnect(this.lwndC);
        }

       /// <summary>
        ///   打开摄像头
       /// </summary>

        public void StartWebCam()
        {
            byte[] lpszName = new byte[100];
            byte[] lpszVer = new byte[100];

            ShowVideo.capGetDriverDescriptionA(0, lpszName, 100, lpszVer, 100);
            this.lwndC = ShowVideo.capCreateCaptureWindowA(lpszName, ShowVideo.WS_VISIBLE + ShowVideo.WS_CHILD, 0, 0, mWidth, mHeight, mControlPtr, 0);

            if (this.capDriverConnect(this.lwndC, 0))
            {
                this.capPreviewRate(this.lwndC, 66);
                this.capPreview(this.lwndC, true);
                ShowVideo.BITMAPINFO bitmapinfo = new ShowVideo.BITMAPINFO();
                bitmapinfo.bmiHeader.biSize = ShowVideo.SizeOf(bitmapinfo.bmiHeader);
                bitmapinfo.bmiHeader.biWidth = 352;
                bitmapinfo.bmiHeader.biHeight = 288;
                bitmapinfo.bmiHeader.biPlanes = 1;
                bitmapinfo.bmiHeader.biBitCount = 24;
                this.capSetVideoFormat(this.lwndC, ref bitmapinfo,ShowVideo.SizeOf(bitmapinfo));
                this.mFrameEventHandler = new ShowVideo.FrameEventHandler(FrameCallBack);
                this.capSetCallbackOnFrame(this.lwndC, this.mFrameEventHandler);
                ShowVideo.SetWindowPos(this.lwndC, 0, 0, 0, mWidth, mHeight, 6);
            }
        }
        
        private bool capDriverConnect(IntPtr lwnd, short i)
        {
            return ShowVideo.SendMessage(lwnd, ShowVideo.WM_CAP_DRIVER_CONNECT, i, 0);
        }
        private bool capDriverDisconnect(IntPtr lwnd)
        {
            return ShowVideo.SendMessage(lwnd, ShowVideo.WM_CAP_DRIVER_DISCONNECT, 0, 0);
        }

        private bool capPreview(IntPtr lwnd, bool f)
        {
            return ShowVideo.SendMessage(lwnd, ShowVideo.WM_CAP_SET_PREVIEW, f, 0);
        }
        private bool capPreviewRate(IntPtr lwnd, short wMS)
        {
            return ShowVideo.SendMessage(lwnd, ShowVideo.WM_CAP_SET_PREVIEWRATE, wMS, 0);
        }

        private bool capSetCallbackOnFrame(IntPtr lwnd,ShowVideo.FrameEventHandler lpProc)
        {
            return ShowVideo.SendMessage(lwnd,ShowVideo.WM_CAP_SET_CALLBACK_FRAME, 0, lpProc);
        }
        private bool capSetVideoFormat(IntPtr hCapWnd, ref  ShowVideo.BITMAPINFO BmpFormat, int CapFormatSize)
        {
            return ShowVideo.SendMessage(hCapWnd, ShowVideo.WM_CAP_SET_VIDEOFORMAT, CapFormatSize, ref  BmpFormat);
        }
        private void FrameCallBack(IntPtr lwnd, IntPtr lpVHdr)
        {
            ShowVideo.VIDEOHDR videoHeader = new ShowVideo.VIDEOHDR();
            byte[] VideoData;
            videoHeader = (ShowVideo.VIDEOHDR)ShowVideo.GetStructure(lpVHdr, videoHeader);
            VideoData = new byte[videoHeader.dwBytesUsed];
            ShowVideo.Copy(videoHeader.lpData, VideoData);
            if (this.RecievedFrame != null)
                this.RecievedFrame(VideoData);
        }
    }

}
