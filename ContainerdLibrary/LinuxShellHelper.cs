/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using System;
using System.Diagnostics;

namespace ContainerdLibrary
{
    internal class LinuxShellHelper
    {
        private const string CpPath = "/bin/cp";

        public static void Copy(string source, string destination)
        {
            string arguments = $"{source} {destination}";
            ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = CpPath, Arguments = arguments, };
            Process process = new Process() { StartInfo = startInfo };
            process.StartInfo.UseShellExecute = false;
            process.Start();

            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                throw new Exception($"Copy failed, exit code {process.ExitCode}");
            }
        }
    }
}
