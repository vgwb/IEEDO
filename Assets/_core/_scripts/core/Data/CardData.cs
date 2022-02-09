using System;
using Newtonsoft.Json;

namespace Ieedo
{
    [Serializable]
    public class CardData
    {
        public uint UID;
        public uint DefID;
        public Timestamp CreationDate;
        public Timestamp ExpirationDate;
        public Timestamp CompletionDate;

        [JsonIgnore]
        public CardDefinition Definition => Statics.Data.Cards.Find(x => x.UID == DefID);

    }
}