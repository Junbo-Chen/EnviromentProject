using System.Data;
using Microsoft.Data.SqlClient;  // <-- Gebruik deze in plaats van System.Data.SqlClient
using Microsoft.Extensions.Configuration;

namespace EnviromentProject.Data
{
    public class DbConnectionHelper
    {
        private readonly string _connectionString;

        public DbConnectionHelper(IConfiguration configuration)
        {
            _connectionString = configuration["DefaultConnection"];
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
