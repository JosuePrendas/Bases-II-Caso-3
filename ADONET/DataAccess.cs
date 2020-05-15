using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }

   

}
