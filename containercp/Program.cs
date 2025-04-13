/* Copyright 2023 Hewlett Packard Enterprise Development LP.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License version 2.1
 */
using System;

namespace containercp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            if (args.Length == 0)
            {
                Console.WriteLine($"containercp v{version.ToString(3)}");
                Console.WriteLine("Usage:");
                Console.WriteLine("containercp <container-name>:<path-in-container> <path>");
                Console.WriteLine("containercp <path> <container-name>:<path-in-container>");
                Console.WriteLine("containercp <container-name>");
                Console.WriteLine("containercp -image-info <image-identifier>");
                Console.WriteLine("containercp -list containers");
                Console.WriteLine("containercp -list images");
                return;
            }

            if (args.Length == 1)
            {
                string containerName = args[0];
                InfoManager.ShowInfo(containerName);
            }
            else if (args.Length == 2)
            {
                if (args[0] == "-image-info")
                {
                    string imageIdentifier = args[1];
                    InfoManager.ShowImageInfo(imageIdentifier);
                }
                else if (args[0] == "-list")
                {
                    switch (args[1])
                    {
                        case "containers":
                            ListManager.ListContainers();
                            break;
                        case "images":
                            ListManager.ListImages();
                            break;
                        default:
                            Console.WriteLine("Invalid argument");
                            return;
                    }
                }
                else
                {
                    string source = args[0];
                    string destination = args[1];
                    CopyManager.ExecuteCopy(source, destination);
                }
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                HandleUnhandledException(ex);
            }
        }

        private static void HandleUnhandledException(Exception ex)
        {
            Console.WriteLine($"{ex.Message}, Stack trace: {ex.StackTrace}");
        }
    }
}