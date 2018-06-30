using System;
using System.Runtime.InteropServices;

namespace ToggleMonitorRefreshRate
{
    // https://www.codeproject.com/articles/36664/changing-display-settings-programmatically

    [StructLayout(LayoutKind.Sequential)]
    public struct POINTL
    {
        [MarshalAs(UnmanagedType.I4)]
        public int x;
        [MarshalAs(UnmanagedType.I4)]
        public int y;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DEVMODE
    {
        // You can define the following constant
        // but OUTSIDE the structure because you know
        // that size and layout of the structure
        // is very important
        // CCHDEVICENAME = 32 = 0x50
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmDeviceName;
        // In addition you can define the last character array
        // as following:
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        //public Char[] dmDeviceName;

        // After the 32-bytes array
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmSpecVersion;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmDriverVersion;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmSize;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmDriverExtra;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmFields;

        public POINTL dmPosition;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayOrientation;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFixedOutput;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmColor;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmDuplex;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmYResolution;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmTTOption;

        [MarshalAs(UnmanagedType.I2)]
        public Int16 dmCollate;

        // CCHDEVICENAME = 32 = 0x50
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string dmFormName;
        // Also can be defined as
        //[MarshalAs(UnmanagedType.ByValArray,
        //    SizeConst = 32, ArraySubType = UnmanagedType.U1)]
        //public Byte[] dmFormName;

        [MarshalAs(UnmanagedType.U2)]
        public UInt16 dmLogPixels;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmBitsPerPel;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPelsWidth;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPelsHeight;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFlags;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDisplayFrequency;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmICMMethod;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmICMIntent;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmMediaType;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmDitherType;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmReserved1;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmReserved2;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPanningWidth;

        [MarshalAs(UnmanagedType.U4)]
        public UInt32 dmPanningHeight;
    }

    class Program
    {
        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean EnumDisplaySettings(
            [param: MarshalAs(UnmanagedType.LPTStr)]
            string lpszDeviceName,
            [param: MarshalAs(UnmanagedType.U4)]
            int iModeNum,
            [In, Out]
            ref DEVMODE lpDevMode);

        [DllImport("User32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern int ChangeDisplaySettings(
            [In, Out]
            ref DEVMODE lpDevMode,
            [param: MarshalAs(UnmanagedType.U4)]
            uint dwflags);

        private static int ENUM_CURRENT_SETTINGS = -1;
        private static int DISP_CHANGE_SUCCESSFUL = 0;

        public static DEVMODE GetCurrentMode()
        {
            DEVMODE mode = new DEVMODE();
            mode.dmSize = (ushort)Marshal.SizeOf(mode);

            if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref mode) == false)
            {
                throw new ApplicationException($"EnumDisplaySettings failed! (Last Win32 error: {Marshal.GetLastWin32Error()})");
            }

            return mode;
        }

        public static void ToggleRefreshRate(uint maxFrequency)
        {
            DEVMODE currentMode = GetCurrentMode();

            DEVMODE newMode = currentMode;

            if (currentMode.dmDisplayFrequency != maxFrequency)
            {
                Console.WriteLine($"Switching to {maxFrequency}...");

                newMode.dmDisplayFrequency = maxFrequency;
            }
            else
            {
                Console.WriteLine($"Switching to 60...");

                newMode.dmDisplayFrequency = 60;
            }

            int result = ChangeDisplaySettings(ref newMode, 0);

            if (result != DISP_CHANGE_SUCCESSFUL)
            {
                throw new ApplicationException($"ChangeDisplaySettings failed! (Result: {result}, Last Win32 error: {Marshal.GetLastWin32Error()})");
            }
        }

        static void Main(string[] args)
        {
            uint maxRefreshRate = 100;

            void printArgumentsHelp()
            {
                Console.WriteLine(
                    $"You can optionally pass a single numeric argument for the maximum refresh rate to switch to. Defaults to {maxRefreshRate}. " +
                    $"Press any key to continue with the default, or exit the process and try again (close the command prompt window, or press Ctrl+C).");
                Console.ReadKey(intercept: true);
            }

            if (args.Length == 0)
            {
                Console.WriteLine($"No max maximum refresh rate passed. Defaulting to {maxRefreshRate}.");
            }
            else if (args.Length == 1)
            {
                uint requestedMaxRefreshRate = 0;

                if (uint.TryParse(args[0], out requestedMaxRefreshRate))
                {
                    maxRefreshRate = requestedMaxRefreshRate;
                }
                else
                {
                    printArgumentsHelp();
                }
            }
            else if (args.Length > 1)
            {
                printArgumentsHelp();
            }

            try
            {
                ToggleRefreshRate(maxRefreshRate);

                Console.WriteLine("Succeeded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error! Ensure that {maxRefreshRate} is a supported refresh rate. Info:\n");
                Console.WriteLine(ex);
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
            }
        }
    }
}
