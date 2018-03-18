using AdaptiveCards;
using SqlDataStore.Models;
using System;
using System.Collections.Generic;

namespace LuisBot.Extensions
{
    public static class UnsortedExtensions
    {
        public static AdaptiveCard CreateAdaptiveCard(this CreditCard creditCard)
        {
            AdaptiveCard card = new AdaptiveCard()
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveContainer()
                    {
                        SelectAction = new AdaptiveOpenUrlAction()
                        {
                            Url = new Uri(creditCard.Link)
                        },
                        Items = new List<AdaptiveElement>
                        {
                            new AdaptiveTextBlock(creditCard.Name) {Size = AdaptiveTextSize.Large, Color = AdaptiveTextColor.Dark, Weight = AdaptiveTextWeight.Bolder},
                            new AdaptiveImage(creditCard.ImageUrl) {Size = AdaptiveImageSize.Large, Spacing = AdaptiveSpacing.Padding},
                            new AdaptiveTextBlock("Annual fee: **£" +  creditCard.AnnualFee.ToString() + "**"),
                            new AdaptiveColumnSet() {Columns = new List<AdaptiveColumn>
                            {
                                new AdaptiveColumn() {Items = new List<AdaptiveElement>
                                {
                                    new AdaptiveTextBlock("Purchase APR: **" + creditCard.PurchaseApr.ToString() + "%**")
                                }},
                                new AdaptiveColumn()
                                {
                                    Items = new List<AdaptiveElement>
                                    {
                                        new AdaptiveImage("https://images.experian.co.uk/rebrand//experian_full_colour.svg") {Size = AdaptiveImageSize.Medium, Spacing = AdaptiveSpacing.Padding, HorizontalAlignment = AdaptiveHorizontalAlignment.Right}
                                    },

                                }
                            }}
                        }
                    }
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveOpenUrlAction()
                    {
                        Url = new Uri(creditCard.Link),
                        Title = "See more details"
                    }
                }
            };
            return card;
        }
    }
}