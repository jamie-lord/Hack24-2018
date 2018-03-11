using System;
using System.Configuration;
using System.Threading.Tasks;
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

        [LuisIntent("Porg")]
        public async Task PorgIntent(IDialogContext context, LuisResult result)
        {
            var resultMessage = context.MakeMessage();

            resultMessage.Attachments.Add(new Attachment()
            {
                ContentUrl = "https://nerdist.com/wp-content/uploads/2017/12/porg-yell.gif",
                ContentType = "image/png"
            });
            await context.PostAsync(resultMessage);
        }

        [LuisIntent("StartSalaryQuery")]
        public async Task StartSalaryQueryIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Please wait while we load the PorgPowered Salary Calculator...");

            var formDialog = new FormDialog<UserDetail>(new UserDetail(), UserDetail.BuildForm, FormOptions.PromptInStart, result.Entities);
            context.Call(formDialog, ResumeAfterSalaryQueryDialog);
        }

        private async Task ResumeAfterSalaryQueryDialog(IDialogContext context, IAwaitable<UserDetail> result)
        {
            try
            {
                var salaryQuery = await result;
                //if (salaryQuery.)

                await context.PostAsync("Your PorgPowered survey has been successfully completed. You will get a confirmation email and SMS. Thanks for using PorgPowered salary bot, Welcome Again And May The Porg Be With you!!! :)");
            }
            catch(FormCanceledException ex)
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