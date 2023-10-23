using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LoginWebApp.Infrastructure.DataHelpers
{
    public class DBHelper
    {
        /*private readonly string _connectionString;*/
        public string ConnectionString { get; set; }
        public DBHelper(IConfiguration configuration)
        {
            ConnectionString = configuration.GetConnectionString("DBConnectionStr");
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
