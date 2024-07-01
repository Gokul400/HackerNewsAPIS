using HackerNews.LogicLayer;
using HackerNews.LogicLayer.Interface;
using HackerNewsAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace HackerNewsUnitTest
{
    public class HackerNewsControllerTests
    {
        [Fact]
        public async Task GetHackerNewsItem_ReturnsOkObjectResult()
        {
            // Arrange
            var memoryCacheMock = new Mock<IMemoryCache>();
            var hackerNewsLogicMock = new Mock<IHackerNewsLogic>();

            var controller = new HackerNewsController(memoryCacheMock.Object, hackerNewsLogicMock.Object);

            var itemList = new List<int> { 1, 2, 3 };
            var hackerItems = new List<HackerNewsItemModel>
            {
                new HackerNewsItemModel { Id = 1, title = "Item 1" },
                new HackerNewsItemModel { Id = 2, title = "Item 2" },
                new HackerNewsItemModel { Id = 3, title = "Item 3" }
            };

            hackerNewsLogicMock.Setup(x => x.GetTopStories()).ReturnsAsync(itemList);
            hackerNewsLogicMock.Setup(x => x.GetHackerNewsItem(itemList)).ReturnsAsync(hackerItems);

            // Act
            var result = await controller.GetHackerNewsItem();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsAssignableFrom<List<HackerNewsItemModel>>(okResult.Value);
            Assert.Equal(3, model.Count); // Assuming hackerItems contains 3 items
        }

        [Fact]
        public async Task GetHackerNewsItem_ReturnsBadRequestOnException()
        {
            // Arrange
            var memoryCacheMock = new Mock<IMemoryCache>();
            var hackerNewsLogicMock = new Mock<IHackerNewsLogic>();

            var controller = new HackerNewsController(memoryCacheMock.Object, hackerNewsLogicMock.Object);

            hackerNewsLogicMock.Setup(x => x.GetTopStories()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await controller.GetHackerNewsItem();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Test exception", badRequestResult.Value);
        }
    }
}