using System;
using System.Collections.Generic;
using System.Linq;

namespace Ieedo
{
    [Serializable]
    public class CardsCollection
    {
        public List<CardDefinition> Cards = new();

        public void Add(CardDefinition card)
        {
            Cards.Add(card);
        }
    }

    [Serializable]
    public class CardDefinition
    {
        public uint UID;
        public LocalizedString Title;
        public LocalizedString Description;
        public uint Difficulty;
        public CategoryID Category;

        public override string ToString()
        {
            return Title.Text;
        }
    }
}