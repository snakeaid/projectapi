using System;
using System.Threading.Tasks;
using Xunit;
using Moq;
using ProjectAPI.Controllers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace ProjectAPI.Tests
{
    public class AccountControllerTests
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            var mockLogger = new Mock<ILogger<AccountController>>();
            AccountController accountController = new AccountController(mockLogger.Object);

            //Act
            var result = accountController.Token("manager", "12345") as ObjectResult;

            var test = new
            {
                access_token = "1",
                username = "1"
            };

            //Assert
            
        }
    }
}

