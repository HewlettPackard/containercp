/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using containercp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace ContainerdLibrary.Tests
{
    [TestClass]
    [DeploymentItem(@"TestData\io.containerd.metadata.v1.bolt\meta.db", @"TestData\io.containerd.metadata.v1.bolt\meta.db")]
    public class ContainerdDatabaseHelperTests
    {
        [TestMethod]
        public void Test_GetContainerDetailsByContainerName()
        {
            // Arrange
            ContainerdDatabaseHelper databaseHelper = new ContainerdDatabaseHelper(@"TestData\io.containerd.metadata.v1.bolt\meta.db");
            
            // Act
            List<ContainerDetails> containerDetails = databaseHelper.GetContainerDetailsByContainerName("coredns");

            // Assert
            Assert.AreEqual(1, containerDetails.Count);
            Assert.AreEqual("64066e8fd336a16f397f2d7b72dacbd6221b2c9696d7c49b31eef44bfcd033c0", containerDetails[0].ContainerID);
            Assert.AreEqual("docker.io/coredns/coredns:1.10.0", containerDetails[0].ImageIdentifier);
            Assert.AreEqual("64066e8fd336a16f397f2d7b72dacbd6221b2c9696d7c49b31eef44bfcd033c0", containerDetails[0].SnapshotKey);
        }

        [TestMethod]
        public void Test_GetImageManifestReference()
        {
            // Arrange
            ContainerdDatabaseHelper databaseHelper = new ContainerdDatabaseHelper(@"TestData\io.containerd.metadata.v1.bolt\meta.db");
            string imageIdentifier = "docker.io/coredns/coredns:1.10.0";

            // Act
            BlobReference blobReference = databaseHelper.GetImageManifestReference(imageIdentifier);

            // Assert
            Assert.AreEqual("sha256", blobReference.HashType);
            Assert.AreEqual("017727efcfeb7d053af68e51436ce8e65edbc6ca573720afb4f79c8594036955", blobReference.Hash);
            Assert.AreEqual(1976, blobReference.Size);
        }

        [TestMethod]
        public void Test_GetLayersSnapshotNames()
        {
            // Arrange
            ContainerdDatabaseHelper databaseHelper = new ContainerdDatabaseHelper(@"TestData\io.containerd.metadata.v1.bolt\meta.db");
            string snapshotKey = "64066e8fd336a16f397f2d7b72dacbd6221b2c9696d7c49b31eef44bfcd033c0";

            // Act
            List<string> layersSnapshotNames = databaseHelper.GetLayersSnapshotNames(snapshotKey);

            // Assert
            Assert.AreEqual(2, layersSnapshotNames.Count);
        }
    }
}