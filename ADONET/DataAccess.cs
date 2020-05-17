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


        private DataAccess(String typeConnection)
        {
            if(typeConnection == "Pooling")
            {
                cache = false;
            }
            connectionString = ConfigurationManager.AppSettings[typeConnection];
            connectionStringW = ConfigurationManager.ConnectionStrings["PruebaCache"].ConnectionString;
            Console.WriteLine("Tipo de Conexion: " + typeConnection + "\nConnection String: " + connectionString);
            //SqlDependency.Start(connectionString);
            //execDependencyTest();
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
        public void cacheTest(string rawKey, object value)
        {
            SqlCacheDependency SqlDep = null;

            Cache pruebaCache = new Cache();
            object prueba = HttpRuntime.Cache.Get("rawKey");
            if (pruebaCache == null)
            {
                try
                {
                    SqlDep = new SqlCacheDependency("PruebaCache", "Persona");
                }

                catch (DatabaseNotEnabledForNotificationException exDBDis)
                {
                    try
                    {
                        SqlCacheDependencyAdmin.EnableNotifications("PruebaCache");
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
                        SqlCacheDependencyAdmin.EnableTableForNotifications("PruebaCache", "Persona");
                    }

                    catch (SqlException exc)
                    {
                        if (exc.Source != null)
                            Console.WriteLine("IOException source: {0}", exc.Source);
                    }
                }
                finally
                {
                    HttpRuntime.Cache.Insert("rawKey", prueba, SqlDep);
                    Console.WriteLine("El objeto fue creado explicitamente.");
                }
            }
            else
            {
                Console.WriteLine("El informacion fue obteniada del Cache.");
            }
        }
        void execDependencyTest()
        {
            // Assume connection is an open SqlConnection.
            SqlConnection connection1 = new SqlConnection(connectionString);
            // Create a new SqlCommand object.
            using (SqlCommand command = new SqlCommand(
                "SELECT persona_id, nombre, primerApellido, segundoApellido, edad FROM dbo.Persona", connection1))
            {
                // Create a dependency and associate it with the SqlCommand.
                SqlDependency dependency = new SqlDependency(command);

                // Subscribe to the SqlDependency event.
                dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);

                // Execute the command.
                connection1.Open();            
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Process the DataReader.
                    reader.Close();
                }
            }
            Console.WriteLine("Entre en execDependencyTest");
        }

        // Handler method
        void OnDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            // Handle the event (for example, invalidate this cache entry).
        }

        void Termination()
        {
            // Release the dependency.
            SqlDependency.Stop(connectionString);
        }
    }
}