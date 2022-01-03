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
    }
}