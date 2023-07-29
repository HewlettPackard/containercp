/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using System.IO;

namespace ContainerdLibrary
{
    public class BlobReference
    {
        public string HashType { get; set; }

        public string Hash { get; set; }

        public long Size { get; set; }

        public DockerMediaType DockerMediaType { get; set; }

        /// <param name="digest">In the following 'hashType:hash' format</param>
        /// <exception cref="InvalidDataException"></exception>
        public BlobReference(string digest, long size, DockerMediaType dockerMediaType)
        {
            string[] digestComponents = digest.Split(':');
            if (digestComponents.Length != 2)
            {
                throw new InvalidDataException("Invalid digest format");
            }

            HashType = digestComponents[0];
            Hash = digestComponents[1];
            Size = size;
            DockerMediaType = dockerMediaType;
        }

        public BlobReference(string hashType, string hash, long size, DockerMediaType dockerMediaType)
        {
            HashType = hashType;
            Hash = hash;
            Size = size;
            DockerMediaType = dockerMediaType;
        }

        public override bool Equals(object obj)
        {
            if (obj is BlobReference blobReference)
            {
                return Equals(blobReference);
            }
            return false;
        }

        public bool Equals(BlobReference blobReference)
        {
            if (blobReference == null)
            {
                return false;
            }

            return (string.Equals( this.HashType, blobReference.HashType) &&
                    string.Equals(this.Hash, blobReference.Hash));
        }

        public override int GetHashCode()
        {
            if (this.Hash == null)
            {
                return 0;
            }

            return this.Hash.GetHashCode();
        }
    }
}
