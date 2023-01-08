using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

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
        public LocString Title;
        public LocString Description;
        public int Difficulty;
        public CategoryID Category;
        public SubCategoryID SubCategory;

        [JsonIgnore]
        public string Icon
        {
            get
            {
                if (SubCategoryDefinition == null)
                    return string.Empty;
                return SubCategoryDefinition.Icon;
            }
        }

        [JsonIgnore]
        public CategoryDefinition CategoryDefinition => Statics.Data.Get<CategoryDefinition>((int)Category);

        [JsonIgnore]
        public SubCategoryDefinition SubCategoryDefinition
        {
            get
            {
                if (CategoryDefinition == null)
                    return null;
                return CategoryDefinition.SubCategories.FirstOrDefault(x => x.ID == SubCategory);
            }
        }

        public override string ToString()
        {
            return $"{Title.Text} [{Category}-{SubCategory}]";
        }
    }
}
