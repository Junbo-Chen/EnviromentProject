using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using EnviromentProject.Model;
using Microsoft.Data.SqlClient;
using Object = EnviromentProject.Model.Object;

namespace EnviromentProject.Data
{
    public class ObjectRepository
    {
        private readonly string _connectionString;

        public ObjectRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<Object> GetObjectsByEnvironmentId(Guid environmentId)
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.Query<Object>(
                "SELECT * FROM Object WHERE EnvironmentId = @EnvironmentId", new { EnvironmentId = environmentId });
        }

        public Object GetObjectById(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            return connection.QueryFirstOrDefault<Object>(
                "SELECT * FROM Object WHERE Id = @Id", new { Id = id });
        }

        public void InsertObject(Object obj)
        {
            using var connection = new SqlConnection(_connectionString);
            obj.Id = Guid.NewGuid();  // Genereer een nieuwe ID

            string sql = "INSERT INTO Object (Id, PrefabId, EnvironmentId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer) " +
                         "VALUES (@Id, @PrefabId, @EnvironmentId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer)";

            connection.Execute(sql, obj);
        }

        public void UpdateObject(Object obj)
        {
            using var connection = new SqlConnection(_connectionString);
            string sql = @"UPDATE Object 
                           SET PrefabId = @PrefabId, PositionX = @PositionX, PositionY = @PositionY,
                               ScaleX = @ScaleX, ScaleY = @ScaleY, RotationZ = @RotationZ, SortingLayer = @SortingLayer
                           WHERE Id = @Id";
            connection.Execute(sql, obj);
        }

        public void DeleteObject(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Execute("DELETE FROM Object WHERE Id = @Id", new { Id = id });
        }
    }
}


