/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace ContainerdLibrary
{
    public class ImageManifestParser
    {
        private const string LayersPropertyName = "layers";
        private const string MediaTypePropertyName = "mediaType";

        /// <returns>List of blob references of layers from lowest to highest</returns>
        public static List<BlobReference> GetImageLayers(string manifestJson)
        {
            if (ImageManifestListParser.IsManifestList(manifestJson))
            {
                throw new NotSupportedException("Manifest list is not supported");
            }
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
    }
}
