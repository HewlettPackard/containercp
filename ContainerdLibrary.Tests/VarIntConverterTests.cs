/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ContainerdLibrary.Tests
{
    [TestClass]
    public class VarIntConverterTests
    {
        [TestMethod]
        public void Test_ToInt64()
        {
            byte[] value = new byte[] { 0xFE, 0x28 };

            long result = VarIntConverter.ToInt64(value);

            Assert.AreEqual(2623, result);
        }

        [TestMethod]
        public void Test_ToInt64_2()
        {
            byte[] value = new byte[] { 0x9A, 0x43 };

            long result = VarIntConverter.ToInt64(value);

            Assert.AreEqual(4301, result);
        }

        [TestMethod]
        public void Test_UToInt64()
        {
            byte[] value = new byte[] { 0xAC, 0x01 };

            ulong result = VarIntConverter.ToUInt64(value);

            Assert.AreEqual((ulong)172, result);
        }
    }
}
