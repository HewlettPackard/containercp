/* Copyright 2023-2024 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContainerdLibrary.Tests
{
    [TestClass]
    public class TarGzHelperTests
    {
        [TestMethod]
        [DeploymentItem(@"TestData\io.containerd.content.v1.content\blobs\sha256\bea1513c492de5b5e94bf8949d2cc55b3eaa9775467c0f243f8f8e68a6496e71", @"TestData\io.containerd.content.v1.content\blobs\sha256\bea1513c492de5b5e94bf8949d2cc55b3eaa9775467c0f243f8f8e68a6496e71")]
        public void Test_IsFileExistsInTarGz()
        {
            // Arrange
            string archivePath = @"TestData\io.containerd.content.v1.content\blobs\sha256\bea1513c492de5b5e94bf8949d2cc55b3eaa9775467c0f243f8f8e68a6496e71";
            string pathInArchive = "/var/lib/dpkg/diversions";

            // Act
            bool isFileExists = TarHelper.IsFileExistsInTarGzip(archivePath, pathInArchive, out FileMetadata fileMetadata);

            // Assert
            Assert.IsTrue(isFileExists);
            Assert.AreEqual(0, fileMetadata.UserID);
            Assert.AreEqual(0, fileMetadata.GroupID);
            Assert.AreEqual(33188, fileMetadata.Mode);
        }
    }
}
