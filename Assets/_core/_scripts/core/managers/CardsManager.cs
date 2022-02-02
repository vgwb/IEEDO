namespace Ieedo
{
    public class CardsManager
    {
        /// <summary>
        /// Assigns a card with the given ID to the current profile
        /// </summary>
        public void AssignCard(uint cardID)
        {
            Statics.Data.Profile.Cards.Add(new CardData
            {
                CardDefID = cardID,
                CreationDate = Timestamp.Now
            });
            Statics.Data.SaveProfile();
        }

        public void DeleteCard(CardDefinition cardDef)
        {
            Statics.Data.Cards.Remove(cardDef);
        }

        // TODO: use CardData instead here
        public CardDefinition GenerateCard(CardDefinition def)
        {
            Statics.Data.AddCardDefinition(def);
            return def;
        }

        public CardDefinition GenerateEmptyCard()
        {
            var card = new CardDefinition();
            Statics.Data.AddCardDefinition(card);
            return card;
        }
    }
}