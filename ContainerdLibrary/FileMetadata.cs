/* Copyright 2024 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
namespace ContainerdLibrary
{
    public class FileMetadata
    {
        public int UserID { get; private set; }

        public int GroupID { get; private set; }

        /// <summary>
        /// Mode contains both the file type and its permissions
        /// </summary>
        public int Mode { get; private set; }

        public FileMetadata(int userID, int groupID, int mode)
        {
            UserID = userID;
            GroupID = groupID;
            Mode = mode;
        }
    }
}
