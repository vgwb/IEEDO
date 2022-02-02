using System;

namespace Ieedo
{
    [Serializable]
    public class CardData
    {
        public uint CardDefID;
        public Timestamp CreationDate;
        public Timestamp ExpirationDate;
        public Timestamp CompletionDate;
    }
}