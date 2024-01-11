/* Copyright 2023-2024 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

namespace ContainerdLibrary.Tests
{
    [TestClass]
    public class ImageManifestListParserTests
    {
        [TestMethod]
        [DeploymentItem(@"TestData\io.containerd.content.v1.content\blobs\sha256\017727efcfeb7d053af68e51436ce8e65edbc6ca573720afb4f79c8594036955", @"TestData\io.containerd.content.v1.content\blobs\sha256\017727efcfeb7d053af68e51436ce8e65edbc6ca573720afb4f79c8594036955")]
        public void Test_GetImageManifests()
        {
            // Arrange
            string manifestListPath = @"TestData\io.containerd.content.v1.content\blobs\sha256\017727efcfeb7d053af68e51436ce8e65edbc6ca573720afb4f79c8594036955";
            string manifestList = File.ReadAllText(manifestListPath);

            // Act
            List<KeyValuePair<BlobReference, DockerPlatform>> manifests = ImageManifestListParser.GetImageManifests(manifestList);

            // Assert
            Assert.AreEqual(6, manifests.Count);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\io.containerd.content.v1.content\blobs\sha256\017727efcfeb7d053af68e51436ce8e65edbc6ca573720afb4f79c8594036955", @"TestData\io.containerd.content.v1.content\blobs\sha256\017727efcfeb7d053af68e51436ce8e65edbc6ca573720afb4f79c8594036955")]
        public void Test_GetImageManifestReferences()
        {
            // Arrange
            string manifestListPath = @"TestData\io.containerd.content.v1.content\blobs\sha256\017727efcfeb7d053af68e51436ce8e65edbc6ca573720afb4f79c8594036955";
            string manifestList = File.ReadAllText(manifestListPath);

            // Act
            BlobReference blobReference = ImageManifestListParser.GetImageManifestReference(manifestList, new DockerPlatform("amd64", "linux"));

            // Assert
            Assert.AreEqual("12fa9b2b130df04d58cc01e117b4ac91d6657e0ca0a6c71bd7da8f4f475b925a", blobReference.Hash);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\io.containerd.content.v1.content\blobs\sha256\017727efcfeb7d053af68e51436ce8e65edbc6ca573720afb4f79c8594036955", @"TestData\io.containerd.content.v1.content\blobs\sha256\017727efcfeb7d053af68e51436ce8e65edbc6ca573720afb4f79c8594036955")]
        public void Test_IsManifestList()
        {
            // Arrange
            string manifestPath = @"TestData\io.containerd.content.v1.content\blobs\sha256\017727efcfeb7d053af68e51436ce8e65edbc6ca573720afb4f79c8594036955";
            string manifest = File.ReadAllText(manifestPath);

            // Act
            bool isManifestList = ImageManifestListParser.IsManifestList(manifest);

            // Assert
            Assert.AreEqual(true, isManifestList);
        }
    }
}
