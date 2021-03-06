﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Streaming.Common.Helpers
{
    public static class PlatformHelper
    {
        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static string CommandlineToolname {
            get {
                if (IsWindows())
                    return "Powershell.exe";
                else
                    return "bash";
            }
        }
    }
}
