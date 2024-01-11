/* Copyright 2024 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using ContainerdLibrary;

namespace containercp
{
    public static class PlatformHelper
    {
        public static DockerPlatform CurrentPlatform
        {
            get
            {
                return new DockerPlatform("amd64", "linux"); // This program is only built for Linux x64
            }
        }
    }
}
