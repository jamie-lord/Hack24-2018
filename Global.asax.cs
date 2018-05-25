using Autofac;
using System.Web.Http;
using System.Configuration;
using System.Reflection;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using LuisBot;
#if DEBUG
#else
using Microsoft.Bot.Builder.Dialogs.Internals;
#endif

namespace SimpleEchoBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Bot Storage: This is a great spot to register the private state storage for your bot. 
            // We provide adapters for Azure Table, CosmosDb, SQL Azure, or you can implement your own!
            // For samples and documentation, see: https://github.com/Microsoft/BotBuilder-Azure

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<IMessageActivity, SqlDataStore.Models.Activity>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForMember(dest => dest.FromId, opt => opt.MapFrom(src => src.From.Id))
                    .ForMember(dest => dest.RecipientId, opt => opt.MapFrom(src => src.Recipient.Id))
                    .ForMember(dest => dest.FromName, opt => opt.MapFrom(src => src.From.Name))
                    .ForMember(dest => dest.RecipientName, opt => opt.MapFrom(src => src.Recipient.Name));
            });

            Conversation.UpdateContainer(
                containerBuilder =>
                {
                    containerBuilder.RegisterType<EntityFrameworkActivityLogger>().AsImplementedInterfaces().InstancePerDependency();
                    containerBuilder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));
#if DEBUG
                    ConfigurationManager.AppSettings["LuisAppId"] = "fc5a8123-58b1-45ce-9e97-1e3d03869a72";
                    ConfigurationManager.AppSettings["LuisAPIKey"] = "462b9fade9d2421890f561b307c81e25";
                    ConfigurationManager.AppSettings["LuisAPIHostName"] = "westeurope.api.cognitive.microsoft.com";
#else
                    // Using Azure Table Storage
                    var store = new TableBotDataStore(ConfigurationManager.AppSettings["AzureWebJobsStorage"]); // requires Microsoft.BotBuilder.Azure Nuget package 

                    // To use CosmosDb or InMemory storage instead of the default table storage, uncomment the corresponding line below
                    // var store = new DocumentDbBotDataStore("cosmos db uri", "cosmos db key"); // requires Microsoft.BotBuilder.Azure Nuget package 
                    // var store = new InMemoryDataStore(); // volatile in-memory store

                    containerBuilder.Register(c => store)
                        .Keyed<IBotDataStore<BotData>>(AzureModule.Key_DataStore)
                        .AsSelf()
                        .SingleInstance();
#endif
                });
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
