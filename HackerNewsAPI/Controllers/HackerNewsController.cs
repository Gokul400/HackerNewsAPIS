using HackerNews.LogicLayer;
using HackerNews.LogicLayer.Interface;
using HackerNews.LogicLayer.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace HackerNewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private static readonly string cacheKey = "hackerItems";
        private IHackerNewsLogic _hackerNewsLogic;

        public HackerNewsController(IMemoryCache memoryCache, IHackerNewsLogic hackerNewsLogic)
        {
            _hackerNewsLogic = hackerNewsLogic;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async  Task<IActionResult> GetHackerNewsItem()
        {
            try
            {
                if (!_memoryCache.TryGetValue(cacheKey, out List<HackerNewsItemModel>? hackerItems))
                {
                   
                    // Cache miss: fetch the items and add them to the cache
                    List<int> itemList = await _hackerNewsLogic.GetTopStories();
                    if (itemList != null)
                    {
                        
                        hackerItems = await _hackerNewsLogic.GetHackerNewsItem(itemList);

                        if (hackerItems != null)
                        {
                           
                            var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                            _memoryCache.Set(cacheKey, hackerItems, cacheEntryOptions);
                        }
                        else
                        {
                            // Handle the case where hackerItems is null
                            // Log an error or throw an exception if necessary
                            throw new Exception("Failed to fetch HackerNews items.");
                        }
                    }
                    else
                    {
                        // Handle the case where itemList is null
                        // Log an error or throw an exception if necessary
                        throw new Exception("Failed to fetch top stories.");
                    }
                }

                return Ok(hackerItems);
            }
            catch (Exception ex) { 
            return  BadRequest(ex.Message);
            }

        }
    }
}
