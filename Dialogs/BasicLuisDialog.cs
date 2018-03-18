using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdaptiveCards;
using LuisBot.Extensions;

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

        #region Intents
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            StringBuilder noComprendeSB = new StringBuilder("Sorry, I didn't understand");
            if (!string.IsNullOrWhiteSpace(result?.Query))
            {
                noComprendeSB.Append($" what you meant when you said \"{result.Query}\"");
            }

            if (!await context.TryPostGifAsync("say what", "Oops I didn't understand"))
            {
                await context.PostAsync(noComprendeSB.ToString());
            }

            await context.PostAsync("Please try a different term or phrase.");
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Gretting" with the name of your newly created intent in the following handler
        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            await context.TryPostGifAsync("porg", "Hi I'm PorgBot!");
        }       

        [LuisIntent("Cancel")]
        public async Task CancelIntent(IDialogContext context, LuisResult result)
        {
            await ShowLuisResult(context, result);
        }

        [LuisIntent("Help")]
        public async Task HelpIntent(IDialogContext context, LuisResult result)
        {
            await ShowLuisResult(context, result);
        }

        [LuisIntent("Image/Gif")]
        public async Task ImageIntent(IDialogContext context, LuisResult result)
        {
            var quantityQueryStr = result.Entities.FirstOrDefault(e => e.Type.ToLowerInvariant().CompareTo("builtin.number") == 0)?.Entity;
            var imageQuery = result.Entities.FirstOrDefault(e => e.Type.ToLowerInvariant().CompareTo("image") == 0)?.Entity;

            string titleComment = $"Here's a {imageQuery}";

            int quantityQuery = -1;
            if (!string.IsNullOrWhiteSpace(quantityQueryStr))
            {
                if (!Int32.TryParse(quantityQueryStr, out quantityQuery))
                {
                    quantityQuery = (int)quantityQueryStr.ToLong();
                }

                if (quantityQuery > 0)
                {
                    titleComment = $"Here are some {imageQuery}";
                }
            }

            if (!await context.TryPostGifAsync(imageQuery, titleComment))
            {
                var resultMessage = context.MakeMessage();
                resultMessage.Text = "Hmmmm, I couldn't find an image for that. Sorry";
                await context.PostAsync(resultMessage);
            }

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
                        var adaptiveCard = creditCard.CreateAdaptiveCard();
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


        [LuisIntent("Farewells")]
        public async Task FareWellIntent(IDialogContext context, LuisResult result)
        {
            await context.TryPostGifAsync("Bye!", "Bye!");
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

            await context.TryPostGifAsync("saw play a game");

            var formDialog = new FormDialog<MillionaireGame>(new MillionaireGame(), MillionaireGame.BuildForm, FormOptions.PromptInStart, result.Entities);
            context.Call(formDialog, ResumeAfterGameDialog);
        }

        private async Task ResumeAfterGameDialog(IDialogContext context, IAwaitable<MillionaireGame> result)
        {
            try
            {
                var gameQuery = await result;
                await context.PostAsync("Thanks for playing!");

                await context.TryPostGifAsync("Congratulations!");
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
                else
                {
                    await context.PostAsync("You failed!");
                }
            }
            finally
            {
                context.Done<object>(null);
            }
        }
        #endregion

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

                    similarJobs = q.Where(j => j.Id != salaryQuery.Id).ToList();
                }

                if (similarJobs != null && similarJobs.Any())
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
                    await context.PostAsync("Your PorgPowered survey has been successfully completed. " +
                        "Thanks for using PorgPowered salary bot. We'll be in touch to let you know how you rank! :)");
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