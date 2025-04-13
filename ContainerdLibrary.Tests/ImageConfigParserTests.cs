/* Copyright 2023 Hewlett Packard Enterprise Development LP.
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
    public class ImageConfigParserTests
    {
        [TestMethod]
        [DeploymentItem(@"TestData\io.containerd.content.v1.content\blobs\sha256\7c2d729072a7ad556cb0ac2f53bfb5b1ab6dd511d1b48be9f009c9166bde6ee3", @"TestData\io.containerd.content.v1.content\blobs\sha256\7c2d729072a7ad556cb0ac2f53bfb5b1ab6dd511d1b48be9f009c9166bde6ee3")]
        public void Test_GetLayersHashList()
        {
            // Arrange
            string manifestPath = @"TestData\io.containerd.content.v1.content\blobs\sha256\7c2d729072a7ad556cb0ac2f53bfb5b1ab6dd511d1b48be9f009c9166bde6ee3";
            string manifest = File.ReadAllText(manifestPath);

            // Act
            List<string> diffIDs = ImageConfigParser.GetImageDiffIDs(manifest);

            // Assert
            Assert.AreEqual(4, diffIDs.Count);
            Assert.AreEqual("sha256:fffe76c64ef2dee2d80a8bb3ad13d65d596d04a45510b1956a976a69215dae92", diffIDs[0]);
        }
    }
}
