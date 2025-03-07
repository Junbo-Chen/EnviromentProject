using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using EnviromentProject.Controllers;
using EnviromentProject.Model;
using EnviromentProject.Data;
using Object = EnviromentProject.Model.Object;

namespace EnviromentProject.Tests
{
    [TestClass]
    public class ObjectControllerTests
    {
        private ObjectController _controller;
        private Mock<ObjectRepository> _mockRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<ObjectRepository>();
            _controller = new ObjectController(_mockRepository.Object);
        }

        [TestMethod]
        public void GetObjectsByEnvironmentId_ReturnsOk_WhenObjectsExist()
        {
            // Arrange
            var environmentId = Guid.NewGuid();
            var mockObjects = new List<Object>
    {
        new Object { Id = Guid.NewGuid(), PrefabId = 1, PositionX = 10, PositionY = 20, ScaleX = 1, ScaleY = 1, RotationZ = 0, SortingLayer = 0, EnvironmentId = environmentId },
        new Object { Id = Guid.NewGuid(), PrefabId = 2, PositionX = 30, PositionY = 40, ScaleX = 1, ScaleY = 1, RotationZ = 0, SortingLayer = 1, EnvironmentId = environmentId }
    };
            _mockRepository.Setup(repo => repo.GetObjectsByEnvironmentId(environmentId)).Returns(mockObjects);

            // Act
            var result = _controller.GetObjectsByEnvironmentId(environmentId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult)); // Controleer of de result een OkObjectResult is
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(mockObjects, okResult.Value);
        }

        [TestMethod]
        public void GetObject_ReturnsNotFound_WhenObjectDoesNotExist()
        {
            // Arrange
            var objectId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetObjectById(objectId)).Returns((Object)null);

            // Act
            var result = _controller.GetObject(objectId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult)); // Controleer of het resultaat een NotFoundResult is
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }


        [TestMethod]
        public void CreateObject2D_ReturnsBadRequest_WhenObjectIsNull()
        {
            // Act
            var result = _controller.CreateObject2D(null);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Object2D is null or missing required fields.", badRequestResult.Value);
        }

        [TestMethod]
        public void CreateObject2D_ReturnsCreatedAtAction_WhenObjectIsCreated()
        {
            // Arrange
            var newObject = new Object
            {
                Id = Guid.NewGuid(),
                PrefabId = 1,
                PositionX = 10,
                PositionY = 20,
                ScaleX = 1,
                ScaleY = 1,
                RotationZ = 0,
                SortingLayer = 0,
                EnvironmentId = Guid.NewGuid()
            };
            _mockRepository.Setup(repo => repo.InsertObject(newObject)).Verifiable();

            // Act
            var result = _controller.CreateObject2D(newObject);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("GetObject", createdResult.ActionName);
            _mockRepository.Verify();
        }

        [TestMethod]
        public void Put_ReturnsNotFound_WhenObjectDoesNotExist()
        {
            // Arrange
            var objectId = Guid.NewGuid();
            var updatedObject = new Object
            {
                Id = objectId,
                PrefabId = 1,
                PositionX = 15,
                PositionY = 25,
                ScaleX = 2,
                ScaleY = 2,
                RotationZ = 0,
                SortingLayer = 1,
                EnvironmentId = Guid.NewGuid()
            };
            _mockRepository.Setup(repo => repo.GetObjectById(objectId)).Returns((Object)null);

            // Act
            var result = _controller.Put(objectId, updatedObject);

            // Assert
            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }

        [TestMethod]
        public void Delete_ReturnsNoContent_WhenObjectIsDeleted()
        {
            // Arrange
            var objectId = Guid.NewGuid();
            var mockObject = new Object
            {
                Id = objectId,
                PrefabId = 1,
                PositionX = 10,
                PositionY = 20,
                ScaleX = 1,
                ScaleY = 1,
                RotationZ = 0,
                SortingLayer = 0,
                EnvironmentId = Guid.NewGuid()
            };
            _mockRepository.Setup(repo => repo.GetObjectById(objectId)).Returns(mockObject);
            _mockRepository.Setup(repo => repo.DeleteObject(objectId)).Verifiable();

            // Act
            var result = _controller.Delete(objectId);

            // Assert
            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            _mockRepository.Verify();
        }
    }
}
