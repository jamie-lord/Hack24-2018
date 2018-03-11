using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AdaptiveCards;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using LuisBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using SqlDataStore.Models;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"],
            ConfigurationManager.AppSettings["LuisAPIKey"],
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            var resultMessage = context.MakeMessage();

            string image = await GetGif("say what");

            StringBuilder noComprendeSB = new StringBuilder("Sorry, I didn't understand");
            if (!string.IsNullOrWhiteSpace(result?.Query))
            {
                noComprendeSB.Append($" what you meant when you said \"{result.Query}\"");
            }

            if (!string.IsNullOrWhiteSpace(image))
            {
                resultMessage.Attachments.Add(new AnimationCard(title: "Ooops", subtitle: noComprendeSB.ToString(), media: new List<MediaUrl> { new MediaUrl(image) }).ToAttachment());
            }
            else
            {
                await context.PostAsync(noComprendeSB.ToString());
            }

            await context.PostAsync(resultMessage);

            await context.PostAsync("Please try a different term or phrase.");
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Gretting" with the name of your newly created intent in the following handler
        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            var resultMessage = context.MakeMessage();

            string image = await GetGif("porg");

            if (!string.IsNullOrWhiteSpace(image))
            {
                resultMessage.Attachments.Add(new AnimationCard(media: new List<MediaUrl> { new MediaUrl(image) }, title: "Hi I'm PorgBot!").ToAttachment());
            }

            await context.PostAsync(resultMessage);
        }

        private async Task<string> GetGif(string searchFor)
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
                return gifResult.Data?[new Random().Next(0, gifResult.Data.Length + 1)]?.Images.Original.Url;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        [LuisIntent("Cancel")]
        public async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Help")]
        public async Task HelpIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        [LuisIntent("Image/Gif")]
        public async Task ImageIntent(IDialogContext context, LuisResult result)
        {
            var resultMessage = context.MakeMessage();

            string image = null;
            var query = result.Entities.FirstOrDefault()?.Entity;

            if (!string.IsNullOrWhiteSpace(query))
            {
                image = await GetGif(query);
            }

            if (string.IsNullOrWhiteSpace(image))
            {
                resultMessage.Text = "Hmmmm, I couldn't find an image for that. Sorry";
            }
            else
            {
                resultMessage.Attachments.Add(new AnimationCard { Media = new List<MediaUrl> { new MediaUrl(image) }, Title = "Looks like a " + query + " to me" }.ToAttachment());
            }

            await context.PostAsync(resultMessage);
        }

        [LuisIntent("Show me your horse")]
        public async Task AmazingHorseIntent(IDialogContext context, LuisResult result)
        {
            var resultMessage = context.MakeMessage();

            resultMessage.Attachments.Add(new VideoCard(title: "LOOK AT MY HORSE!", autoloop: true, autostart: true, media: new List<MediaUrl> { new MediaUrl("https://www.youtube.com/watch?v=O3rpmctmC_M") }).ToAttachment());

            await context.PostAsync(resultMessage);
        }

        [LuisIntent("Credit Card")]
        public async Task CreditCardIntent(IDialogContext context, LuisResult result)
        {
            try
            {
                List<CreditCard> creditCards = null;

                using (SqlDataStore.DataContext dataContext = new SqlDataStore.DataContext())
                {
                    var q = from creditCard in dataContext.CreditCards
                            select creditCard;

                    creditCards = q.ToList();
                }

                if (creditCards != null && creditCards.Count > 0)
                {
                    await context.PostAsync($"We've found {creditCards.Count} credit cards that might interest you...");

                    var resultMessage = context.MakeMessage();
                    resultMessage.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                    foreach (var creditCard in creditCards)
                    {
                        var adaptiveCard = CreditCardFactory(creditCard);
                        Attachment attachment = new Attachment()
                        {
                            ContentType = AdaptiveCard.ContentType,
                            Content = adaptiveCard
                        };
                        resultMessage.Attachments.Add(attachment);
                    }

                    await context.PostAsync(resultMessage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await context.PostAsync("I'm having some trouble getting credit card details at the moment, maybe try again in a minute or two.");
            }
        }

        private AdaptiveCard CreditCardFactory(CreditCard creditCard)
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

        [LuisIntent("StartSalaryQuery")]
        public async Task StartSalaryQueryIntent(IDialogContext context, LuisResult result)
        {
            var formDialog = new FormDialog<UserDetail>(new UserDetail(), UserDetail.BuildForm, FormOptions.PromptInStart, result.Entities);
            context.Call(formDialog, ResumeAfterSalaryQueryDialog);
        }

        [LuisIntent("PlayAGame")]
        public async Task StartPlayAGameIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Okay, let's play a gaaaaameeeeeeeee");

            LuisResult luisResult = new LuisResult("Saw play a game", new List<EntityRecommendation>()
            {
                new EntityRecommendation { Entity = "Saw play a game" }
            });
            await ImageIntent(context, luisResult);

            var formDialog = new FormDialog<MillionaireGame>(new MillionaireGame(), MillionaireGame.BuildForm, FormOptions.PromptInStart, result.Entities);
            context.Call(formDialog, ResumeAfterGameDialog);
        }

        private async Task ResumeAfterGameDialog(IDialogContext context, IAwaitable<MillionaireGame> result)
        {
            try
            {
                var gameQuery = await result;
                await context.PostAsync("Thanks for playing!");
                LuisResult luisResult = new LuisResult("Congratulations!", new List<EntityRecommendation>()
                {
                    new EntityRecommendation { Entity = "Congratulations!" }
                });
                await ImageIntent(context, luisResult);
            }
            catch (FormCanceledException e)
            {
                int questionStep = -1;
                if (e.Message.ToLowerInvariant().Equals("form quit."))
                {
                    await context.PostAsync("You quit? Really? Shame on you!");
                }
                else if (Int32.TryParse(e.Message, out questionStep))
                {
                    await context.PostAsync($"Oh dear oh dear. You couldn't get past question {questionStep}!");
                }
            }
            finally
            {
                context.Done<object>(null);
            }
        }
        private async Task ResumeAfterSalaryQueryDialog(IDialogContext context, IAwaitable<UserDetail> result)
        {
            try
            {
                var salaryQuery = await result;
                List<UserDetail> similarJobs = null;

                using (SqlDataStore.DataContext dataContext = new SqlDataStore.DataContext())
                {
                    salaryQuery.JobTitle = salaryQuery.JobTitle.ToLower();
                    dataContext.UserDetails.Add(salaryQuery);
                    dataContext.SaveChanges();

                    var q = from userDetail in dataContext.UserDetails
                            where userDetail.JobTitle.ToLower().Contains(salaryQuery.JobTitle.ToLower()) &&
                                  userDetail.Location.ToLower().Contains(salaryQuery.Location.ToLower())
                            select userDetail;

                    similarJobs = q.ToList();
                }

                if (similarJobs != null && similarJobs.Count() > 0)
                {
                    await context.PostAsync("Nice! Looks like we've found some people in your area that do the same thing as you.");

                    double averageSalary = 0;
                    foreach (var similarJob in similarJobs)
                    {
                        averageSalary = averageSalary + similarJob.Salary;
                    }

                    averageSalary = averageSalary / similarJobs.Count();

                    if (salaryQuery.Salary > averageSalary)
                    {
                        await context.PostAsync($"Wow, you earn above the average in your area for people with your job!");
                        await context.PostAsync($"The average salary is only £{string.Format("{0:0.00}", averageSalary)}");
                        await context.PostAsync($"That means you earn £{string.Format("{0:0.00}", (salaryQuery.Salary - averageSalary))} more than the average {salaryQuery.JobTitle} in your area.");
                    }
                    else
                    {
                        await context.PostAsync($"Hmmmm... Time to ask your boss about a pay increase. You earn less than the going rate in your area.");
                        await context.PostAsync($"The average salary for a {salaryQuery.JobTitle} is £{string.Format("{0:0.00}", averageSalary)}");
                        await context.PostAsync("So your salary is a bit lower than other people in your area. Don't worry! Want to take a break? Type 'Let's Play' for a game :)");
                    }
                }
                else
                {
                    await context.PostAsync(
                        "Your PorgPowered survey has been successfully completed. You will get a confirmation email and SMS. Thanks for using PorgPowered salary bot, Welcome Again And May The Porg Be With you!!! :)");
                }
            }
            catch (FormCanceledException ex)
            {
                string reply;
                if (ex.InnerException == null)
                {
                    reply = "It looks like you have quit the survey. Rerun this at any time by simply typing \"Start Salary Calculator\"";
                }
                else
                {
                    reply = $"Oops! Something went wrong :( Technical Details: {ex.InnerException.Message}";
                }

                await context.PostAsync(reply);
            }
            finally
            {
                context.Done<object>(null);
            }
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            context.Wait(MessageReceived);
        }
    }
}