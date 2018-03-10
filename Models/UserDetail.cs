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
            Male, Female, Other, Skip
        };

        [Prompt("What is your {&}?")]
        public string Name { get; set; }
        [Prompt("Where do you work?")]
        public string Location { get; set; }
        [Prompt("What is your {&}? (£)")]
        public double Salary { get; set; }
        [Optional]
        [Prompt("How many years of experience do you have?")]
        [Describe("Years of Experience")]
        [Template(TemplateUsage.NoPreference, "Skip")]
        public int? YearsOfXp { get; set; }
        [Optional]
        [Prompt("How old are you?")]
        [Describe("Age")]
        [Template(TemplateUsage.NoPreference, "Skip")]
        public int? Age { get; set; }
        [Optional]
        [Prompt("What is your {&}? {||}")]
        public GenderOptions? Gender { get; set; }

        public static IForm<UserDetail> BuildForm()
        {
            return new FormBuilder<UserDetail>().Message("Hi! Welcome to PorgPowered salary bot").Build();
        }
    }
}