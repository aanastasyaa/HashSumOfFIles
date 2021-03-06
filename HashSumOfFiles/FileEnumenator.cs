﻿using System;
using System.Collections.Generic;
using System.IO;
using NLog;

namespace HashSumOfFiles
{
    /*
        Предоставляет метод для поиска файлов в каталогах
        Пути к найденным файлам помещаются в очередь
    */
    public class FileEnumenator
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // очередь, в которую сохраняются пути до найденных файлов
        private MyConcurrentQueue<string> files;

        public FileEnumenator(MyConcurrentQueue<string> queue)
        {
            this.files = queue;
        }

        public void GetFilePaths(object path)
        {
            string startingDirectory = path as string;
            if (startingDirectory == null)
                return;

            if (File.Exists(startingDirectory))
            {
                files.Enqueue(startingDirectory);
            }
            else if (Directory.Exists(startingDirectory))
            {
                ProcessDirectory(startingDirectory);
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory", startingDirectory);
                logger.Error("{0} is not a valid file or directory", startingDirectory);
            }

            files.CompleteEnqueue();
            Console.WriteLine("All files were found in " + startingDirectory);
        }

        private void ProcessDirectory(string directory)
        {
            Queue<string> subDirs = new Queue<string>();
            subDirs.Enqueue(directory);
            while (subDirs.Count > 0)
            {
                string currentDir = subDirs.Dequeue();
                try
                {
                    foreach (string file in Directory.GetFiles(currentDir))
                    {
                        files.Enqueue(file);
                        logger.Info("Found file {0}", file);
                    }
                    foreach (string subdirectory in Directory.GetDirectories(currentDir))
                        subDirs.Enqueue(subdirectory);
                }
                catch (UnauthorizedAccessException e)
                {
                    files.Enqueue(currentDir);
                    logger.Error(e,"Can`t get access to {0}", currentDir);
                }                
            }
        }
    }
}
