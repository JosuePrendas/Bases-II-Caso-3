
using System;
using System.Threading;
using System.Diagnostics;

namespace ADONET
{
    static class Program
    {
        
        [STAThread]
        public static void Main(String[] args)
        {
            createThreads(10,"Pooling","Select * from Customer");
        }
		public static void createThreads(int pQuantity,string pTypeConnection,string pCommand)
        {
            Thread[] threads = new Thread[pQuantity];
            Stopwatch stopwatch = new Stopwatch();
            double total = 0;
            for(int threadIndex = 0; threadIndex<pQuantity; threadIndex++)
            {
                threads[threadIndex] = new Thread(new ParameterizedThreadStart(abrirConexion));
            }
            for (int threadIndex = 0; threadIndex < pQuantity; threadIndex++)
            {
                String[] parametters = {pTypeConnection,pCommand};
                threads[threadIndex].Start(parametters);
            }
            for (int threadIndex = 0; threadIndex < pQuantity; threadIndex++)
            {
                stopwatch.Start();
                threads[threadIndex].Join();
                stopwatch.Stop();
                total += (stopwatch.Elapsed.TotalMilliseconds) / 10;
                Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds / 10);
            }
            Console.WriteLine(pTypeConnection + " Average: " + (total / pQuantity));
        }
        public static void abrirConexion(object pParametters)
        {
            String[] parametters = (String[])pParametters;
            DataAccess.getInstance(parametters[0]).execQuery(parametters[1]);
        }
    }
}
