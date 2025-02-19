using System.Collections.Generic;
using System.Data;
using Dapper;
using EnviromentProject.Model;
using Environment = EnviromentProject.Model.Environment;

namespace EnviromentProject.Data
{
    public class EnvironmentRepository
    {
        private readonly DbConnectionHelper _dbHelper;

        public EnvironmentRepository(DbConnectionHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IEnumerable<Environment> GetAllEnvironments()
        {
            using var connection = _dbHelper.CreateConnection();
            return connection.Query<Environment>("SELECT * FROM Environment");
        }

        public Environment GetEnvironmentById(Guid id)
        {
            using var connection = _dbHelper.CreateConnection();
            return connection.QueryFirstOrDefault<Environment>(
                "SELECT * FROM Environment WHERE Id = @Id", new { Id = id });
        }

        public void InsertEnvironment(Environment environment)
        {
            using var connection = _dbHelper.CreateConnection();
            environment.Id = Guid.NewGuid();
            string sql = "INSERT INTO Environment (Name, MaxHeight, MaxLength, UserId) " +
                         "VALUES (@Name, @MaxHeight, @MaxLength, @UserId)";
            connection.Execute(sql, environment);
        }

        public void UpdateEnvironment(Environment environment)
        {
            using var connection = _dbHelper.CreateConnection();
            string sql = @"UPDATE Environment 
                   SET Name = @Name, MaxHeight = @MaxHeight, MaxLength = @MaxLength,
                   WHERE Id = @Id";
            connection.Execute(sql, environment);
        }

        // 🔹 Verwijder een environment op basis van ID
        public void DeleteEnvironment(Guid id)
        {
            using var connection = _dbHelper.CreateConnection();
            connection.Execute("DELETE FROM Environment WHERE Id = @Id", new { Id = id });
        }
    }
}
