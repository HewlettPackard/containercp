/* Copyright 2023-2024 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace ContainerdLibrary
{
    public static class ImageManifestListParser
    {
        private const string ManifestsPropertyName = "manifests";
        private const string MediaTypePropertyName = "mediaType";

        public static List<KeyValuePair<BlobReference, DockerPlatform>> GetImageManifests(string manifestListJson)
        {
            List<KeyValuePair<BlobReference, DockerPlatform>> result = new List<KeyValuePair<BlobReference, DockerPlatform>>();
            JObject manifestList = JObject.Parse(manifestListJson);
            JArray manifests = manifestList.GetValue(ManifestsPropertyName) as JArray;
            foreach (JObject manifest in manifests)
            {
                JToken mediaTypeObject = manifest.GetValue(MediaTypePropertyName);
                JToken digestObject = manifest.GetValue("digest");
                JToken sizeObject = manifest.GetValue("size");

                JObject platformObject = (JObject)manifest.GetValue("platform");
                JToken architectureObject = platformObject.GetValue("architecture");
                JToken osObject = platformObject.GetValue("os");

                string mediaType = mediaTypeObject.Value<string>();
                DockerMediaType dockerMediaType = DockerMediaTypeParser.ParseManifestMediaType(mediaType);

                string digest = digestObject.Value<string>();
                int size = sizeObject.Value<int>();
                BlobReference blobReference = new BlobReference(digest, size, dockerMediaType);

                string architecture = architectureObject.Value<string>();
                string os = osObject.Value<string>();
                DockerPlatform platform = new DockerPlatform(architecture, os);
                result.Add(new KeyValuePair<BlobReference, DockerPlatform>(blobReference, platform));
            }

            return result;
        }

        public static BlobReference GetImageManifestReference(string manifestListJson, DockerPlatform platform)
        {
            List<KeyValuePair<BlobReference, DockerPlatform>> manifests = GetImageManifests(manifestListJson);
            if (manifests.Count == 0)
            {
                throw new InvalidDataException("Manifest list is empty");
            }

            if (manifests.Count == 1)
            {
                return manifests[0].Key;
            }

            foreach (KeyValuePair<BlobReference, DockerPlatform> manifest in manifests)
            {
                if (manifest.Value.Architecture == platform.Architecture && manifest.Value.OS == platform.OS)
                {
                    return manifest.Key;
                }
            }

            throw new Exception($"Image does not support architecture {platform.Architecture} and os {platform.OS}");
        }

        internal static bool IsManifestList(string manifestJson)
        {
            JObject manifest = JObject.Parse(manifestJson);
            JToken mediaTypeObject = manifest.GetValue(MediaTypePropertyName);
            string mediaType = mediaTypeObject.Value<string>();
            DockerMediaType dockerMediaType = DockerMediaTypeParser.ParseManifestMediaType(mediaType);
            return DockerMediaTypeHelper.IsManifestList(dockerMediaType);
        }
    }
}
