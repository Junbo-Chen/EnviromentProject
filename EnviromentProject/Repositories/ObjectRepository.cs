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

        // 🔹 Haal alle objecten op
        public IEnumerable<Object> GetAllObjects()
        {
            using var connection = _dbHelper.CreateConnection();
            return connection.Query<Object>("SELECT * FROM Object");
        }

        // 🔹 Haal een specifiek object op via ID
        public Object GetObjectById(Guid id)
        {
            using var connection = _dbHelper.CreateConnection();
            return connection.QueryFirstOrDefault<Object>(
                "SELECT * FROM Object WHERE Id = @Id", new { Id = id });
        }

        // 🔹 Voeg een nieuw object toe
        public void InsertObject(Object obj)
        {
            using var connection = _dbHelper.CreateConnection();
            obj.Id = Guid.NewGuid();
            string sql = @"INSERT INTO Object 
                          (PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, EnvironmentId)
                          VALUES 
                          (@PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer, @EnvironmentId)";
            connection.Execute(sql, obj);
        }

        // 🔹 Verwijder een object op basis van ID
        public void DeleteObject(Guid id)
        {
            using var connection = _dbHelper.CreateConnection();
            connection.Execute("DELETE FROM Object WHERE Id = @Id", new { Id = id });
        }
    }
}

