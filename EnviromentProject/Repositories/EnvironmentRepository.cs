using System.Collections.Generic;
using System.Data;
using Dapper;
using EnviromentProject.Model;
using Microsoft.Data.SqlClient;
using Environment = EnviromentProject.Model.Environment;

namespace EnviromentProject.Data
{
    public class EnvironmentRepository
    {
        private readonly string _connectionString;

        public EnvironmentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Environment> GetAllEnvironments()
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.Query<Environment>("SELECT * FROM Environment");
        }

        public Environment GetEnvironmentById(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.QueryFirstOrDefault<Environment>(
                "SELECT * FROM Environment WHERE Id = @Id", new { Id = id });
        }

        public void InsertEnvironment(Environment environment)
        {
            using var connection = new SqlConnection(_connectionString);
            environment.Id = Guid.NewGuid();

            string sql = "INSERT INTO Environment (Id, Name, MaxHeight, MaxLength, UserId) " +
                         "VALUES (@Id, @Name, @MaxHeight, @MaxLength, @UserId)";

            connection.Execute(sql, environment);
        }
        public IEnumerable<Environment> GetEnvironmentsByUserId(string userId)
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.Query<Environment>(
                "SELECT * FROM Environment WHERE UserId = @UserId", new { UserId = userId });
        }

        public void UpdateEnvironment(Environment environment)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"UPDATE Environment 
                   SET Name = @Name, MaxHeight = @MaxHeight, MaxLength = @MaxLength,
                   WHERE Id = @Id";
            connection.Execute(sql, environment);
        }

        // 🔹 Verwijder een environment op basis van ID
        public void DeleteEnvironment(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Execute("DELETE FROM Environment WHERE Id = @Id", new { Id = id });
        }
    }
}
