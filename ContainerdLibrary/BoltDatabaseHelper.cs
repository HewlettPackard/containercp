/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using BoltDB;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace ContainerdLibrary
{
    internal class BoltDatabaseHelper
    {
        private static Dictionary<string, string> s_pathToCopy = new Dictionary<string, string>();

        public static BoltDatabase Open(string path)
        {
            try
            {
                return new BoltDatabase(path);
            }
            catch (IOException ex) when (ex.Message.EndsWith("is being used by another process."))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    string copyPath;
                    if (!s_pathToCopy.TryGetValue(path, out copyPath))
                    {
                        copyPath = Path.GetTempFileName();
                        LinuxShellHelper.Copy(path, copyPath);
                        s_pathToCopy.Add(path, copyPath);
                    }
                    
                    return new BoltDatabase(copyPath);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
