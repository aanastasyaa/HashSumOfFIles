﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace HashSumOfFiles
{
    /*
        Предоставляет метод для расчета хэш-суммы файлов по алгоритму MD5        
    */
    public class FileHashSumCalculator
    {
        // очередь, из которой берутся пути к файлам
        private MyConcurrentQueue<string> files;
        // очередь, в которую сохраняются вычисленные хэш-суммы
        private MyConcurrentQueue<FileHashSum> fileHashSums;

        public FileHashSumCalculator(MyConcurrentQueue<string> queue, MyConcurrentQueue<FileHashSum> fileHashSums)
        {
            this.files = queue;
            this.fileHashSums = fileHashSums;
        }

        public void CalculateHashSums()
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                while (!files.IsCompleted()) // пока первый поток не закончит добавлять элементы и очередь не пуста
                {
                    string filename = files.Dequeue();
                    if (filename == null)
                        break;
                    FileHashSum file = new FileHashSum(filename);
                    BufferedStream fileStream = null;
                    try
                    {
                        fileStream = new BufferedStream(File.OpenRead(file.Filename), 2000000);
                        byte[] hashSum = md5.ComputeHash(fileStream);
                        file.HashSum = BitConverter.ToString(hashSum).Replace("-", string.Empty);

                    }
                    catch (IOException e)
                    {
                        file.Error = e.Message;
                    }
                    catch (UnauthorizedAccessException e)
                    {
                        file.Error = e.Message;
                    }
                    finally
                    {
                        if (fileStream != null)
                            fileStream.Close();
                        fileHashSums.Enqueue(file);
                    }
                }
                fileHashSums.CompleteEnqueue();
            }
        }
    }
}
