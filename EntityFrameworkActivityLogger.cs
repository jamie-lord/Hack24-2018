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
            try
            {
                using (SqlDataStore.DataContext dataContext = new SqlDataStore.DataContext())
                {
                    var newActivity = Mapper.Map<IMessageActivity, SqlDataStore.Models.Activity>(msg);
                    dataContext.Activities.Add(newActivity);
                    dataContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return Task.CompletedTask;
        }
    }
}