using System;
using System.Collections.Generic;
using System.Text;

namespace Ieedo
{
    [Serializable]
    public class OnboardingState
    {

    }

    [Serializable]
    public class CardDataCollection : List<CardData>
    {
        public override string ToString()
        {
            var s = new StringBuilder();
            foreach (var data in this)
            {
                s.AppendLine(data.ToString());
            }
            return s.ToString();
        }
    }

    [Serializable]
    public class CategoryDataCollection : List<CategoryData>
    {
        public override string ToString()
        {
            var s = new StringBuilder();
            foreach (var data in this)
            {
                s.AppendLine(data.ToString());
            }
            return s.ToString();
        }
    }

    [Serializable]
    public class ProfileDescription
    {
        public string Name;
        public string Country;
        public Language Language;
    }

    [Serializable]
    public class ProfileData
    {
        public ProfileDescription Description;

        // State
        public OnboardingState OnboardingState;
        public int Level;
        public CategoryDataCollection Categories;
        public CardDataCollection Cards;

        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendLine($"{Description.Name}({Description.Country} - {Description.Language})");
            s.AppendLine(Categories.ToString());
            s.AppendLine(Cards.ToString());
            return s.ToString();
        }
    }
}