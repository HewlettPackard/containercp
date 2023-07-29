/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
namespace ContainerdLibrary
{
    public class ContainerDetails
    {
        public string ContainerID { get; set; }
        
        public string ContainerName { get; set; }

        public string ImageIdentifier { get; set; }

        public string SnapshotKey { get; set; }

        public string PodName { get; set; }

        public string PodNamespace { get; set; }

        public ContainerDetails(string containerID, string containerName, string imageIdentifier, string snapshotKey, string podName, string podNamespace)
        {
            ContainerID = containerID;
            ContainerName = containerName;
            ImageIdentifier = imageIdentifier;
            SnapshotKey = snapshotKey;
            PodName = podName;
            PodNamespace = podNamespace;
        }
    }
}
