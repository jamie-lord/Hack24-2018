using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PorgBot.Data
{
    [Serializable]
    public class UserDetail
    {
        public enum GenderOptions
        {
            Male, Female, Other, Unspecified
        };

        public string Name { get; set; }
        public string Location { get; set; }
        public double Salary { get; set; }
        public int? YearsOfXp { get; set; }
        public int? Age { get; set; }
        public GenderOptions? Gender { get; set; }

        public static IForm<UserDetail> BuildForm()
        {
            return new FormBuilder<UserDetail>().Message("Hi! Welcome to PorgPowered salary bot").Build();
        }
    }
}