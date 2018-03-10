using AutoMapper;
using Microsoft.Bot.Builder.History;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace LuisBot
{
    public class EntityFrameworkActivityLogger : IActivityLogger
    {
        Task IActivityLogger.LogAsync(IActivity activity)
        {
            IMessageActivity msg = activity.AsMessageActivity();
            using (SqlDataStore.ConversationDataContext dataContext = new SqlDataStore.ConversationDataContext())
            {
                var newActivity = Mapper.Map<IMessageActivity, SqlDataStore.Models.Activity>(msg);
                if (string.IsNullOrEmpty(newActivity.Id))
                    newActivity.Id = Guid.NewGuid().ToString();
                dataContext.Activities.Add(newActivity);
                dataContext.SaveChanges();
            }
            return Task.CompletedTask;
        }
    }
}