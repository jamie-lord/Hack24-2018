using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using System.Web.Http.Description;
using System.Net.Http;
using LuisBot.Models;
using Microsoft.Bot.Builder.FormFlow;
using System.Linq;

namespace Microsoft.Bot.Sample.LuisBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        internal static IDialog<UserDetail> MakeUserDetailDialog()
        {
            return Chain.From(() => FormDialog.FromForm(UserDetail.BuildForm));
        }

        /// <summary>
        /// POST: api/Messages
        /// receive a message from a user and send replies
        /// </summary>
        /// <param name="activity"></param>
        [ResponseType(typeof(void))]
        public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            // check if activity is of type message
            if (activity.GetActivityType() == ActivityTypes.Message)
            {
                var mentions = activity.GetMentions()?.ToList();
                var message = activity.Text;
                bool mentioned = false;
                foreach (var mention in mentions)
                {
                    if(mention.Mentioned.Id == activity.Recipient.Id)
                    {
                        if (!string.IsNullOrWhiteSpace(mention.Text))
                        {
                            message = message.Replace(mention.Text, string.Empty);
                            mentioned = true;
                        }
                    }
                }

                activity.Text = message;

                if (mentioned)
                    await Conversation.SendAsync(activity, () => new BasicLuisDialog());

                // The following is for FormFlow dialog.
                // TODO: uncomment when we can
                //await Conversation.SendAsync(activity, MakeUserDetailDialog);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}