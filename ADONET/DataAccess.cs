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


        private DataAccess(String typeConnection)
        {
            connectionString = ConfigurationManager.AppSettings[typeConnection];
            Console.WriteLine("Tipo de Conexion: " + typeConnection+"\nConnection String: "+connectionString);
            SqlDependency.Start(connectionString);
            execDependencyTest();
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
                if(connection.State == System.Data.ConnectionState.Open)
                    connection.Close();
                
            }


        }
        //****************************************************************
        //public T GetOrSetCache<T>(string key, T obj, int cacheTime) where T : class, new()
        //{
        //    System.Web.Caching.Cache cacheContainer = HttpRuntime.Cache;
        //    T cacheObj = cacheContainer.Get(key) as T;

        //    if (cacheObj == null)
        //    {
        //        cacheContainer.Insert(key,
        //            obj,
        //            null,
        //            DateTime.Now.AddMinutes(cacheTime),
        //            System.Web.Caching.Cache.NoSlidingExpiration);
        //        cacheObj = obj;
        //    }

        //    return cacheObj;
        //}

        public void execDependencyTest()
        {
            // Assume connection is an open SqlConnection.
            SqlConnection connection = new SqlConnection(connectionString);
            // Create a new SqlCommand object.
            using (SqlCommand command = new SqlCommand(
                "SELECT nombre, primer_apellido, segundo_apellido FROM dbo.Personas WHERE persona_id = 1",
                connection))
            {
                // Create a dependency and associate it with the SqlCommand.
                SqlDependency dependency = new SqlDependency(command);

                // Subscribe to the SqlDependency event.
                dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);
                Console.WriteLine(dependency);
            }
            Console.WriteLine("Aqui estoy");
        }

        // Handler method
        void OnDependencyChange(object sender, SqlNotificationEventArgs e)
        {
            // Handle the event (for example, invalidate this cache entry).
            Console.WriteLine(e);
        }

        void Termination()
        {
            // Release the dependency.
            SqlDependency.Stop(connectionString);
        }
    }
}