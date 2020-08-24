using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RestServiceProject.Controllers;
using RestServiceProject.Data;
using RestServiceProject.Data.Repositories;
using RestServiceProject.Mapper;
using RestServiceProject.Models;
using RestServiceProject.Test.TestData;
using RestServiceProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RestServiceProject.Test.ControllerTests
{
    public class UsersControllerTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly UserController _userController;


        private readonly MapperConfiguration _mapperConfig;
        private readonly IMapper _mapper;

        private List<User> _testUserData;

        /// <summary>
        /// Arrange Test
        /// </summary>
        public UsersControllerTest()
        {
            _mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new Profiles());
            });

            _mapper = _mapperConfig.CreateMapper();

            _unitOfWork = new Mock<IUnitOfWork>();

            _testUserData = DataGenerator.GenerateUsers();

            // Get Many
            _unitOfWork.Setup(repo => repo.Repository<User>().FindAll())
                .ReturnsAsync(_testUserData);

            // Get One
            _unitOfWork.Setup(repo => repo.Repository<User>().FindById(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => _testUserData.SingleOrDefault(u => u.Id == id));

            _userController = new UserController(_mapper, _unitOfWork.Object);

        }

        #region GetMany
        [Fact]
        public async Task Get_Users_Returns_Ok()
        {
            //Act
            var okResult = await _userController.Get();

            //Asset
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public async Task Get_Users_Returns_All_Items()
        {
            //Act
            var result = await _userController.Get();
            var okResult = result.Result as OkObjectResult;

            //Assert
            var items = Assert.IsType<List<UserViewModel>>(okResult.Value);
            Assert.Equal(9, items.Count);
        }

        [Fact]
        public async Task Get_Users_Returns_200_Ok()
        {
            //Act
            var result = await _userController.Get();
            var okResult = result.Result as OkObjectResult;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
        }
        #endregion GetMany


        #region GetByID
        [Fact]
        public async Task GetById_UnknownGuidPassed_Returns_Not_Found()
        {
            //Act
            var notFoundResult = await _userController.GetUser(new Guid());

            //Assert
            Assert.IsType<NotFoundResult>(notFoundResult.Result);
        }

        [Fact]
        public async Task GetById_ExistingGuidPassed_ReturnsOkResult()
        {
            // Arrange
            Func<User, bool> isFirstUser = u => u.Email.Contains("test1"); // existing user
            var existingUser = _testUserData.FirstOrDefault(isFirstUser);

            // Act
            var okResult = await _userController.GetUser(existingUser.Id);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void GetById_ExistingGuidPassed_ReturnsRightItem()
        {
            // Arrange
            Func<User, bool> isFirstUser = u => u.Email.Contains("test1"); // existing user
            var existingUserId = _testUserData.FirstOrDefault(isFirstUser).Id;

            // Act
            var okResult = _userController.GetUser(existingUserId).Result.Result as ObjectResult;

            // Assert
            Assert.IsType<UserViewModel>(okResult.Value);
            Assert.Equal(existingUserId.ToString(), (okResult.Value as UserViewModel).Id);
        }
        #endregion GetByID
    }
}
