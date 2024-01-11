/* Copyright 2023-2024 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using System;

namespace ContainerdLibrary
{
    internal class DockerMediaTypeParser
    {
        private const string DockerManifestV2MediaType = "application/vnd.docker.distribution.manifest.v2+json";

        private const string DockerManifestListV2MediaType = "application/vnd.docker.distribution.manifest.list.v2+json";

        private const string OciManifestV1MediaType = "application/vnd.oci.image.manifest.v1+json"; // Identical in format to ManifestV2MediaType

        private const string OciManifestListV1MediaType = "application/vnd.oci.image.index.v1+json"; // Identical in format to ManifestListV2MediaType

        private const string DockerTarGzipLayerMediaType = "application/vnd.docker.image.rootfs.diff.tar.gzip";

        private const string DockerTarLayerMediaType = "application/vnd.docker.image.rootfs.diff.tar";

        private const string OciTarGzipLayerMediaType = "application/vnd.oci.image.layer.v1.tar+gzip";

        private const string OciTarLayerMediaType = "application/vnd.oci.image.layer.v1.tar";

        public static DockerMediaType ParseManifestMediaType(string mediaType)
        {
            switch (mediaType)
            {
                case DockerManifestV2MediaType:
                case OciManifestV1MediaType:
                    return DockerMediaType.ManifestV2Json;
                case DockerManifestListV2MediaType:
                case OciManifestListV1MediaType:
                    return DockerMediaType.ManifestListV2Json;
                default:
                    throw new NotSupportedException($"{mediaType} manifest media type is not supported");
            }
        }

        public static DockerMediaType ParseImageLayerMediaType(string mediaType)
        {
            switch (mediaType)
            {
                case DockerTarGzipLayerMediaType:
                case OciTarGzipLayerMediaType:
                    return DockerMediaType.ImageTarGzip;
                case DockerTarLayerMediaType:
                case OciTarLayerMediaType:
                    return DockerMediaType.ImageTar;
                default:
                    throw new NotSupportedException($"{mediaType} image media type is not supported");
            }
        }
    }
}
