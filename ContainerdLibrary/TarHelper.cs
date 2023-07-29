/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using System.IO;
using System.Text;
using Utilities;

namespace ContainerdLibrary
{
    internal static class TarHelper
    {
        public static bool IsFileExistsInTarGzip(string archivePath, string pathInArchive)
        {
            FileStream archiveStream = File.OpenRead(archivePath);
            using (archiveStream)
            {
                using (GZipInputStream gZipStream = new GZipInputStream(archiveStream))
                {
                    return IsFileExistsInTarStream(gZipStream, pathInArchive);
                }
            }
        }

        public static bool IsFileExistsInTar(string archivePath, string pathInArchive)
        {
            FileStream archiveStream = File.OpenRead(archivePath);
            using (archiveStream)
            {
                return IsFileExistsInTarStream(archiveStream, pathInArchive);
            }
        }

        internal static bool IsFileExistsInTarStream(Stream archiveStream, string pathInArchive)
        {
            if (pathInArchive.StartsWith("/"))
            {
                pathInArchive = pathInArchive.Substring(1);
            }

            using (TarInputStream tarStream = new TarInputStream(archiveStream, Encoding.UTF8))
            {
                TarEntry entry = tarStream.GetNextEntry();
                while (entry != null)
                {
                    if (entry.Name == pathInArchive)
                    {
                        return true;
                    }
                    entry = tarStream.GetNextEntry();
                }
            }

            return false;
        }

        public static bool ExtractFileFromTarGzip(string archivePath, string pathInArchive, string outputPath)
        {
            FileStream archiveStream = File.OpenRead(archivePath);
            using (archiveStream)
            {
                using (GZipInputStream gZipStream = new GZipInputStream(archiveStream))
                {
                    return ExtractFileFromTarStream(gZipStream, pathInArchive, outputPath);
                }
            }
        }

        public static bool ExtractFileFromTar(string archivePath, string pathInArchive, string outputPath)
        {
            FileStream archiveStream = File.OpenRead(archivePath);
            using (archiveStream)
            {
                return ExtractFileFromTarStream(archiveStream, pathInArchive, outputPath);
            }
        }

        internal static bool ExtractFileFromTarStream(Stream archiveStream, string pathInArchive, string outputPath)
        {
            if (pathInArchive.StartsWith("/"))
            {
                pathInArchive = pathInArchive.Substring(1);
            }

            using (TarInputStream tarStream = new TarInputStream(archiveStream, Encoding.UTF8))
            {
                TarEntry entry = tarStream.GetNextEntry();
                while (entry != null)
                {
                    if (entry.Name == pathInArchive)
                    {
                        FileStream outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
                        using (outputStream)
                        {
                            tarStream.CopyEntryContents(outputStream);
                        }
                        return true;
                    }
                    entry = tarStream.GetNextEntry();
                }
            }

            return false;
        }

        /// <summary>
        /// If the file already exists in the archive, it will be replaced.
        /// </summary>
        public static void PutFileInTarGzipArchive(string archivePath, string pathInArchive, Stream fileStream, string outputPath)
        {
            FileStream sourceArchiveStream = File.OpenRead(archivePath);
            FileStream destinationArchiveStream = File.OpenWrite(outputPath);
            using (sourceArchiveStream)
            {
                using (destinationArchiveStream)
                {
                    using (GZipInputStream gZipStream = new GZipInputStream(sourceArchiveStream))
                    {
                        using (GZipOutputStream gZipOutputStream = new GZipOutputStream(destinationArchiveStream))
                        {
                            PutFileInTarArchive(gZipStream, pathInArchive, fileStream, gZipOutputStream);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// If the file already exists in the archive, it will be replaced.
        /// </summary>
        public static void PutFileInTarArchive(string archivePath, string pathInArchive, Stream fileStream, string outputPath)
        {
            FileStream sourceArchiveStream = File.OpenRead(archivePath);
            FileStream destinationArchiveStream = File.OpenWrite(outputPath);
            using (sourceArchiveStream)
            {
                using (destinationArchiveStream)
                {
                    PutFileInTarArchive(sourceArchiveStream, pathInArchive, fileStream, destinationArchiveStream);
                }
            }
        }

        internal static void PutFileInTarArchive(Stream archiveStream, string pathInArchive, Stream fileStream, Stream outputStream)
        {
            if (pathInArchive.StartsWith("/"))
            {
                pathInArchive = pathInArchive.Substring(1);
            }

            using (TarInputStream tarStream = new TarInputStream(archiveStream, Encoding.UTF8))
            {
                using (TarOutputStream tarOutputStream = new TarOutputStream(outputStream, Encoding.UTF8))
                {
                    TarEntry entry = tarStream.GetNextEntry();
                    while (entry != null)
                    {
                        if (entry.Name != pathInArchive)
                        {
                            tarOutputStream.PutNextEntry(entry);
                            tarStream.CopyEntryContents(tarOutputStream);
                            tarOutputStream.CloseEntry();
                        }

                        entry = tarStream.GetNextEntry();
                    }

                    TarEntry newFileEntry = TarEntry.CreateTarEntry(pathInArchive);
                    newFileEntry.Size = fileStream.Length;
                    tarOutputStream.PutNextEntry(newFileEntry);
                    ByteUtils.CopyStream(fileStream, tarOutputStream);
                    tarOutputStream.CloseEntry();
                }
            }
        }
    }
}
