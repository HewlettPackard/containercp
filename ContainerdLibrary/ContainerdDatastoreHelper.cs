/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using containercp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ContainerdLibrary
{
    public class ContainerdDatastoreHelper
    {
        private const string DefaultContainerdDirectoryPath = "/var/snap/microk8s/common/var/lib/containerd";
        private const string BlobsRelativePath = "io.containerd.content.v1.content/blobs";
        private const string DatabaseRelativePath = "io.containerd.metadata.v1.bolt/meta.db";
        private const string SnapshotDatabaseRelativePath = "io.containerd.snapshotter.v1.overlayfs/metadata.db";
        private const string SnapshotsDirectoryRelativePath = "io.containerd.snapshotter.v1.overlayfs/snapshots";
        private const string Sha256HashType = "sha256";

        private string m_containerdDirectoryPath;
        private ContainerdDatabaseHelper m_databaseHelper;
        private SnapshotDatabaseHelper m_snapshotDatabaseHelper;

        public ContainerdDatastoreHelper() : this(DefaultContainerdDirectoryPath)
        {
        }

        public ContainerdDatastoreHelper(string containerdDirectoryPath)
        {
            m_containerdDirectoryPath = containerdDirectoryPath;
            string databasePath = Path.Combine(containerdDirectoryPath, DatabaseRelativePath);
            m_databaseHelper = new ContainerdDatabaseHelper(databasePath);
            string snapshotDatabasePath = Path.Combine(containerdDirectoryPath, SnapshotDatabaseRelativePath);
            m_snapshotDatabaseHelper = new SnapshotDatabaseHelper(snapshotDatabasePath);
        }

        public bool IsDatabaseExists()
        {
            return m_databaseHelper.IsDatabaseExists();
        }

        public string GetBlobPath(BlobReference reference)
        {
            return Path.Combine(m_containerdDirectoryPath, BlobsRelativePath, reference.HashType, reference.Hash);
        }

        public BlobReference GetImageManifestReference(string imageIdentifier)
        {
            return m_databaseHelper.GetImageManifestReference(imageIdentifier);
        }

        public string GetImageManifest(string imageIdentifier, DockerPlatform platform)
        {
            BlobReference blobReference = m_databaseHelper.GetImageManifestReference(imageIdentifier);
            if (blobReference == null)
            {
                throw new ArgumentException($"Cannot find image manifest for {imageIdentifier}");
            }

            string manifestPath = GetBlobPath(blobReference);
            string manifestText = File.ReadAllText(manifestPath);
            if (DockerMediaTypeHelper.IsManifestList(blobReference.DockerMediaType))
            {
                blobReference = ImageManifestListParser.GetImageManifestReference(manifestText, platform);
                manifestPath = GetBlobPath(blobReference);
                manifestText = File.ReadAllText(manifestPath);
            }
            return manifestText;
        }

        public List<BlobReference> GetImageLayers(string imageIdentifier, DockerPlatform platform)
        {
            string imageManifest = GetImageManifest(imageIdentifier, platform);
            return ImageManifestParser.GetImageLayers(imageManifest);
        }

        public List<string> GetImageDiffIDs(string imageIdentifier, DockerPlatform platform)
        {
            string imageManifest = GetImageManifest(imageIdentifier, platform);
            BlobReference configReference = ImageManifestParser.GetImageConfigBlobReference(imageManifest);
            string configPath = GetBlobPath(configReference);
            string configText = File.ReadAllText(configPath);
            return ImageConfigParser.GetImageDiffIDs(configText);
        }

        public List<string> GetImageChainIDs(string imageIdentifier, DockerPlatform platform)
        {
            List<string> diffIDs = GetImageDiffIDs(imageIdentifier, platform);
            List<string> result = new List<string>(diffIDs.Count);
            if (diffIDs.Count > 0)
            {
                result.Add(diffIDs[0]);
            }

            for (int index = 1; index < diffIDs.Count; index++)
            {
                string previousChainID = result[index - 1];
                string chainID = CalculateImageChainID(previousChainID, diffIDs[index]);
                result.Add(chainID);
            }

            return result;
        }

        public string GetSnapshotName(string snapshotKey)
        {
            return m_databaseHelper.GetSnapshotName(snapshotKey);
        }

        public List<ContainerDetails> GetAllContainers()
        {
            return m_databaseHelper.GetAllContainers();
        }

        public List<ContainerDetails> GetContainerDetails(string containerName)
        {
            return m_databaseHelper.GetContainerDetailsByContainerName(containerName);
        }

        public List<KeyValuePair<string, BlobReference>> GetAllImages()
        {
            return m_databaseHelper.GetAllImages();
        }

        public bool IsFileExistsInLayer(BlobReference reference, string pathInContainer, out FileMetadata fileMetadata)
        {
            string blobPath = GetBlobPath(reference);
            if (reference.DockerMediaType == DockerMediaType.ImageTarGzip)
            {
                return TarHelper.IsFileExistsInTarGzip(blobPath, pathInContainer, out fileMetadata);
            }
            else
            {
                return TarHelper.IsFileExistsInTar(blobPath, pathInContainer, out fileMetadata);
            }
        }

        public List<ContainerDetails> GetContainersUsingLayer(BlobReference reference, DockerPlatform platform)
        {
            List<string> images = GetImagesUsingLayer(reference, platform);
            List<ContainerDetails> containers = m_databaseHelper.GetAllContainers();
            List<ContainerDetails> result = new List<ContainerDetails>();
            foreach (ContainerDetails container in containers)
            {
                if (images.Contains(container.ImageIdentifier))
                {
                    result.Add(container);
                }
            }

            return result;
        }

        public List<string> GetImagesUsingLayer(BlobReference reference, DockerPlatform platform)
        {
            List<KeyValuePair<string, BlobReference>> images = m_databaseHelper.GetAllImages();
            List<string> result = new List<string>();
            foreach (KeyValuePair<string, BlobReference> image in images)
            {
                string imageIdentifier = image.Key;
                string manifest = GetImageManifest(imageIdentifier, platform);
                List<BlobReference> blobReferences = ImageManifestParser.GetImageLayers(manifest);
                if (blobReferences.Contains(reference))
                {
                    result.Add(imageIdentifier);
                }
            }

            return result;
        }

        public List<string> GetNamedImagesUsingLayer(BlobReference reference, DockerPlatform platform)
        {
            List<string> images = GetImagesUsingLayer(reference, platform);
            List<string> result = new List<string>();
            foreach(string image in images)
            {
                if (!image.StartsWith($"{Sha256HashType}:") && !image.Contains($"@{Sha256HashType}:"))
                {
                    result.Add(image);
                }
            }

            return result;
        }

        public int FindImageLayerIndexContainingFile(List<BlobReference> imageLayers, string pathInImage)
        {
            return FindImageLayerIndexContainingFile(imageLayers, pathInImage, out _);
        }

        public int FindImageLayerIndexContainingFile(List<BlobReference> imageLayers, string pathInImage, out FileMetadata fileMetadata)
        {
            for (int index = imageLayers.Count - 1; index >= 0; index--)
            {
                BlobReference layerReference = imageLayers[index];
                if (IsFileExistsInLayer(layerReference, pathInImage, out fileMetadata))
                {
                    return index;
                }
            }

            fileMetadata = null;
            return -1;
        }

        public int FindImageLayerIndexForStoringFile(List<BlobReference> imageLayers, string pathInImage, DockerPlatform platform)
        {
            int layerContaningFileIndex = FindImageLayerIndexContainingFile(imageLayers, pathInImage);

            if (layerContaningFileIndex >= 0)
            {
                int numberOfImagesUsingLayer = GetNamedImagesUsingLayer(imageLayers[layerContaningFileIndex], platform).Count;
                if (numberOfImagesUsingLayer == 1)
                {
                    return layerContaningFileIndex;
                }
            }

            BlobReference topLayerReference = imageLayers[imageLayers.Count - 1];
            if (GetNamedImagesUsingLayer(topLayerReference, platform).Count == 1)
            {
                return imageLayers.Count - 1;
            }

            return -1;
        }

        public List<string> GetLayersSnapshotNames(string containerSnapshotKey)
        {
            return m_databaseHelper.GetLayersSnapshotNames(containerSnapshotKey);
        }

        public ulong? GetSnapshotID(string snapshotName)
        {
            return m_snapshotDatabaseHelper.GetSnapshotID(snapshotName);
        }

        public string GetSnapshotPath(string snapshotName)
        {
            ulong? snapshotID = m_snapshotDatabaseHelper.GetSnapshotID(snapshotName);
            if (!snapshotID.HasValue)
            {
                return null;
            }
            return Path.Combine(m_containerdDirectoryPath, SnapshotsDirectoryRelativePath, snapshotID.Value.ToString());
        }

        public void ExtractFileFromImageLayer(BlobReference layerReference, string pathInImage, string outputPath)
        {
            string layerFilePath = GetBlobPath(layerReference);
            if (layerReference.DockerMediaType == DockerMediaType.ImageTarGzip)
            {
                TarHelper.ExtractFileFromTarGzip(layerFilePath, pathInImage, outputPath);
            }
            else
            {
                TarHelper.ExtractFileFromTar(layerFilePath, pathInImage, outputPath);
            }
        }

        public BlobReference PutFileInImageLayer(BlobReference layerReference, string pathInImage, FileStream fileStream, bool overwrite, FileMetadata fileMetadata = null)
        {
            string layerFilePath = GetBlobPath(layerReference);
            // We put the file in the blob directory and not in /tmp to avoid copy time (in case /tmp is mapped to a different partition)
            string tempOutputPath = Path.Combine(DefaultContainerdDirectoryPath, BlobsRelativePath, "TempFile");
            if (layerReference.DockerMediaType == DockerMediaType.ImageTarGzip)
            {
                TarHelper.PutFileInTarGzipArchive(layerFilePath, pathInImage, fileStream, tempOutputPath, fileMetadata);
            }
            else
            {
                TarHelper.PutFileInTarArchive(layerFilePath, pathInImage, fileStream, tempOutputPath, fileMetadata);
            }

            if (overwrite)
            {
                File.Delete(layerFilePath);
                File.Move(tempOutputPath, layerFilePath);
                return layerReference;
            }
            else
            {
                string hash = HashCalculator.CalculateSha256HashString(tempOutputPath);
                string updatedLayerPath = Path.Combine(m_containerdDirectoryPath, BlobsRelativePath, Sha256HashType, hash);
                File.Move(tempOutputPath, updatedLayerPath);
                long blobSize = new FileInfo(updatedLayerPath).Length;

                return new BlobReference(Sha256HashType, hash, blobSize, layerReference.DockerMediaType);
            }
        }

        internal static string CalculateImageChainID(string previousChainID, string diffID)
        {
            string stringToHash = previousChainID + " " + diffID;
            return "sha256:" + HashCalculator.CalculateSha256HashString(Encoding.ASCII.GetBytes(stringToHash));
        }
    }
}
