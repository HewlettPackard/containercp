﻿/* Copyright 2023 Hewlett Packard Enterprise Development LP.
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
    public class ImageManifestParserTests
    {
        [TestMethod]
        [DeploymentItem(@"TestData\io.containerd.content.v1.content\blobs\sha256\7d257bb9ceb6e3f6b16e1c37b01459b5964a1b3a715a7065507178c17b7e3d38", @"TestData\io.containerd.content.v1.content\blobs\sha256\7d257bb9ceb6e3f6b16e1c37b01459b5964a1b3a715a7065507178c17b7e3d38")]
        public void Test_GetLayersHashList()
        {
            // Arrange
            string manifestPath = @"TestData\io.containerd.content.v1.content\blobs\sha256\7d257bb9ceb6e3f6b16e1c37b01459b5964a1b3a715a7065507178c17b7e3d38";
            string manifest = File.ReadAllText(manifestPath);
            
            // Act
            List<BlobReference> layers = ImageManifestParser.GetImageLayers(manifest);

            // Assert
            Assert.AreEqual(19, layers.Count);
        }

        [TestMethod]
        [DeploymentItem(@"TestData\io.containerd.content.v1.content\blobs\sha256\7d257bb9ceb6e3f6b16e1c37b01459b5964a1b3a715a7065507178c17b7e3d38", @"TestData\io.containerd.content.v1.content\blobs\sha256\7d257bb9ceb6e3f6b16e1c37b01459b5964a1b3a715a7065507178c17b7e3d38")]
        public void Test_GetImageConfigBlobReference()
        {
            // Arrange
            string manifestPath = @"TestData\io.containerd.content.v1.content\blobs\sha256\7d257bb9ceb6e3f6b16e1c37b01459b5964a1b3a715a7065507178c17b7e3d38";
            string manifest = File.ReadAllText(manifestPath);

            // Act
            BlobReference configBlobReference = ImageManifestParser.GetImageConfigBlobReference(manifest);

            // Assert
            Assert.AreEqual(DockerMediaType.ImageConfigV1Json, configBlobReference.DockerMediaType);
            Assert.AreEqual(6243, configBlobReference.Size);
            Assert.AreEqual("85b54b71a42fb89ae671a1d77faf69a61d6046647c328c610059cd158fa48408", configBlobReference.Hash);
            Assert.AreEqual("sha256", configBlobReference.HashType);
        }
    }
}
