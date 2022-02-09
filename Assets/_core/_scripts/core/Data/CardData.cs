using System;
using Newtonsoft.Json;

namespace Ieedo
{
    [Serializable]
    public class CardData
    {
        public uint UID;
        public uint DefID;
        public Timestamp CreationTimestamp;
        public Timestamp ExpirationTimestamp;
        public Timestamp ValidationTimestamp;

        [JsonIgnore]
        public CardDefinition Definition => Statics.Data.Cards.Find(x => x.UID == DefID);

        #region Timing

        public int DaysLeft => (ExpirationTimestamp.Date - DateTime.Now).Days;
        public bool IsDueToday => DaysLeft == 0;
        public bool IsDueTomorrow => DaysLeft == 1;
        public bool IsExpired => DaysLeft < 0;

        #endregion
    }
}