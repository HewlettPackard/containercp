/* Copyright 2023-2025 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ContainerdLibrary
{
    public class ImageConfigParser
    {
        private const string RootFsPropertyName = "rootfs";
        private const string DiffIDsPropertyName = "diff_ids";

        public static List<string> GetImageDiffIDs(string configJson)
        {
            List<string> result = new List<string>();
            JObject config = JObject.Parse(configJson);
            JObject rootFs = config.GetValue(RootFsPropertyName) as JObject;
            JArray entries = rootFs.GetValue(DiffIDsPropertyName) as JArray;
            foreach (JValue entry in entries)
            {
                string hash = entry.Value<string>();
                result.Add(hash);
            }

            return result;
        }
    }
}
