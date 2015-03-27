namespace Tekla.UI.HyperToolTips
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// The w m_ nchittest.
        /// </summary>
        internal const int WM_NCHITTEST = 0x0084;

        /// <summary>
        /// The w m_ ncactivate.
        /// </summary>
        internal const int WM_NCACTIVATE = 0x0086;

        /// <summary>
        /// The w s_ e x_ noactivate.
        /// </summary>
        internal const int WS_EX_NOACTIVATE = 0x08000000;

        /// <summary>
        /// The httransparent.
        /// </summary>
        internal const int HTTRANSPARENT = -1;

        /// <summary>
        /// The htleft.
        /// </summary>
        internal const int HTLEFT = 10;

        /// <summary>
        /// The htright.
        /// </summary>
        internal const int HTRIGHT = 11;

        /// <summary>
        /// The httop.
        /// </summary>
        internal const int HTTOP = 12;

        /// <summary>
        /// The httopleft.
        /// </summary>
        internal const int HTTOPLEFT = 13;

        /// <summary>
        /// The httopright.
        /// </summary>
        internal const int HTTOPRIGHT = 14;

        /// <summary>
        /// The htbottom.
        /// </summary>
        internal const int HTBOTTOM = 15;

        /// <summary>
        /// The htbottomleft.
        /// </summary>
        internal const int HTBOTTOMLEFT = 16;

        /// <summary>
        /// The htbottomright.
        /// </summary>
        internal const int HTBOTTOMRIGHT = 17;

        /// <summary>
        /// The w m_ user.
        /// </summary>
        internal const int WM_USER = 0x0400;

        /// <summary>
        /// The w m_ reflect.
        /// </summary>
        internal const int WM_REFLECT = WM_USER + 0x1C00;

        /// <summary>
        /// The w m_ command.
        /// </summary>
        internal const int WM_COMMAND = 0x0111;

        /// <summary>
        /// The cb n_ dropdown.
        /// </summary>
        internal const int CBN_DROPDOWN = 7;

        /// <summary>
        /// The w m_ getminmaxinfo.
        /// </summary>
        internal const int WM_GETMINMAXINFO = 0x0024;

        /// <summary>The hiword.</summary>
        /// <param name="n">The n.</param>
        /// <returns>The System.Int32.</returns>
        internal static int HIWORD(int n)
        {
            return (n >> 16) & 0xffff;
        }

        /// <summary>The hiword.</summary>
        /// <param name="n">The n.</param>
        /// <returns>The System.Int32.</returns>
        internal static int HIWORD(IntPtr n)
        {
            return HIWORD(unchecked((int)(long)n));
        }

        /// <summary>The loword.</summary>
        /// <param name="n">The n.</param>
        /// <returns>The System.Int32.</returns>
        internal static int LOWORD(int n)
        {
            return n & 0xffff;
        }

        /// <summary>The loword.</summary>
        /// <param name="n">The n.</param>
        /// <returns>The System.Int32.</returns>
        internal static int LOWORD(IntPtr n)
        {
            return LOWORD(unchecked((int)(long)n));
        }

        /// <summary>
        /// The minmaxinfo.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct MINMAXINFO
        {
            /// <summary>
            /// The reserved.
            /// </summary>
            public Point reserved;

            /// <summary>
            /// The max size.
            /// </summary>
            public Size maxSize;

            /// <summary>
            /// The max position.
            /// </summary>
            public Point maxPosition;

            /// <summary>
            /// The min track size.
            /// </summary>
            public Size minTrackSize;

            /// <summary>
            /// The max track size.
            /// </summary>
            public Size maxTrackSize;
        }
    }
}