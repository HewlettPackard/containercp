/* Copyright 2023-2024 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using Newtonsoft.Json.Linq;

namespace ContainerdLibrary
{
    public static class ImageManifestListParser
    {
        private const string MediaTypePropertyName = "mediaType";

        internal static bool IsManifestList(string manifestJson)
        {
            JObject manifest = JObject.Parse(manifestJson);
            JToken mediaTypeObject = manifest.GetValue(MediaTypePropertyName);
            string mediaType = mediaTypeObject.Value<string>();
            DockerMediaType dockerMediaType = DockerMediaTypeParser.ParseManifestMediaType(mediaType);
            return dockerMediaType == DockerMediaType.ManifestListV2Json;
        }
    }
}
