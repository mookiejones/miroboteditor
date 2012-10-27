using System;
using System.Runtime.InteropServices;

namespace miRobotEditor.Core
{
    
        /// <summary>
        /// Native Methods
        /// </summary>
        static class NativeMethods
        {
            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DestroyIcon(IntPtr handle);
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteObject(IntPtr hObject);
        }
    
}
