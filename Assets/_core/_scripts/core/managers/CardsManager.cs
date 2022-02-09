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

        public void AssignCard(CardData cardData)
        {
            Statics.Data.Profile.Cards.Add(cardData);
            Statics.Data.SaveProfile();
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
            if (Statics.Data.CardDefinitions.Count == 0) def.UID = 1;
            else def.UID = Statics.Data.CardDefinitions.Max(x => x.UID) + 1;
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
    }
}