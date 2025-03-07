using Xunit;
using Moq;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using EnviromentProject.Controllers;
using EnviromentProject.Data;
using EnviromentProject.Model;
using Environment = EnviromentProject.Model.Environment;
using Object = EnviromentProject.Model.Object;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using Microsoft.Extensions.Hosting;
using EnviromentProject.Interface;

namespace EnviromentProject.Tests
{

    [TestClass]
    public class ObjectControllerTests
    {
        private ObjectController _controller;
        private Mock<IObjectRepository> _mockRepository;

        [TestInitialize]
        public void Setup()
        {
            _mockRepository = new Mock<IObjectRepository>();
            _controller = new ObjectController(_mockRepository.Object);
        }

        [TestMethod]
        public void GetObjectsByEnvironmentId_ReturnsOk_WhenObjectsExist()
        {
            var environmentId = Guid.NewGuid();
            var mockObjects = new List<Object>
            {
                new Object { Id = Guid.NewGuid(), PrefabId = 1, PositionX = 10, PositionY = 20, ScaleX = 1, ScaleY = 1, RotationZ = 0, SortingLayer = 0, EnvironmentId = environmentId },
                new Object { Id = Guid.NewGuid(), PrefabId = 2, PositionX = 30, PositionY = 40, ScaleX = 1, ScaleY = 1, RotationZ = 0, SortingLayer = 1, EnvironmentId = environmentId }
            };
            _mockRepository.Setup(repo => repo.GetObjectsByEnvironmentId(environmentId)).Returns(mockObjects);

            var result = _controller.GetObjectsByEnvironmentId(environmentId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));

            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            var okValue = okResult.Value as List<Object>;
            Assert.IsNotNull(okValue);
            Assert.AreEqual(mockObjects.Count, okValue.Count);
            Assert.IsTrue(mockObjects.SequenceEqual(okValue));
        }

        [TestMethod]
        public void GetObject_ReturnsNotFound_WhenObjectDoesNotExist()
        {
            var objectId = Guid.NewGuid();
            _mockRepository.Setup(repo => repo.GetObjectById(objectId)).Returns((Object)null);

            var result = _controller.GetObject(objectId);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
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

            var result = _controller.CreateObject2D(newObject);

            var createdResult = result as CreatedAtActionResult;
            Assert.IsNotNull(createdResult);
            Assert.AreEqual("GetObject", createdResult?.ActionName);
            _mockRepository.Verify();
        }

        [TestMethod]
        public void Put_ReturnsNotFound_WhenObjectDoesNotExist()
        {
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

            var result = _controller.Put(objectId, updatedObject);

            var notFoundResult = result as NotFoundResult;
            Assert.IsNotNull(notFoundResult);
        }

        [TestMethod]
        public void Delete_ReturnsNoContent_WhenObjectIsDeleted()
        {
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

            var result = _controller.Delete(objectId);

            var noContentResult = result as NoContentResult;
            Assert.IsNotNull(noContentResult);
            _mockRepository.Verify();
        }
    }
}
