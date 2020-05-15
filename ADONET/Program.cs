
using System;


namespace ADONET
{
    static class Program
    {
        
        [STAThread]
        public static void Main(String[] args)
        {
            DataAccess.getInstance("Caching").execQuery("Select * from Customer");
        }
    }
}
