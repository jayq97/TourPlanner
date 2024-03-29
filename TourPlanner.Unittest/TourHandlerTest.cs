﻿using Moq;
using NUnit.Framework;
using System;
using TourPlanner.BusinessLayer.Logger;
using TourPlanner.BusinessLayer.TourHandler;
using TourPlanner.DataAccessLayer.Database;
using TourPlanner.DataAccessLayer.FileSystem;
using TourPlanner.Models;

namespace TourPlanner.Unittest
{
    public class TourHandlerTest
    {
        private Mock<IDatabase> _database = new Mock<IDatabase>();
        private Mock<IFileSystem> _fileSystem = new Mock<IFileSystem>();
        private Mock<ILog4NetLogger> _logger = new Mock<ILog4NetLogger>();

        private ITourHandler _tourHandler;

        private Tour testTour1;
        private TourLog testTourLog1;

        [SetUp]
        public void Setup()
        {
            _tourHandler = TourHandlerFactory.GetHandler(_database.Object, _fileSystem.Object, _logger.Object);
            testTour1 = new Tour(1, "TestTour1", "Description1", "Wien", "Graz", "Car", 200, new TimeSpan(2, 0, 0));
            testTourLog1 = new TourLog("Comment1", "Medium", new TimeSpan(2, 30, 30), 4);
        }

        [Test]
        public void Test_SameInstance()
        {
            ITourHandler _secondTourHandler = TourHandlerFactory.GetHandler(_database.Object, _fileSystem.Object, _logger.Object);

            Assert.AreEqual(_tourHandler, _secondTourHandler);
        }

        [Test]
        public void Test_AddNewTour()
        {
            _tourHandler.AddNewTour(testTour1);

            _database.Verify(mock => mock.AddNewTour(testTour1), Times.Once());
            _fileSystem.Verify(mock => mock.SaveImageFile(testTour1, It.IsAny<int>()), Times.Once());
            _database.Verify(mock => mock.AddNewTourLog(testTour1.Id, testTourLog1), Times.Never());
        }

        [Test]
        public void Test_AddNewTourLog()
        {
            _tourHandler.AddNewTourLog(testTour1.Id, testTourLog1);

            _database.Verify(mock => mock.AddNewTourLog(testTour1.Id, testTourLog1), Times.Once());
        }

        [Test]
        public void Test_DeleteTour()
        {
            _tourHandler.DeleteTour(testTour1);

            _database.Verify(mock => mock.DeleteTour(testTour1), Times.Once());
            _fileSystem.Verify(mock => mock.DeleteImageFile(testTour1), Times.Once());
        }

        [Test]
        public void Test_GetImageFile()
        {
            _tourHandler.GetImageFile(testTour1);

            _fileSystem.Verify(mock => mock.LoadImageFile(testTour1), Times.Once());
        }
    }
}