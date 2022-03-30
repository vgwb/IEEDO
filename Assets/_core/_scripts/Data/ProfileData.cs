using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool HasCardsWithStatus(CardValidationStatus status)
        {
            return this.Count(x => x.Status == status) > 0;
        }

        public int NCardsWithStatus(CardValidationStatus status)
        {
            return this.Count(x => x.Status == status);
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
        public bool IsNewProfile;
    }

    #region Activities

    [Serializable]
    public class ActivitiesData : List<ActivityData>
    {
        public ActivityData GetActivityData(ActivityID id)
        {
            return this.FirstOrDefault(x => x.ID == id);
        }

    }

    [Serializable]
    public class ActivityData
    {
        public ActivityID ID;
        public bool Unlocked;
        public int CurrentLevel;
        public ActivityResults Results = new();
    }

    [Serializable]
    public class ActivityResults : List<ActivityResult>
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

    #endregion

    [Serializable]
    public class ProfileData
    {
        public ProfileDescription Description;

        // State
        public int Level;
        public int CurrentScore;

        public OnboardingState OnboardingState;
        public CategoryDataCollection Categories;
        public CardDataCollection Cards;
        public ActivitiesData ActivitiesData;

        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendLine($"{Description.Name}({Description.Country} - {Description.Language})");
            s.AppendLine(Categories.ToString());
            s.AppendLine(Cards.ToString());
            s.AppendLine(ActivitiesData.ToString());
            return s.ToString();
        }
    }
}