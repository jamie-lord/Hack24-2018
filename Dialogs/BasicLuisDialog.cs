using System;
using System.Configuration;
using System.Linq;
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

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "Gretting" with the name of your newly created intent in the following handler
        [LuisIntent("Greeting")]
        public async Task GreetingIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
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
                try
                {
                    var giphy = new Giphy("oZy0HYzaXNCmrq0dNOGyiuZgyaaTc3hL");
                    var searchParameter = new SearchParameter()
                    {
                        Query = query
                    };
                    // Returns gif results
                    var gifResult = await giphy.GifSearch(searchParameter);
                    image = gifResult.Data.FirstOrDefault()?.Images.Original.Url;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
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
            await context.PostAsync("Please wait while we load the Salary Calculator...");

            var formDialog = new FormDialog<UserDetail>(new UserDetail(), UserDetail.BuildForm);
            context.Call(formDialog, ResumeAfterSalaryQueryDialog);
        }

        private async Task ResumeAfterSalaryQueryDialog(IDialogContext context, IAwaitable<UserDetail> result)
        {
            await context.PostAsync("Thanks, I'll just work out how well you get paid...");
        }

        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            context.Wait(MessageReceived);
        }
    }
}