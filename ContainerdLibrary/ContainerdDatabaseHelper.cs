/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using BoltDB;
using ContainerdLibrary;
using System.Collections.Generic;
using System.IO;

namespace containercp
{
    internal class ContainerdDatabaseHelper
    {
        private const string V1BucketName = "v1";
        private const string KubernetesIoBucketName = "k8s.io";
        private const string ContainersBucketName = "containers";
        private const string ImagesBucketName = "images";
        private const string SnapshotsBucketName = "snapshots";
        private const string OverlayFSBucketName = "overlayfs";

        private const string LabelsBucketName = "labels";
        private const string ContainerNameKey = "io.kubernetes.container.name";
        private const string PodNameKey = "io.kubernetes.pod.name";
        private const string PodNamespaceKey = "io.kubernetes.pod.namespace";
        private const string ContainerImageKey = "image";

        private string m_databasePath;

        public ContainerdDatabaseHelper(string databasePath)
        {
            m_databasePath = databasePath;
        }

        public bool IsDatabaseExists()
        {
            return File.Exists(m_databasePath);
        }

        public ContainerDetails GetContainerDetailsByContainerID(string containerID)
        {
            List<ContainerDetails> result = new List<ContainerDetails>();
            BoltDatabase containerdDatabase = BoltDatabaseHelper.Open(m_databasePath);
            Bucket v1Bucket = containerdDatabase.GetBucket(V1BucketName);
            Bucket k8sIoBucket = v1Bucket.GetBucket(KubernetesIoBucketName);
            Bucket containersBucket = k8sIoBucket.GetBucket(ContainersBucketName);
            Bucket containerBucket = containersBucket.GetBucket(containerID);
            if (containerBucket == null)
            {
                return null;
            }

            return GetContainerDetails(containerID, containerBucket);
        }

        public List<ContainerDetails> GetContainerDetailsByContainerName(string containerName)
        {
            List<ContainerDetails> result = new List<ContainerDetails>();
            BoltDatabase containerdDatabase = BoltDatabaseHelper.Open(m_databasePath);
            Bucket v1Bucket = containerdDatabase.GetBucket(V1BucketName);
            Bucket k8sIoBucket = v1Bucket.GetBucket(KubernetesIoBucketName);
            Bucket containersBucket = k8sIoBucket.GetBucket(ContainersBucketName);
            foreach(KeyValuePair<string, Bucket> bucketElement in containersBucket.GetBuckets())
            {
                string containerID = bucketElement.Key;
                Bucket containerBucket = bucketElement.Value;
                ContainerDetails containerDetails = GetContainerDetails(containerID, containerBucket);
                if (containerDetails.ContainerName == containerName)
                {
                    result.Add(containerDetails);
                }
            }

            return result;
        }

        public List<ContainerDetails> GetAllContainers()
        {
            List<ContainerDetails> result = new List<ContainerDetails>();
            BoltDatabase containerdDatabase = BoltDatabaseHelper.Open(m_databasePath);
            Bucket v1Bucket = containerdDatabase.GetBucket(V1BucketName);
            Bucket k8sIoBucket = v1Bucket.GetBucket(KubernetesIoBucketName);
            Bucket containersBucket = k8sIoBucket.GetBucket(ContainersBucketName);
            foreach (KeyValuePair<string, Bucket> bucketElement in containersBucket.GetBuckets())
            {
                string containerID = bucketElement.Key;
                Bucket containerBucket = bucketElement.Value;
                ContainerDetails containerDetails = GetContainerDetails(containerID, containerBucket);
                result.Add(containerDetails);
            }

            return result;
        }

        public List<KeyValuePair<string, BlobReference>> GetAllImages()
        {
            List<KeyValuePair<string, BlobReference>> result = new List<KeyValuePair<string, BlobReference>>();
            BoltDatabase containerdDatabase = BoltDatabaseHelper.Open(m_databasePath);
            Bucket v1Bucket = containerdDatabase.GetBucket(V1BucketName);
            Bucket k8sIoBucket = v1Bucket.GetBucket(KubernetesIoBucketName);
            Bucket imagesBucket = k8sIoBucket.GetBucket(ImagesBucketName);
            foreach (KeyValuePair<string, Bucket> imageBucket in imagesBucket.GetBuckets())
            {
                string imageIdentifier = imageBucket.Key;
                BlobReference manifestReference = GetImageManifestBlobReference(imageBucket.Value);
                
                result.Add(new KeyValuePair<string, BlobReference>(imageIdentifier, manifestReference));
            }
            return result;
        }

        public BlobReference GetImageManifestReference(string imageIdentifier)
        {
            BoltDatabase containerdDatabase = BoltDatabaseHelper.Open(m_databasePath);
            Bucket v1Bucket = containerdDatabase.GetBucket(V1BucketName);
            Bucket k8sIoBucket = v1Bucket.GetBucket(KubernetesIoBucketName);
            Bucket imagesBucket = k8sIoBucket.GetBucket(ImagesBucketName);
            Bucket imageBucket = imagesBucket.GetBucket(imageIdentifier);
            if (imageBucket == null)
            {
                return null;
            }

            return GetImageManifestBlobReference(imageBucket);
        }

        public string GetSnapshotName(string containerSnapshotKey)
        {
            BoltDatabase containerdDatabase = BoltDatabaseHelper.Open(m_databasePath);
            Bucket v1Bucket = containerdDatabase.GetBucket(V1BucketName);
            Bucket k8sIoBucket = v1Bucket.GetBucket(KubernetesIoBucketName);
            Bucket snapshotsBucket = k8sIoBucket.GetBucket(SnapshotsBucketName);
            Bucket overlayfsBucket = snapshotsBucket.GetBucket(OverlayFSBucketName);
            Bucket snapshotBucket = overlayfsBucket.GetBucket(containerSnapshotKey);
            if (snapshotBucket == null)
            {
                return null;
            }

            return snapshotBucket.GetStringValueByKey("name");
        }

        /// <returns>List of snapshots of layers from lowest to highest</returns>
        public List<string> GetLayersSnapshotNames(string containerSnapshotKey)
        {
            List<string> result = new List<string>();
            BoltDatabase containerdDatabase = BoltDatabaseHelper.Open(m_databasePath);
            Bucket v1Bucket = containerdDatabase.GetBucket(V1BucketName);
            Bucket k8sIoBucket = v1Bucket.GetBucket(KubernetesIoBucketName);
            Bucket snapshotsBucket = k8sIoBucket.GetBucket(SnapshotsBucketName);
            Bucket overlayfsBucket = snapshotsBucket.GetBucket(OverlayFSBucketName);
            Bucket containerSnapshotBucket = overlayfsBucket.GetBucket(containerSnapshotKey);
            string parentDigest = containerSnapshotBucket.GetStringValueByKey("parent");
            while (parentDigest != null)
            {
                Bucket parentBucket = overlayfsBucket.GetBucket(parentDigest);
                string name = parentBucket.GetStringValueByKey("name");
                result.Add(name);
                parentDigest = parentBucket.GetStringValueByKey("parent");
            }

            result.Reverse();

            return result;
        }

        private static ContainerDetails GetContainerDetails(string containerID, Bucket containerBucket)
        {
            Bucket labelsBucket = containerBucket.GetBucket(LabelsBucketName);
            string containerName = labelsBucket.GetStringValueByKey(ContainerNameKey);
            string podName = labelsBucket.GetStringValueByKey(PodNameKey);
            string podNamespace = labelsBucket.GetStringValueByKey(PodNamespaceKey);
            string imageIdentifier = containerBucket.GetStringValueByKey(ContainerImageKey);
            string snapshotterKey = containerBucket.GetStringValueByKey("snapshotKey");
            return new ContainerDetails(containerID, containerName, imageIdentifier, snapshotterKey, podName, podNamespace);
        }

        public static uint ToUInt32(byte[] buffer, int offset)
        {
            return (uint)((buffer[offset + 0] << 24) | (buffer[offset + 1] << 16)
                | (buffer[offset + 2] << 8) | (buffer[offset + 3] << 0));
        }

        public static ulong ToUInt64(byte[] buffer, int offset)
        {
            return (((ulong)ToUInt32(buffer, offset + 0)) << 32) | ToUInt32(buffer, offset + 4);
        }

        private static BlobReference GetImageManifestBlobReference(Bucket imageBucket)
        {
            Bucket targetBucket = imageBucket.GetBucket("target");
            if (targetBucket == null)
            {
                throw new InvalidDataException("target bucket is missing");
            }

            string mediaType = targetBucket.GetStringValueByKey("mediatype");
            DockerMediaType dockerMediaType = DockerMediaTypeParser.ParseManifestMediaType(mediaType);
            string digest = targetBucket.GetStringValueByKey("digest");
            byte[] sizeBytes = (byte[])targetBucket.GetElementValueByKey("size");
            long size = VarIntConverter.ToInt64(sizeBytes);
            return new BlobReference(digest, size, dockerMediaType);
        }

        
    }
}
