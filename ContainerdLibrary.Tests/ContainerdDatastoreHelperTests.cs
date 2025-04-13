/* Copyright 2023-2025 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContainerdLibrary.Tests
{
    [TestClass]
    public class ContainerdDatastoreHelperTests
    {
        [TestMethod]
        public void Test_CalculateImageChainID()
        {
            // Arrange
            string layer0DiffID = "sha256:fffe76c64ef2dee2d80a8bb3ad13d65d596d04a45510b1956a976a69215dae92";
            string layer1DiffID = "sha256:24442061199135f3ef00507fd48e7dfe8df8bc30fb4172ab9a455f1700a8cda1";
            
            // Act
            string layer1ChainID = ContainerdDatastoreHelper.CalculateImageChainID(layer0DiffID, layer1DiffID);

            // Assert
            string expected = "sha256:e359fe7804a37f45a94789ad750cbc9d1424f3a3eb8ad100946af9baf6bfc3b5";
            Assert.AreEqual(expected, layer1ChainID);
        }
    }
}
