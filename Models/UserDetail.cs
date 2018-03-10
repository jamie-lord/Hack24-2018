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
        [Pattern("/(.*[a-z]){3,}/i")]
        [Prompt("What is your name?")]
        public string Name { get; set; }
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
        [Prompt("What is your gender?")]
        public GenderOptions? Gender { get; set; }

        public static IForm<UserDetail> BuildForm()
        {
            return new FormBuilder<UserDetail>().Message("Hi! Welcome to PorgPowered salary bot").Build();
        }
    }
}