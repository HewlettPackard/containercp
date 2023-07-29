/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using System;

namespace ContainerdLibrary
{
    internal class DockerMediaTypeParser
    {
        private const string ManifestV2MediaType = "application/vnd.docker.distribution.manifest.v2+json";

        private const string ManifestListV2MediaType = "application/vnd.docker.distribution.manifest.list.v2+json";

        private const string TarGzipLayerMediaType = "application/vnd.docker.image.rootfs.diff.tar.gzip";

        private const string TarLayerMediaType = "application/vnd.docker.image.rootfs.diff.tar";

        public static DockerMediaType ParseManifestMediaType(string mediaType)
        {
            switch (mediaType)
            {
                case ManifestV2MediaType:
                    return DockerMediaType.ManifestV2Json;
                case ManifestListV2MediaType:
                    return DockerMediaType.ManifestListV2Json;
                default:
                    throw new NotSupportedException($"{mediaType} manifest media type is not supported");
            }
        }

        public static DockerMediaType ParseImageLayerMediaType(string mediaType)
        {
            switch (mediaType)
            {
                case TarGzipLayerMediaType:
                    return DockerMediaType.ImageTarGzip;
                case TarLayerMediaType:
                    return DockerMediaType.ImageTar;
                default:
                    throw new NotSupportedException($"{mediaType} image media type is not supported");
            }
        }
    }
}
