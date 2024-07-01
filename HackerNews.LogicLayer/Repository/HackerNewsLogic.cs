using HackerNews.LogicLayer.Interface;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.LogicLayer.Repository
{
    public class HackerNewsLogic : IHackerNewsLogic
    {
        public async Task<List<int>> GetTopStories()
        {
            string url = "https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty";
            List<int> storyIds = new List<int>();
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    storyIds = JsonConvert.DeserializeObject<List<int>>(responseBody);
                    if(storyIds.Count > 200)
                    {
                        storyIds = storyIds.Take(200).ToList();
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }
            }

            return storyIds;
        }


        public async Task<List<HackerNewsItemModel>> GetHackerNewsItem(List<int> args)
        {
            List<HackerNewsItemModel> hackerNewsList = new List<HackerNewsItemModel>();
            try
            {
                foreach (int i in args)
                {
                    string url = "https://hacker-news.firebaseio.com/v0/item/" + i + ".json?print=pretty";

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(url);
                        response.EnsureSuccessStatusCode();

                        string responseBody = await response.Content.ReadAsStringAsync();
                        HackerNewsItemModel item = JsonConvert.DeserializeObject<HackerNewsItemModel>(responseBody);
                        hackerNewsList.Add(item);
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }

            return hackerNewsList.Where(x=>!string.IsNullOrEmpty(x.url)).ToList();
        }
    }
}
