using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace HashSumOfFiles
{
    class Program
    {
        private static System.Diagnostics.Stopwatch stopWatch;
        private static int countHashCalc = 5;
        private static MyConcurrentQueue<string> filenames = new MyConcurrentQueue<string>();
        private static MyConcurrentQueue<FileHashSum> fileHashSums = new MyConcurrentQueue<FileHashSum>(countHashCalc);

        static void Main(string[] args)
        {
            //Console.WriteLine("Enter directory:/n");
            //String directory = Console.ReadLine();
            string directory = @"C:\ProgramData";
            Console.WriteLine("Start in " + directory);
            DateTime start = DateTime.Now;            

            FileEnumenator fileEnumenator = new FileEnumenator(filenames);
            FileHashSumCalculator hashCalculator = new FileHashSumCalculator(filenames, fileHashSums);
            FileHashSumDAO dbWriter = new FileHashSumDAO(fileHashSums);

            stopWatch = new System.Diagnostics.Stopwatch();
            stopWatch.Start();
            Thread threadEnumenator = new Thread(new ParameterizedThreadStart(fileEnumenator.GetFilePaths));
            threadEnumenator.Start(directory);
            ThreadArray threadHashWorkers = new ThreadArray(countHashCalc, hashCalculator.CalculateHashSums);            
            Thread threadWriter = new Thread(dbWriter.WriteToDB);
            threadWriter.Start();
            threadHashWorkers.Start();

            threadEnumenator.Join();
            threadHashWorkers.Join();
            threadWriter.Join();

            TimeSpan elapsedTime = stopWatch.Elapsed;
            Console.WriteLine("{0} minutes, {1} seconds", elapsedTime.Minutes, elapsedTime.Seconds);
            Console.ReadKey();
            NLog.LogManager.Shutdown();
        }
    }

    public class ThreadArray
    {
        private Thread[] threads;

        public ThreadArray(int count, ThreadStart method)
        {
            threads = new Thread[count];
            for (int i = 0; i < count; i++)
                threads[i] = new Thread(method);
        }

        public void Start()
        {
            for (int i = 0; i < threads.Length; i++)
                threads[i].Start();
        }

        public void Join()
        {
            for (int i = 0; i < threads.Length; i++)
                threads[i].Join();
        }
    }
}
