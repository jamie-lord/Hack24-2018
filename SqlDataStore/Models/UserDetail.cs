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

        [Prompt("What is your {&}?")]
        public string Name { get; set; }
        [Prompt("What is your email?")]
        public string Email { get; set; }
        [Prompt("What is your phone number?")]
        public double PhoneNumber { get; set; }
        [Prompt("What's your job title?")]
        public string JobTitle { get; set; }
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
        [Prompt("What is your {&}? {||}")]
        public GenderOptions? Gender { get; set; }

        public static IForm<UserDetail> BuildForm()
        {
            //OnCompletionAsyncDelegate<UserDetail> processOrder = async (context, state) =>
            //{
            //    await context.PostAsync("Your PorgPowered Salary bot Has Been Successfully Completed. You will get a confirmation email and SMS. Thanks for using PorgPowered salary bot, Welcome Again And May The Porg Be With you!!! :)");
            //};

            return new FormBuilder<UserDetail>().Message("Hi! Welcome to PorgPowered salary bot")
                .Field(nameof(Name))
                .Field(nameof(Email),
                validate: async (state, response) =>
                {
                    var result = new ValidateResult { IsValid = true, Value = response };
                    var email = (response as string).Trim();
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    Match match = regex.Match(email);
                    if (!match.Success)
                    {
                        result.Feedback = "Email is incorrect";
                        result.IsValid = false;
                    }
                    return result;
                }
                )
                .Field(nameof(PhoneNumber))
                .Field(nameof(JobTitle))
                .Field(nameof(Location))
                .Field(nameof(Salary))
                .Field(nameof(YearsOfXp))
                .Field(nameof(Age))
                .Field(nameof(Gender))
                //.OnCompletion(processOrder)
                .Build();
        }
    }
}