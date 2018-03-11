using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Bot.Sample.LuisBot
{
    [Serializable]
    public class MillionaireGame
    {
        public enum QuestionOneEnum
        {
            Transaction, Total, Tax, Trauma
        }

        [Prompt("Question 1: In the UK, VAT stands for value-added ...? {||}")]
        public QuestionOneEnum QuestionOne { get; set; }
        public static IForm<MillionaireGame> BuildForm()
        {
            IForm<MillionaireGame> formGame = new FormBuilder<MillionaireGame>()
                .Message("Let's play, \"Who wants to be a millionaire?\"")
                .Message("Oh the irony...")
                .Field(nameof(QuestionOne), validate: (state, response) =>
                {
                    QuestionOneEnum choice = (QuestionOneEnum)response;

                    var result = new ValidateResult()
                    {
                        IsValid = choice == QuestionOneEnum.Tax,
                        Feedback = choice == QuestionOneEnum.Tax ? " That's the correct answer!" : "That's incorrect I'm afraid..",
                    };

                    if (!result.IsValid)
                    {
                        throw new FormCanceledException("1", null);
                    }

                    return Task.FromResult(result);
                })
                .Build();

            return formGame;
        }
    }
}
