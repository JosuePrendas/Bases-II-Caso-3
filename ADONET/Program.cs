
using System;

namespace ADONET
{
    static class Program
    {
        
        [STAThread]
        public static void Main(String[] args)
        {
            DataAccess.getInstance("Pooling").execQuery("Select * from Customer");
        }
    }
}
