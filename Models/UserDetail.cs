using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuisBot.Models
{
    [Serializable]
    public class UserDetail
    {
        public enum GenderOptions
        {
            Male, Female, Other, Unspecified
        };
        [Prompt("What is your name?")]
        public string Name { get; set; }
        [Prompt("What is your email?")]
        public string Email { get; set; }
        [Prompt("What is your phone number?")]
        public double PhoneNumber { get; set; }
        [Prompt("What's your job title?")]
        public string JobTitle { get; set; }
        [Prompt("Where do you work?")]
        public string Location { get; set; }
        [Prompt("What is your salary? (£)")]
        public double Salary { get; set; }
        [Optional]
        [Prompt("How many years of experience do you have?")]
        public int? YearsOfXp { get; set; }
        [Optional]
        [Prompt("How old are you?")]
        public int? Age { get; set; }
        [Optional]
        [Prompt("What is your {&}? {||}")]
        public GenderOptions? Gender { get; set; }        

        public static IForm<UserDetail> BuildForm()
        {
            OnCompletionAsyncDelegate<UserDetail> processOrder = async (context, state) =>
            {
                await context.PostAsync("Your PorgPowered Salary bot Has Been Successfully Completed. You will get a confirmation email and SMS. Thanks for using PorgPowered salary bot, Welcome Again And May Be the Porg With you!!! :)");
            };

            return new FormBuilder<UserDetail>().Message("Hi! Welcome to PorgPowered salary bot")
                .OnCompletion(processOrder)
                .Build();
        }
    }
}