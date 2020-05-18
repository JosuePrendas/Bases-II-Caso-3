using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls;

namespace ADONET
{
    class DataAccess
    {
        private static DataAccess access;
        private string connectionString;
        private string connectionStringW;
        private static Object lockObject = new object();
        private static bool cache = true;
        private static Cache pruebaCache;


        private DataAccess(String typeConnection)
        {
            if(typeConnection == "Pooling")
            {
                cache = false;
            }
            pruebaCache = new Cache();
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
        public void execQuery(String query)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, connection);
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                reader.Close();
                
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);

            }finally
            {
                if (connection.State == System.Data.ConnectionState.Open)
                    if (!cache)
                        cacheTest(query, lockObject);
                    connection.Close();               
            }
        }
        //****************************************************************
        public void cacheTest(string key, object value)
        {
            SqlCacheDependency SqlDep = null;            
            //object prueba1 = pruebaCache[key];
            object prueba2 = HttpRuntime.Cache.Get("Persona");
            if (prueba2 == null)
            {
                
                {
                    HttpRuntime.Cache.Insert("Persona", pruebaCache, SqlDep);
                    Console.WriteLine("El objeto fue creado explicitamente.");
                }
            }
            else
            {
                Console.WriteLine("El informacion fue obteniada del Cache.");
            }
        }
    }
}