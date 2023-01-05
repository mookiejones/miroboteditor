using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using miRobotEditor.Enums;

namespace miRobotEditor.Classes
{
    [Localizable(false), SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        public delegate IntPtr MessageHandler(WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled);

        [Localizable(false)]
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, EntryPoint = "CommandLineToArgvW")]
        private static extern IntPtr _CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string cmdLine,
            out int numArgs);

        [Localizable(false)]
        [DllImport("kernel32.dll", EntryPoint = "LocalFree", SetLastError = true)]
        private static extern IntPtr _LocalFree(IntPtr hMem);

        [DllImport("user32.dll")]
        internal static extern short GetKeyState(int keyCode);

        public static string[] CommandLineToArgvW(string cmdLine)
        {
            IntPtr intPtr = IntPtr.Zero;
            string[] result;
            try
            {
                intPtr = _CommandLineToArgvW(cmdLine, out int num);
                if (intPtr == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }
                string[] array = new string[num];
                for (int i = 0; i < num; i++)
                {
                    IntPtr ptr = Marshal.ReadIntPtr(intPtr, i * Marshal.SizeOf(typeof(IntPtr)));
                    array[i] = Marshal.PtrToStringUni(ptr);
                }
                result = array;
            }
            finally
            {
                IntPtr intPtr2 = _LocalFree(intPtr);
                Console.WriteLine(IntPtr.Zero.Equals(intPtr2));
            }
            return result;
        }
    }
}