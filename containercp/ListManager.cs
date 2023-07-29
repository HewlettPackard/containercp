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
    public class ListManager
    {
        public static void ListContainers()
        {
            ContainerdDatastoreHelper datastoreHelper = new ContainerdDatastoreHelper();
            if (!datastoreHelper.IsDatabaseExists())
            {
                Console.WriteLine("Error: containerd database was not found");
                return;
            }
            List<ContainerDetails> containers = datastoreHelper.GetAllContainers();
            foreach (ContainerDetails container in containers)
            {
                Console.WriteLine($"ID: {container.ContainerID}");
                Console.WriteLine($" Name: {container.ContainerName}");
                Console.WriteLine($" Image: {container.ImageIdentifier}");
                Console.WriteLine($" Pod: {container.PodName}");
            }
        }

        public static void ListImages()
        {
            ContainerdDatastoreHelper datastoreHelper = new ContainerdDatastoreHelper();
            if (!datastoreHelper.IsDatabaseExists())
            {
                Console.WriteLine("Error: containerd database was not found");
                return;
            }
            List<KeyValuePair<string, BlobReference>> images = datastoreHelper.GetAllImages();
            foreach (KeyValuePair<string, BlobReference> image in images)
            {
                Console.WriteLine(image.Key);
            }
        }
    }
}
