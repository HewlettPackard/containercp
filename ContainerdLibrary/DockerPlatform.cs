/* Copyright 2024 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
namespace ContainerdLibrary
{
    public class DockerPlatform
    {
        public string Architecture { get; set; }

        public string OS { get; set; }

        public DockerPlatform(string architecture, string os)
        {
            Architecture = architecture;
            OS = os;
        }

        public override bool Equals(object obj)
        {
            if (obj is BlobReference blobReference)
            {
                return Equals(blobReference);
            }
            return false;
        }

        public bool Equals(DockerPlatform dockerPlatform)
        {
            if (dockerPlatform == null)
            {
                return false;
            }

            return (string.Equals(this.Architecture, dockerPlatform.Architecture) &&
                    string.Equals(this.OS, dockerPlatform.OS));
        }

        public override int GetHashCode()
        {
            if (this.Architecture == null)
            {
                return 0;
            }

            return this.Architecture.GetHashCode();
        }
    }
}
