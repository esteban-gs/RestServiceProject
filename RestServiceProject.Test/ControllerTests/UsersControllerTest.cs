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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

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

            // Add
            _unitOfWork.Setup(repo => repo.Repository<User>().Add(It.IsAny<User>()))
                .Callback((User user) =>
                {
                    user.Id = new Guid("ab2bd817-98cd-4cf3-a80a-53ea0cd9c200");
                    _testUserData.Add(user);
                }).Returns(Task.CompletedTask);

            // Remove
            _unitOfWork.Setup(repo => repo.Repository<User>().Remove(It.IsAny<User>()))
                .Callback((User user) =>
                {
                    _testUserData.Remove(user);
                });

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
            // Act
            var okResult = await _userController.GetUser(TestUsers.ExistingGuid);

            // Assert
            Assert.IsType<OkObjectResult>(okResult.Result);
        }

        [Fact]
        public void GetById_ExistingGuidPassed_ReturnsRightItem()
        {
            // Act
            var okResult = _userController.GetUser(TestUsers.ExistingGuid).Result.Result as ObjectResult;

            // Assert
            Assert.IsType<UserViewModel>(okResult.Value);
            Assert.Equal(TestUsers.ExistingGuid.ToString(), (okResult.Value as UserViewModel).Id);
        }
        #endregion GetByID

        #region Add
        [Fact]
        public void Add_InvalidObjectPassed_ReturnsBadRequest()
        {
            // Arrange
            var itemMissingEmail = new UserInputModel()
            {
                Email = String.Empty,
                Password = TestUsers.TestPassWord,
                PasswordConfirm = TestUsers.TestPassWord
            };
            _userController.ModelState.AddModelError("Email", "Required");

            // Act 
            var badResponse = _userController.Post(itemMissingEmail).Result.Result;

            // Assert
            Assert.IsType<BadRequestObjectResult>(badResponse);
        }

        [Fact]
        public void Add_ValidObjectPassed_RetrunsCreatedResource()
        {
            // Arrange
            UserInputModel uim = new UserInputModel()
            {
                Email = TestUsers.PostEmail,
                Password = TestUsers.TestPassWord,
                PasswordConfirm = TestUsers.TestPassWord
            };

            // Act 
            var createdResponse = _userController.Post(uim).Result.Result;

            // Assert
            Assert.IsType<CreatedAtRouteResult>(createdResponse);
        }

        [Fact]
        public void Add_ValidObjectPassed_ReturnedResponseHasCreatedItem()
        {
            // Arrange
            var testItem = new UserInputModel()
            {
                Email = TestUsers.PostEmail,
                Password = TestUsers.TestPassWord,
                PasswordConfirm = TestUsers.TestPassWord
            };

            // Act
            var createdResponse = _userController.Post(testItem).Result.Result as CreatedAtRouteResult;
            var item = createdResponse.Value as UserViewModel;

            // Assert
            Assert.IsType<UserViewModel>(item);
            Assert.Equal(TestUsers.PostEmail, item.Email);
        }

        #endregion Add

        #region Remove
        [Fact]
        public void Remove_NotExistingGuidPassed_ReturnsNotFoundResponse()
        {
            // Arrange
            var notExistingGuid = Guid.NewGuid();

            // Act
            var badResponse = _userController.Delete(notExistingGuid).Result;

            // Assert
            Assert.IsType<NotFoundResult>(badResponse);
        }

        [Fact]
        public void Remove_ExistingGuidPassed_ReturnsOkResult()
        {
            // Act
            var noContentResponse = _userController.Delete(TestUsers.ExistingGuid).Result;

            // Assert
            Assert.IsType<NoContentResult>(noContentResponse);
        }

        [Fact]
        public void Remove_ExistingGuidPassed_RemovesOneItem()
        {
            // Act
            var okResponse = _userController.Delete(TestUsers.ExistingGuid).Result;

            // Assert
            Assert.Equal(8, _testUserData.Count());
            Assert.NotEqual(9, _testUserData.Count());
        }
        #endregion Remove
    }
}
