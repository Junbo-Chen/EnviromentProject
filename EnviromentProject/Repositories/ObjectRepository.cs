using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using EnviromentProject.Model;
using Object = EnviromentProject.Model.Object;

namespace EnviromentProject.Data
{
    public class ObjectRepository
    {
        private readonly DbConnectionHelper _dbHelper;

        public ObjectRepository(DbConnectionHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public IEnumerable<Object> GetObjectsByEnvironmentId(Guid environmentId)
        {
            using var connection = _dbHelper.CreateConnection();
            return connection.Query<Object>(
                "SELECT * FROM Object WHERE EnvironmentId = @EnvironmentId", new { EnvironmentId = environmentId });
        }

        public Object GetObjectById(Guid id)
        {
            using var connection = _dbHelper.CreateConnection();
            return connection.QueryFirstOrDefault<Object>(
                "SELECT * FROM Object WHERE Id = @Id", new { Id = id });
        }

        public void InsertObject(Object obj)
        {
            using var connection = _dbHelper.CreateConnection();
            obj.Id = Guid.NewGuid();  // Genereer een nieuwe ID

            string sql = "INSERT INTO Object (Id, PrefabId, EnvironmentId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer) " +
                         "VALUES (@Id, @PrefabId, @EnvironmentId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer)";

            connection.Execute(sql, obj);
        }

        public void UpdateObject(Object obj)
        {
            using var connection = _dbHelper.CreateConnection();
            string sql = @"UPDATE Object 
                           SET PrefabId = @PrefabId, PositionX = @PositionX, PositionY = @PositionY,
                               ScaleX = @ScaleX, ScaleY = @ScaleY, RotationZ = @RotationZ, SortingLayer = @SortingLayer
                           WHERE Id = @Id";
            connection.Execute(sql, obj);
        }

        public void DeleteObject(Guid id)
        {
            using var connection = _dbHelper.CreateConnection();
            connection.Execute("DELETE FROM Object WHERE Id = @Id", new { Id = id });
        }
    }
}


