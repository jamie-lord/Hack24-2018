using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GiphyDotNet.Manager;
using GiphyDotNet.Model.Parameters;
using LuisBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;

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
            await context.PostAsync("Hi I'm PorgBot!");

            var resultMessage = context.MakeMessage();

            string image = await GetGif("porg");

            if (!string.IsNullOrWhiteSpace(image))
            {
                resultMessage.Attachments.Add(new Attachment()
                {
                    ContentUrl = image,
                    ContentType = "image/png"
                });
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
                resultMessage.Attachments.Add(new Attachment()
                {
                    ContentUrl = image,
                    ContentType = "image/png"
                });
            }

            await context.PostAsync(resultMessage);
        }

        [LuisIntent("StartSalaryQuery")]
        public async Task StartSalaryQueryIntent(IDialogContext context, LuisResult result)
        {
            var formDialog = new FormDialog<UserDetail>(new UserDetail(), UserDetail.BuildForm, FormOptions.PromptInStart, result.Entities);
            context.Call(formDialog, ResumeAfterSalaryQueryDialog);
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