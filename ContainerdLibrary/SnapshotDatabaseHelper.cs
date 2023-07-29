/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using BoltDB;

namespace ContainerdLibrary
{
    internal class SnapshotDatabaseHelper
    {
        private const string V1BucketName = "v1";
        private const string SnapshotsBucketName = "snapshots";

        private string m_databasePath;

        public SnapshotDatabaseHelper(string databasePath)
        {
            m_databasePath = databasePath;
        }

        public ulong? GetSnapshotID(string snapshotName)
        {
            BoltDatabase containerdDatabase = BoltDatabaseHelper.Open(m_databasePath);
            Bucket v1Bucket = containerdDatabase.GetBucket(V1BucketName);
            Bucket snapshotsBucket = v1Bucket.GetBucket(SnapshotsBucketName);
            Bucket snapshotBucket = snapshotsBucket.GetBucket(snapshotName);
            if (snapshotBucket == null)
            {
                return null;
            }
            byte[] idBytes = (byte[])snapshotBucket.GetElementValueByKey("id");
            return VarIntConverter.ToUInt64(idBytes);
        }
    }
}
