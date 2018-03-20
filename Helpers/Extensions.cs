using AdaptiveCards;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using SqlDataStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace LuisBot.Helpers
{
    public static class Extensions
    {
        public static AdaptiveCard CreateAdaptiveCard(this CreditCard creditCard)
        {
            AdaptiveCard card = new AdaptiveCard()
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveContainer()
                    {
                        SelectAction = new AdaptiveOpenUrlAction()
                        {
                            Url = new Uri(creditCard.Link)
                        },
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveTextBlock(creditCard.Name) {Size = AdaptiveTextSize.Large, Color = AdaptiveTextColor.Dark, Weight = AdaptiveTextWeight.Bolder},
                            new AdaptiveImage(creditCard.ImageUrl) {Size = AdaptiveImageSize.Large, Spacing = AdaptiveSpacing.Padding},
                            new AdaptiveTextBlock("Annual fee: **£" +  creditCard.AnnualFee.ToString() + "**"),
                            new AdaptiveColumnSet() {Columns = new List<AdaptiveColumn>
                            {
                                new AdaptiveColumn() {Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveTextBlock("Purchase APR: **" + creditCard.PurchaseApr.ToString() + "%**")
                                }},
                                new AdaptiveColumn()
                                {
                                    Items = new List<AdaptiveElement>
                                    {
                                        new AdaptiveImage("https://images.experian.co.uk/rebrand//experian_full_colour.svg") {Size = AdaptiveImageSize.Medium, Spacing = AdaptiveSpacing.Padding, HorizontalAlignment = AdaptiveHorizontalAlignment.Right}
                                    },

                                }
                            }}
                        }
                    }
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveOpenUrlAction()
                    {
                        Url = new Uri(creditCard.Link),
                        Title = "See more details"
                    }
                }
            };
            return card;
        }

        private static Dictionary<string, long> numberTable =
                new Dictionary<string, long>
        {{"zero",0},{"one",1},{"two",2},{"three",3},{"four",4},
        {"five",5},{"six",6},{"seven",7},{"eight",8},{"nine",9},
        {"ten",10},{"eleven",11},{"twelve",12},{"thirteen",13},
        {"fourteen",14},{"fifteen",15},{"sixteen",16},
        {"seventeen",17},{"eighteen",18},{"nineteen",19},{"twenty",20},
        {"thirty",30},{"forty",40},{"fifty",50},{"sixty",60},
        {"seventy",70},{"eighty",80},{"ninety",90},{"hundred",100},
        {"thousand",1000},{"million",1000000},{"billion",1000000000},
        {"trillion",1000000000000},{"quadrillion",1000000000000000},
        {"quintillion",1000000000000000000}};

        /// <summary>
        /// Taken from https://stackoverflow.com/questions/11278081/convert-words-string-to-int
        /// </summary>
        /// <param name="numberString">String worded number representation</param>
        /// <returns>long version of word representation</returns>
        public static long ToLong(this string numberString)
        {
            var numbers = Regex.Matches(numberString, @"\w+").Cast<Match>()
                 .Select(m => m.Value.ToLowerInvariant())
                 .Where(v => numberTable.ContainsKey(v))
                 .Select(v => numberTable[v]);

            long acc = 0, total = 0L;
            foreach (var n in numbers)
            {
                if (n >= 1000)
                {
                    total += (acc * n);
                    acc = 0;
                }
                else if (n >= 100)
                {
                    acc *= n;
                }
                else acc += n;
            }
            return (total + acc) * (numberString.StartsWith("minus",
                  StringComparison.InvariantCultureIgnoreCase) ? -1 : 1);
        }

        public static async Task<bool> TryPostGifAsync(this IDialogContext context, string searchTerm, string title = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return false;
            try
            {
                MediaUrl mediaUrl = await GifHelpers.GetGifMediaUrlAsync(searchTerm);

                if (mediaUrl != null)
                {
                    var message = context.MakeMessage();
                    message.AddAnimationCard(mediaUrl, title);
                    await context.PostAsync(message);
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void AddAnimationCard(this IMessageActivity messageActivity, MediaUrl mediaUrl, string title = null)
        {
            var animationCard = new AnimationCard { Media = new List<MediaUrl> { mediaUrl } };
            if (!string.IsNullOrWhiteSpace(title))
            {
                animationCard.Title = title;
            }
            messageActivity.Attachments.Add(animationCard.ToAttachment());
        }
    }
}