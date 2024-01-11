/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using ContainerdLibrary;
using System;
using System.Collections.Generic;

namespace containercp
{
    public static class InfoManager
    {
        public static void ShowInfo(string containerName)
        {
            if (containerName.Contains(":"))
            {
                Console.WriteLine("Invalid arguments");
                return;
            }

            ContainerdDatastoreHelper datastoreHelper = new ContainerdDatastoreHelper();
            if (!datastoreHelper.IsDatabaseExists())
            {
                Console.WriteLine("Error: containerd database was not found");
                return;
            }
            List<ContainerDetails> containerDetailsList = datastoreHelper.GetContainerDetails(containerName);
            if (containerDetailsList.Count == 0)
            {
                Console.WriteLine($"Container {containerName} was not found");
                return;
            }
            
            foreach (ContainerDetails containerDetails in containerDetailsList)
            {
                string imageIdentifer = containerDetails.ImageIdentifier;
                Console.WriteLine($"Container ID: {containerDetails.ContainerID}");
                Console.WriteLine($"Image identifier: {imageIdentifer}");
                string snapshotKey = containerDetails.SnapshotKey;
                Console.WriteLine($"Snapshot Key: {snapshotKey}");
                string containerSnapshotName = datastoreHelper.GetSnapshotName(snapshotKey);
                Console.WriteLine($"Snapshot Name: {containerSnapshotName}");
                ulong? containerSnapshotID;
                try
                {
                    containerSnapshotID = datastoreHelper.GetSnapshotID(containerSnapshotName); ;
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine("Error: Access Denied, this program needs superuser permissions!");
                    return;
                }

                if (containerSnapshotID.HasValue)
                {
                    Console.WriteLine($"Snapshot ID: {containerSnapshotID}");
                }
                BlobReference manifestReference = datastoreHelper.GetImageManifestReference(containerDetails.ImageIdentifier);
                Console.WriteLine($"Image manifest reference: {manifestReference.Hash}");
                List<BlobReference> layerReferences = datastoreHelper.GetImageLayers(containerDetails.ImageIdentifier, PlatformHelper.CurrentPlatform);
                List<string> layerSnapshotNames = datastoreHelper.GetLayersSnapshotNames(snapshotKey);
                for (int index = 0; index < layerReferences.Count; index++)
                {
                    BlobReference layerReference = layerReferences[index];
                    string snapshotName = layerSnapshotNames[index];
                    Console.WriteLine($"Layer {index}: {layerReference.Hash}, size: {layerReference.Size}");
                    ulong? snapshotID = datastoreHelper.GetSnapshotID(snapshotName);
                    Console.WriteLine($"  Snapshot ID: {snapshotID}");
                    Console.WriteLine($"  Snapshot Name: {snapshotName}");
                    List<ContainerDetails> containersUsingLayer = datastoreHelper.GetContainersUsingLayer(layerReference, PlatformHelper.CurrentPlatform);
                    foreach (ContainerDetails containerUsingLayer in containersUsingLayer)
                    {
                        if (containerUsingLayer.ContainerName == containerName)
                        {
                            continue;
                        }
                        Console.WriteLine($"  Container using this layer: {containerUsingLayer.ContainerName}");
                    }
                }
            }
        }
    }
}
