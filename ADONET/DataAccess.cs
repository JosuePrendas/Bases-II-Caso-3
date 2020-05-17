using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace ADONET
{
    class DataAccess
    {
        private static DataAccess access;
        private string connectionString;
        private static Object lockObject = new object();
        private static Cache cache = null;


        private DataAccess(String typeConnection)
        {
            if (typeConnection == "Cache")
                cache = new Cache();
            connectionString = ConfigurationManager.AppSettings[typeConnection];
            Console.WriteLine("Tipo de Conexion: " + typeConnection + "\nConnection String: " + connectionString);

        }

        public static DataAccess getInstance(String typeConnection)
        {
            if (access != null)
                return access;
            lock (lockObject)
            {
                access = access != null ? access : new DataAccess(typeConnection);
                return access;
            }
        }

        public DataSet execQuery(String query, DataSet resultBuffer)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                SqlDataAdapter da = new SqlDataAdapter(query, connection);
                da.SelectCommand.CommandType = CommandType.Text;
                da.Fill(resultBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return resultBuffer;
        }
        public void execQuery(String query)
        {
            DataSet ds = new DataSet();
            if (cache != null)
            {
                lock (lockObject)
                {
                    if (cache["Customers"] != null)
                    {
                        ds = (DataSet)cache["Customers"];
                        Console.WriteLine("Datos recogidos de cache");
                    }
                    else
                    {
                        ds = execQuery(query,ds);
                        cache.Insert("Customers",ds);
                        Console.WriteLine("Datos insertados en cache");
                    }
                }
            }
            else
            {
                ds = execQuery(query, ds);
            }
        }
    }
}
