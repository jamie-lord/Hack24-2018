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
            Transaction=1, Total, Tax, Trauma
        }

        public enum QuestionTwoEnum
        {
            Demonstrator=1, Instigator, Investigator, Terminator
        }

        public enum QuestionThreeEnum
        {
            Tables=1, Gables, Cables, Stables
        }

        public enum QuestionFourEnum
        {
            [Describe("Elbow room")]
            Elbow=1,
            [Describe("Foot rest")]
            FootRest,
            [Describe("Ear Hole")]
            EarHole,
            [Describe("Knee Lounge")]
            KneeLounge
        }

        public enum QuestionFiveEnum
        {
            Pan=1, Pin, Pen, Pun
        }

        [Prompt("Question 1: In the UK, VAT stands for value-added ...? {||}")]
        public QuestionOneEnum QuestionOne { get; set; }

        [Prompt("Question 2: Which character was first played by Arnold Schwarzenegger in a 1984 film? {||}")]
        public QuestionTwoEnum QuestionTwo { get; set; }

        [Prompt("Question 3: What may an electrician lay? {||}")]
        public QuestionThreeEnum QuestionThree { get; set; }

        [Prompt("Question 4: Which of these means adequate space for moving in? {||}")]
        public QuestionFourEnum QuestionFour { get; set; }

        [Prompt("How is a play on words commonly described? {||}")]
        public QuestionFiveEnum QuestionFive { get; set; }
        

        public static IForm<MillionaireGame> BuildForm()
        {
            IForm<MillionaireGame> formGame = new FormBuilder<MillionaireGame>()
                .Message("Let's play, \"Who wants to be a millionaire?\"")
                .Message("Oh the irony...")
                .Field(nameof(QuestionOne), validate: async (state, response) =>
                {
                    QuestionOneEnum choice = (QuestionOneEnum)response;

                    var result = new ValidateResult()
                    {
                        IsValid = choice == QuestionOneEnum.Tax,
                        Feedback = choice == QuestionOneEnum.Tax ? "That's the correct answer!" : "That's incorrect I'm afraid..",
                        Value = response
                    };

                    if (!result.IsValid)
                    {
                        throw new FormCanceledException("1", null);
                    }

                    return result;
                })
                .Field(nameof(QuestionTwo), validate: async (state, response) =>
                {
                    QuestionTwoEnum choice = (QuestionTwoEnum)response;

                    var result = new ValidateResult()
                    {
                        IsValid = choice == QuestionTwoEnum.Terminator,
                        Feedback = choice == QuestionTwoEnum.Terminator ? "You are doing well!" : "That's incorrect I'm afraid..",
                        Value = response
                    };

                    if (!result.IsValid)
                    {
                        throw new FormCanceledException("2", null);
                    }

                    return result;
                })
                .Field(nameof(QuestionThree), validate: async (state, response) =>
                {
                    QuestionThreeEnum choice = (QuestionThreeEnum)response;

                    var result = new ValidateResult()
                    {
                        IsValid = choice == QuestionThreeEnum.Cables,
                        Feedback = choice == QuestionThreeEnum.Cables ? "Well that answer was obvious. Congratulations!" : "That's incorrect I'm afraid..",
                        Value = response
                    };

                    if (!result.IsValid)
                    {
                        throw new FormCanceledException("3", null);
                    }

                    return result;
                })
                .Field(nameof(QuestionFour), validate: async (state, response) =>
                {
                    QuestionFourEnum choice = (QuestionFourEnum)response;

                    var result = new ValidateResult()
                    {
                        IsValid = choice == QuestionFourEnum.Elbow,
                        Feedback = choice == QuestionFourEnum.Elbow ? "We have a genius here! That's correct!" : "That's incorrect I'm afraid..",
                        Value = response
                    };

                    if (!result.IsValid)
                    {
                        throw new FormCanceledException("4", null);
                    }

                    return result;
                })
                .Field(nameof(QuestionFive), validate: async (state, response) =>
                {
                    QuestionFiveEnum choice = (QuestionFiveEnum)response;

                    var result = new ValidateResult()
                    {
                        IsValid = choice == QuestionFiveEnum.Pun,
                        Feedback = choice == QuestionFiveEnum.Pun ? "Well this has been pun. You've done really well!" : "That's incorrect I'm afraid..",
                        Value = response
                    };

                    if (!result.IsValid)
                    {
                        throw new FormCanceledException("5", null);
                    }

                    return result;
                })
                .AddRemainingFields()
                .Build();

            return formGame;
        }
    }
}
