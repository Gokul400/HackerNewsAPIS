using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.LogicLayer.Interface
{
    public interface IHackerNewsLogic
    {
        Task<List<int>> GetTopStories();
        Task<List<HackerNewsItemModel>> GetHackerNewsItem(List<int> args);
    }
}