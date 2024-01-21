/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using ContainerdLibrary;
using System;
using System.Collections.Generic;
using System.IO;

namespace containercp
{
    public class CopyManager
    {
        public static void ExecuteCopy(string source, string destination)
        {
            if (source.Contains(":") && destination.Contains(":"))
            {
                Console.WriteLine("Invalid arguments");
                return;
            }

            if (!source.Contains(":") && !destination.Contains(":"))
            {
                Console.WriteLine("Invalid arguments");
                return;
            }

            bool copyFromContainer = source.Contains(":");
            Console.WriteLine($"Copying from {source} to {destination}");
            if (copyFromContainer)
            {
                (string sourceContainerName, string pathInSourceContainer) = ParseContainerAndPath(source);
                CopyFromContainer(sourceContainerName, pathInSourceContainer, destination);
                
            }
            else
            {
                (string destinationContainerName, string pathInDestinationContainer) = ParseContainerAndPath(destination);
                CopyToContainer(source, destinationContainerName, pathInDestinationContainer);
            }
        }

        private static void CopyFromContainer(string sourceContainerName, string pathInSourceContainer, string destination)
        {
            ContainerdDatastoreHelper containerdDatastore = new ContainerdDatastoreHelper();
            if (!containerdDatastore.IsDatabaseExists())
            {
                Console.WriteLine("Error: containerd database was not found");
                return;
            }
            List<ContainerDetails> sourceContainerDetailsList = containerdDatastore.GetContainerDetails(sourceContainerName);
            sourceContainerDetailsList = RemoveDuplicateEntries(sourceContainerDetailsList);
            if (sourceContainerDetailsList.Count == 0)
            {
                Console.WriteLine($"Container {sourceContainerName} was not found");
                return;
            }

            if (sourceContainerDetailsList.Count > 1)
            {
                Console.WriteLine($"Container {sourceContainerName} is used by multiple instances");
                return;
            }

            ContainerDetails sourceContainerDetails = sourceContainerDetailsList[0];
            List<BlobReference> imageLayers = containerdDatastore.GetImageLayers(sourceContainerDetails.ImageIdentifier, PlatformHelper.CurrentPlatform);
            int layerIndex = containerdDatastore.FindImageLayerIndexContainingFile(imageLayers, pathInSourceContainer);

            if (layerIndex == -1)
            {
                Console.WriteLine($"Error: File {pathInSourceContainer} was not found in {sourceContainerName} container image");
                return;
            }

            Console.WriteLine($"Copying file from layer {layerIndex}");
            Console.WriteLine($"  Blob hash: {imageLayers[layerIndex].Hash}");
            containerdDatastore.ExtractFileFromImageLayer(imageLayers[layerIndex], pathInSourceContainer, destination);
        }

        private static void CopyToContainer(string source, string destinationContainerName, string pathInDestinationContainer)
        {
            ContainerdDatastoreHelper containerdDatastore = new ContainerdDatastoreHelper();
            if (!containerdDatastore.IsDatabaseExists())
            {
                Console.WriteLine("Error: containerd database was not found");
                return;
            }
            List<ContainerDetails> destinationContainerDetailsList = containerdDatastore.GetContainerDetails(destinationContainerName);
            destinationContainerDetailsList = RemoveDuplicateEntries(destinationContainerDetailsList);
            if (destinationContainerDetailsList.Count == 0)
            {
                Console.WriteLine($"Container {destinationContainerName} was not found");
                return;
            }

            if (destinationContainerDetailsList.Count > 1)
            {
                Console.WriteLine($"Container {destinationContainerName} is used by multiple instances");
                return;
            }

            ContainerDetails destinationContainerDetails = destinationContainerDetailsList[0];

            List<BlobReference> imageLayers = containerdDatastore.GetImageLayers(destinationContainerDetails.ImageIdentifier, PlatformHelper.CurrentPlatform);
            int layerIndex = containerdDatastore.FindImageLayerIndexContainingFile(imageLayers, pathInDestinationContainer, out FileMetadata fileMetadata);
            if (layerIndex >= 0)
            {
                List<string> imagesUsingLayer = containerdDatastore.GetNamedImagesUsingLayer(imageLayers[layerIndex], PlatformHelper.CurrentPlatform);
                if (imagesUsingLayer.Count > 1)
                {
                    Console.WriteLine($"The file is located on layer {layerIndex} which is used by the following images:");
                    foreach (string imageIdentifier in imagesUsingLayer)
                    {
                        Console.WriteLine($"Image identifier: {imageIdentifier}");
                    }
                }
            }

            layerIndex = containerdDatastore.FindImageLayerIndexForStoringFile(imageLayers, pathInDestinationContainer, PlatformHelper.CurrentPlatform);
            if (layerIndex == -1)
            {
                Console.WriteLine($"Error: Was not able to find suitable layer for {pathInDestinationContainer} in {destinationContainerName} container image");
                return;
            }

            Console.WriteLine($"Copying file to layer {layerIndex}");
            Console.WriteLine($"  Blob hash: {imageLayers[layerIndex].Hash}");

            FileStream sourceFileStream;
            try
            {
                sourceFileStream = File.OpenRead(source);
            }
            catch
            {
                Console.WriteLine($"Error: Cannot read source file '{source}'");
                return;
            }

            using (sourceFileStream)
            {
                try
                {
                    containerdDatastore.PutFileInImageLayer(imageLayers[layerIndex], pathInDestinationContainer, sourceFileStream, true, fileMetadata);
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Error: Access Denied, this program needs superuser permissions!");
                    return;
                }
            }

            // Update snapshots
            Console.WriteLine("Updating snapshot");
            List<string> layersSnapshotNames = containerdDatastore.GetLayersSnapshotNames(destinationContainerDetails.SnapshotKey);
            string layerSnapshotName = layersSnapshotNames[layerIndex];
            string layerSnapshotPath = containerdDatastore.GetSnapshotPath(layerSnapshotName);
            if (pathInDestinationContainer.StartsWith('/'))
            {
                pathInDestinationContainer = pathInDestinationContainer.Substring(1);
            }
            string pathInDestinationSnapshot = Path.Combine(layerSnapshotPath, "fs", pathInDestinationContainer);
            string parentDirectoryPath = Path.GetDirectoryName(pathInDestinationSnapshot);
            if (!Directory.Exists(parentDirectoryPath))
            {
                Directory.CreateDirectory(parentDirectoryPath);
            }
            File.Copy(source, pathInDestinationSnapshot, true);
            Console.WriteLine($"Updated snapshot, path: {layerSnapshotPath}");

            Console.WriteLine($"The {destinationContainerDetails.ContainerName} container has been modified");
            Console.WriteLine($"The following pods should be restarted for changes to take effect:");
            Console.WriteLine($"Pod name: {destinationContainerDetails.PodName}");
        }

        // TODO: Find better method to detect these unused entries
        private static List<ContainerDetails> RemoveDuplicateEntries(List<ContainerDetails> containers)
        {
            List<ContainerDetails> result = new List<ContainerDetails>();
            foreach(ContainerDetails container in containers)
            {
                if (!ContainesContainer(result, container.ContainerName, container.ImageIdentifier))
                {
                    result.Add(container);
                }
                else
                {
                    Console.WriteLine($"Ignoring duplicate container entry: {container.ContainerID}");
                }
            }

            return result;
        }

        private static bool ContainesContainer(List<ContainerDetails> containers, string containerName, string imageIdentifier)
        {
            foreach (ContainerDetails container in containers)
            {
                if (container.ContainerName == containerName &&
                    container.ImageIdentifier == imageIdentifier)
                {
                    return true;
                }
            }

            return false;
        }

        private static (string containerName, string pathInContainer) ParseContainerAndPath(string containerAndPath)
        {
            int containerNameEndIndex = containerAndPath.IndexOf(":");
            string containerName = containerAndPath.Substring(0, containerNameEndIndex);
            string pathInContainer = containerAndPath.Substring(containerNameEndIndex + 1);
            return (containerName, pathInContainer);
        }
    }
}
