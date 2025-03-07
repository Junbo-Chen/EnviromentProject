using System;
using System.Collections.Generic;
using Moq;
using Xunit;
using Dapper;
using System.Data;
using System.Linq;
using EnviromentProject.Data;
using EnviromentProject.Model;
using Microsoft.Data.SqlClient;

namespace EnviromentProject.Tests
{
    public class EnvironmentRepositoryTests
    {
        private readonly Mock<IDbConnection> _mockConnection;
        private readonly EnvironmentRepository _environmentRepository;

        public EnvironmentRepositoryTests()
        {
            // Maak een mock voor de databaseverbinding
            _mockConnection = new Mock<IDbConnection>();
            _environmentRepository = new EnvironmentRepository("FakeConnectionString");
        }

        [Fact]
        public void GetAllEnvironments_ShouldReturnEnvironments()
        {
            // Arrange: Mock de retourwaarde van de databasequery
            var mockEnvironments = new List<Environment>
            {
                new Environment { Id = Guid.NewGuid(), Name = "Environment 1", MaxHeight = 10, MaxLength = 10, UserId = "user1" },
                new Environment { Id = Guid.NewGuid(), Name = "Environment 2", MaxHeight = 20, MaxLength = 20, UserId = "user2" }
            };

            _mockConnection.SetupDapperAsync(c => c.Query<Environment>(It.IsAny<string>(), null, null, null, null))
                           .Returns(mockEnvironments);

            // Act: Haal alle omgevingen op
            var environments = _environmentRepository.GetAllEnvironments();

            // Assert: Controleer of de juiste omgevingen zijn opgehaald
            Assert.NotNull(environments);
            Assert.Equal(2, environments.Count());
            Assert.Equal("Environment 1", environments.First().Name);
            Assert.Equal("Environment 2", environments.Last().Name);
        }

        [Fact]
        public void GetEnvironmentById_ShouldReturnEnvironment()
        {
            // Arrange: Mock de retourwaarde van de databasequery
            var mockEnvironment = new Environment { Id = Guid.NewGuid(), Name = "Test Environment", MaxHeight = 10, MaxLength = 10, UserId = "user1" };

            _mockConnection.SetupDapperAsync(c => c.QueryFirstOrDefault<Environment>(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                           .Returns(mockEnvironment);

            // Act: Haal de omgeving op op basis van ID
            var environmentId = mockEnvironment.Id;
            var environment = _environmentRepository.GetEnvironmentById(environmentId);

            // Assert: Controleer of de juiste omgeving is opgehaald
            Assert.NotNull(environment);
            Assert.Equal(mockEnvironment.Id, environment.Id);
            Assert.Equal("Test Environment", environment.Name);
        }

        [Fact]
        public void InsertEnvironment_ShouldInsertEnvironment()
        {
            // Arrange: Maak een mock voor de environment
            var environment = new Environment
            {
                Id = Guid.NewGuid(),
                Name = "New Environment",
                MaxHeight = 15,
                MaxLength = 15,
                UserId = "user1"
            };

            // Mock de Execute-method van Dapper
            _mockConnection.SetupDapperAsync(c => c.Execute(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                           .Returns(1); // Simuleer dat het invoegen succesvol is

            // Act: Roep de Insert-methode aan
            _environmentRepository.InsertEnvironment(environment);

            // Assert: Controleer dat de Execute-methode werd aangeroepen
            _mockConnection.Verify(m => m.Execute(It.IsAny<string>(), It.IsAny<object>(), null, null, null), Times.Once);
        }

        [Fact]
        public void DeleteEnvironment_ShouldDeleteEnvironment()
        {
            // Arrange: Maak een mock voor de ID
            var environmentId = Guid.NewGuid();

            // Mock de Execute-methode van Dapper
            _mockConnection.SetupDapperAsync(c => c.Execute(It.IsAny<string>(), It.IsAny<object>(), null, null, null))
                           .Returns(1); // Simuleer dat het verwijderen succesvol is

            // Act: Roep de Delete-methode aan
            _environmentRepository.DeleteEnvironment(environmentId);

            // Assert: Controleer dat de Execute-methode werd aangeroepen
            _mockConnection.Verify(m => m.Execute(It.IsAny<string>(), It.IsAny<object>(), null, null, null), Times.Once);
        }
    }

    // Dapper mock extensie voor asynchrone methodes
    public static class DapperMockExtensions
    {
        public static ISetup<T, TResult> SetupDapperAsync<T, TResult>(this Mock<T> mock, Expression<Func<T, TResult>> expression) where T : class
        {
            return mock.Setup(expression);
        }
    }
}
