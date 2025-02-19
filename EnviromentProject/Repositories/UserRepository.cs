using System.Data;
using Dapper;
using EnviromentProject.Model;
using BCrypt.Net;
using Microsoft.CodeAnalysis.Scripting;

namespace EnviromentProject.Data
{
    public class UserRepository
    {
        private readonly DbConnectionHelper _dbHelper;

        public UserRepository(DbConnectionHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public User GetUserByUsername(string username)
        {
            using var connection = _dbHelper.CreateConnection();
            return connection.QueryFirstOrDefault<User>(
                "SELECT * FROM [User] WHERE Username = @Username", new { Username = username });
        }

        public void InsertUser(User user)
        {
            using var connection = _dbHelper.CreateConnection();

            // Maak een nieuwe GUID als deze nog leeg is
            user.Id = user.Id == Guid.Empty ? Guid.NewGuid() : user.Id;

            // Hash het wachtwoord met BCrypt
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            string sql = "INSERT INTO [User] (Id, Username, Password) VALUES (@Id, @Username, @Password)";
            connection.Execute(sql, user);
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);
        }
        public User AuthenticateUser(string username, string enteredPassword)
        {
            var user = GetUserByUsername(username);

            if (user != null && VerifyPassword(enteredPassword, user.Password))
            {
                return user;
            }

            return null; 
        }

        public void DeleteUser(string username)
        {
            using var connection = _dbHelper.CreateConnection();
            connection.Execute("DELETE FROM [User] WHERE Username = @Username", new { Username = username });
        }

        public IEnumerable<User> GetAllUsers()
        {
            using var connection = _dbHelper.CreateConnection();
            return connection.Query<User>("SELECT * FROM [User]");
        }
    }
}
