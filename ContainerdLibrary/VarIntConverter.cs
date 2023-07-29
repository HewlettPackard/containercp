/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using System;

namespace ContainerdLibrary
{
    // This file implements "varint" encoding of 64-bit integers.
    // The encoding is:
    // - unsigned integers are serialized 7 bits at a time, starting with the
    //   least significant bits
    // - the most significant bit (msb) in each output byte indicates if there
    //   is a continuation byte (msb = 1)
    // - signed integers are mapped to unsigned integers using "zig-zag"
    //   encoding: Positive values x are written as 2*x + 0, negative values
    //   are written as 2*(^x) + 1; that is, negative numbers are complemented
    //   and whether to complement is encoded in bit 0.
    public static class VarIntConverter
    {
        public static long ToInt64(byte[] value)
        {
            ulong result = ToUInt64(value);
            
            if (result % 2 == 0) // Positive number
            {
                return (long)(result >> 1);
            }
            else // Negative number
            {
                //result = ~result;
                throw new NotImplementedException();
            }
        }

        public static ulong ToUInt64(byte[] value)
        {
            int index = 0;
            ulong result = 0;
            while (true)
            {
                int numberOfBitsToShift = (index * 7);
                bool hasMoreBytes = (value[index] >> 7) == 1;
                result = result | ((ulong)(value[index] & 0x7F) << numberOfBitsToShift);
                if (!hasMoreBytes)
                {
                    break;
                }
                index++;
            }

            return result;
        }
    }
}
