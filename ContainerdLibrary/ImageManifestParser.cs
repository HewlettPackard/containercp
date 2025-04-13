/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ContainerdLibrary
{
    public class ImageManifestParser
    {
        private const string ConfigPropertyName = "config";
        private const string LayersPropertyName = "layers";
        private const string MediaTypePropertyName = "mediaType";

        /// <returns>List of blob references of layers from lowest to highest</returns>
        public static List<BlobReference> GetImageLayers(string manifestJson)
        {
            List<BlobReference> result = new List<BlobReference>();
            JObject manifest = JObject.Parse(manifestJson);
            JArray layers = manifest.GetValue(LayersPropertyName) as JArray;
            foreach(JObject layer in layers)
            {
                JToken mediaTypeObject = layer.GetValue(MediaTypePropertyName);
                JToken digestObject = layer.GetValue("digest");
                JToken sizeObject = layer.GetValue("size");

                string mediaType = mediaTypeObject.Value<string>();
                DockerMediaType dockerMediaType = DockerMediaTypeParser.ParseImageLayerMediaType(mediaType);

                string digest = digestObject.Value<string>();
                int size = sizeObject.Value<int>();
                result.Add(new BlobReference(digest, size, dockerMediaType));
            }

            return result;
        }

        public static BlobReference GetImageConfigBlobReference(string manifestJson)
        {
            JObject manifest = JObject.Parse(manifestJson);
            JObject config = manifest.GetValue(ConfigPropertyName) as JObject;
            JToken mediaTypeObject = config.GetValue(MediaTypePropertyName);
            JToken digestObject = config.GetValue("digest");
            JToken sizeObject = config.GetValue("size");
            string mediaType = mediaTypeObject.Value<string>();
            DockerMediaType dockerMediaType = DockerMediaTypeParser.ParseImageConfigMediaType(mediaType);
            string digest = digestObject.Value<string>();
            int size = sizeObject.Value<int>();
            return new BlobReference(digest, size, dockerMediaType);
        }
    }
}
