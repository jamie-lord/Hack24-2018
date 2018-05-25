using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace LuisBot.Helpers
{
    public static class GifHelpers
    {
        public static async Task<MediaUrl> GetGifMediaUrlAsync(string searchFor)
        {
            try
            {
                var giphy = new Giphy("oZy0HYzaXNCmrq0dNOGyiuZgyaaTc3hL");
                var searchParameter = new SearchParameter()
                {
                    Query = searchFor
                };
                // Returns gif results
                var gifResult = await giphy.GifSearch(searchParameter);

                var stringUrl = gifResult.Data?[new Random().Next(0, gifResult.Data.Length + 1)]?.Images.Original.Url;

                return new MediaUrl(stringUrl);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}