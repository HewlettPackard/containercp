/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContainerdLibrary.Tests
{
    [TestClass]
    [DeploymentItem(@"TestData\io.containerd.snapshotter.v1.overlayfs\metadata.db", @"TestData\io.containerd.snapshotter.v1.overlayfs\metadata.db")]
    public class SnapshotDatabaseHelperTests
    {
        [TestMethod]
        public void Test_GetSnapshotID()
        {
            // Arrange
            SnapshotDatabaseHelper snapshotDatabaseHelper = new SnapshotDatabaseHelper(@"TestData\io.containerd.snapshotter.v1.overlayfs\metadata.db");
            string snapshotName = "k8s.io/61/sha256:d36071bbf41810d9b17dd8a0d0bab69945e0f8f1907aa1b87f11c0a27728b9ca";

            // Act
            ulong? snapshotID = snapshotDatabaseHelper.GetSnapshotID(snapshotName);

            // Assert
            Assert.AreEqual((ulong)35, snapshotID);
        }
    }
}
