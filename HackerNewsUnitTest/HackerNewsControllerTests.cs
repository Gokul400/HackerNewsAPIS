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

            var itemList = new List<int> { 40832214, 40848340, 40845133 };
            var hackerItems = new List<HackerNewsItemModel>
            {
                new HackerNewsItemModel {
                                        Id = 40832214,
                                        title = "Thousands of Pablo Picasso’s works in a new online archive",
                                        url = "https://www.smithsonianmag.com/smart-news/thousands-of-pablo-picassos-works-are-now-online-180984597/"
                                        },
               new HackerNewsItemModel  {
                                        Id = 40848340,
                                        title = "A Survey of General-Purpose Polyhedral Compilers",
                                        url= "https://dl.acm.org/doi/abs/10.1145/3674735"
                                        },
                new HackerNewsItemModel  {
                                        Id = 40845133,
                                        title = "Arctic 'dirty fuel' ban for ships comes into force",
                                        url ="https://www.bbc.com/news/articles/cpv3dk4ydr3o"
                                        }
            };
            object cachedItems = null;
            memoryCacheMock.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out cachedItems)).Returns(false);
            var cacheEntryMock = new Mock<ICacheEntry>();
            memoryCacheMock.Setup(mc => mc.CreateEntry(It.IsAny<object>())).Returns(cacheEntryMock.Object);


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