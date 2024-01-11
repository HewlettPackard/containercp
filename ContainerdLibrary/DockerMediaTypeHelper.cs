/* Copyright 2024 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
namespace ContainerdLibrary
{
    public static class DockerMediaTypeHelper
    {
        public static bool IsManifestList(DockerMediaType mediaType)
        {
            return mediaType == DockerMediaType.ManifestListV2Json;
        }
    }
}
