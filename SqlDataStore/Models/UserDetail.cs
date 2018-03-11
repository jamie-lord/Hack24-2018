using Microsoft.Bot.Builder.FormFlow;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Text.RegularExpressions;

namespace LuisBot.Models
{
    [Serializable]
    public class UserDetail
    {
        public enum GenderOptions
        {
            Male, Female, Other, Skip
        };

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Prompt("What's your first name?")]
        public string Name { get; set; }
        [Prompt("Great {Name:string}, now I need your email address?")]
        public string Email { get; set; }
        [Prompt("What is your phone number {Name:string}?")]
        public double PhoneNumber { get; set; }
        [Prompt("Ok {Name:string}, What's your job title?")]
        public string JobTitle { get; set; }
        [Prompt("Where is your work place?")]
        public string Location { get; set; }
        [Prompt("How much do you earn being a {JobTitle:string}?")]
        public double Salary { get; set; }
        [Optional]
        [Prompt("How many years have you worked in {Location:string}?")]
        [Describe("Years of Experience")]
        [Template(TemplateUsage.NoPreference, "Skip")]
        public int? YearsOfXp { get; set; }
        [Optional]
        [Prompt("How old are you {Name:string}?")]
        [Describe("Age")]
        [Template(TemplateUsage.NoPreference, "Skip")]
        public int? Age { get; set; }
        [Prompt("How would you identify yourself? {||}")]
        public GenderOptions? Gender { get; set; }

        public static IForm<UserDetail> BuildForm()
        {
            //OnCompletionAsyncDelegate<UserDetail> processOrder = async (context, state) =>
            //{
            //    await context.PostAsync("Your PorgPowered Salary bot Has Been Successfully Completed. You will get a confirmation email and SMS. Thanks for using PorgPowered salary bot, Welcome Again And May The Porg Be With you!!! :)");
            //};

            return new FormBuilder<UserDetail>()
                .Message("Okay, lets see if you're being paid well...")
                .Message("I just need the answer to a few simple questions.")
                //.OnCompletion(processOrder)
                .Build();
        }
    }
}