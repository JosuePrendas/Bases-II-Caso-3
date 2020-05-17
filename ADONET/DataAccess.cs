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
        private static Object lockObject = new object();
        private static bool cache = true;


        private DataAccess(String typeConnection)
        {
            if(typeConnection == "Pooling")
            {
                cache = false;
            }
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
                        cacheTest();
                    connection.Close();
                
            }
        }
        //****************************************************************
        public void cacheTest()
        {
            SqlCacheDependency SqlDep = null;
            var string1 = "Caching";
            var prueba = HttpRuntime.Cache[string1];
            if (prueba == null)
            {
                try
                {
                    SqlDep = new SqlCacheDependency("BD_Granel", "Persona");
                }

                catch (DatabaseNotEnabledForNotificationException exDBDis)
                {
                    try
                    {
                        SqlCacheDependencyAdmin.EnableNotifications("BD_Granel");
                        //ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString
                        //connectionString
                    }
                    catch (UnauthorizedAccessException exPerm)
                    {
                        if (exPerm.Source != null)
                            Console.WriteLine("IOException source: {0}", exPerm.Source);
                    }
                }
                catch (TableNotEnabledForNotificationException exTabDis)
                {
                    try
                    {
                        SqlCacheDependencyAdmin.EnableTableForNotifications("BD_Granel", "Persona");
                    }

                    catch (SqlException exc)
                    {
                        if (exc.Source != null)
                            Console.WriteLine("IOException source: {0}", exc.Source);
                    }
                }
                finally
                {
                    HttpRuntime.Cache.Insert(string1, lockObject, SqlDep);
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