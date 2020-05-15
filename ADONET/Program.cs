
using System;
using System.Threading;

namespace ADONET
{
    static class Program
    {
        
        [STAThread]
        public static void Main(String[] args)
        {
            createThreads(50,"Pooling","Select * from Customer");
        }
		public static void createThreads(int pQuantity,string pTypeConnection,string pCommand)
        {
            Thread[] threads = new Thread[pQuantity];
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
                threads[threadIndex].Join();
            }
        }
        public static void abrirConexion(object pParametters)
        {
            String[] parametters = (String[])pParametters;
            DataAccess.getInstance(parametters[0]).execQuery(parametters[1]);
        }
    }
}
