using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Approva.QA.Common;

namespace SQLOps
{
    public class SQLOperations
    {
        public static void _executeSQLQueries(string _commandText, string _connectionString)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(_commandText, connection);
                try
                {
                    command.Connection.Open();
                }
                catch (Exception ex)
                {

                    Logger.WriteMessage("Problem while connection to database, Please check Connection String at Appsecurity.xml");
                    Logger.WriteError(ex.Message);
                }
                try
                {
                    Logger.WriteMessage(_commandText);
                    command.ExecuteNonQuery();
                    Logger.WriteMessage("command executed successfully");
                }
                catch (SqlException ex)
                {
                    if(ex.Number== 15530)                    
                        Logger.WriteMessage("command executed successfully");                        
                    else
                    {
                        Logger.WriteMessage("Query execution failed");
                        Logger.WriteError(" "+ex.Number +":" + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Logger.WriteMessage("Query execution failed");
                    Logger.WriteError(ex.Message);
                }
            }
        }

        public static void _revokeGuestUserfromDB(String dbName, String command, string _connectionString)
        {
            string _command = "USE [" + dbName + "];" + command + "".ToString();
            _executeSQLQueries(_command, _connectionString);
        }
    }
}
