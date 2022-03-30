using System;
using System.Linq;

namespace Ieedo
{
    public class CardsManager
    {

        #region Held Cards

        /// <summary>
        /// Assigns a card with the given ID to the current profile
        /// </summary>
        public void AssignCard(uint cardID)
        {
            Statics.Data.Profile.Cards.Add(new CardData
            {
                DefID = cardID,
                CreationTimestamp = Timestamp.Now
            });
            Statics.Data.SaveProfile();
        }

        public void AddCard(CardData cardData)
        {
            Statics.Data.Profile.Cards.Add(cardData);
            Statics.Data.SaveProfile();
            Statics.Analytics.Card(cardData.Status.ToString());
        }

        /// <summary>
        /// Deletes a card with the given ID from the current profile
        /// </summary>
        public void DeleteCard(uint cardID)
        {
            Statics.Data.Profile.Cards.RemoveAll(x => x.UID == cardID);
            Statics.Data.SaveProfile();
        }

        public void DeleteCard(CardData cardData)
        {
            Statics.Data.Profile.Cards.Remove(cardData);
            Statics.Data.SaveProfile();
        }

        #endregion


        #region Card Definitions

        public void DeleteCardDefinition(CardDefinition cardDef)
        {
            Statics.Data.CardDefinitions.Remove(cardDef);
        }

        public CardDefinition GenerateCardDefinition(CardDefinition def, bool isDefaultCard = false)
        {
            if (Statics.Data.CardDefinitions.Count == 0)
                def.UID = 1;
            else
                def.UID = Statics.Data.CardDefinitions.Max(x => x.UID) + 1;
            Statics.Data.AddCardDefinition(def, isDefaultCard);
            return def;
        }

        public CardDefinition GenerateEmptyCard()
        {
            var card = new CardDefinition();
            Statics.Data.AddCardDefinition(card);
            return card;
        }

        #endregion

        #region Test

        public void GenerateTestCards(int nToGenerate)
        {
            Statics.Data.LoadCardDefinitions();
            for (int i = 0; i < nToGenerate; i++)
            {
                var category = (CategoryID)UnityEngine.Random.Range(1, Enum.GetValues(typeof(CategoryID)).Length);
                var subcategory = (SubCategoryID)((int)category * 100 + UnityEngine.Random.Range(0, 3));
                var cardDef = Statics.Cards.GenerateCardDefinition(
                    new CardDefinition
                    {
                        Category = category,
                        SubCategory = subcategory,
                        Description = new LocString { DefaultText = "Card Description" },
                        Difficulty = UnityEngine.Random.Range(1, 4),
                        Title = new LocString { DefaultText = "Card Title" },
                    },
                    isDefaultCard: AppManager.I.ApplicationConfig.SaveCardsAsDefault
                );

                // Create a new Data for this profile for that card
                var cardData = new CardData
                {
                    DefID = cardDef.UID,
                    CreationTimestamp = new Timestamp(DateTime.Now),
                    ExpirationTimestamp = new Timestamp(DateTime.Now + TimeSpan.FromDays(UnityEngine.Random.Range(0, 12))),
                    Status = CardValidationStatus.Todo,
                };
                Statics.Data.Profile.Cards.Add(cardData);
            }
            Log.Err($"{nToGenerate} card(s) generated");
        }

        #endregion
    }
}
