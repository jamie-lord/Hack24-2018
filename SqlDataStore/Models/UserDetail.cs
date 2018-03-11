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
        [Prompt("Now I need your email address?")]
        public string Email { get; set; }
        [Prompt("What is your phone number?")]
        public double PhoneNumber { get; set; }
        [Prompt("What's your job title?")]
        public string JobTitle { get; set; }
        [Prompt("Where is your work place?")]
        public string Location { get; set; }
        [Prompt("How much do you earn?")]
        public double Salary { get; set; }
        [Optional]
        [Prompt("How long have you worked there?")]
        [Describe("Years of Experience")]
        [Template(TemplateUsage.NoPreference, "Skip")]
        public int? YearsOfXp { get; set; }
        [Optional]
        [Prompt("How old are you?")]
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