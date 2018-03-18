using LuisBot.Helpers;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuisBot.Extensions
{
    public static class MessageExtensions
    {
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
            var animationCard = new AnimationCard { Media = new List<MediaUrl> { mediaUrl }};
            if (!string.IsNullOrWhiteSpace(title))
            {
                animationCard.Title = title;
            }
            messageActivity.Attachments.Add(animationCard.ToAttachment());
        }
    }
}