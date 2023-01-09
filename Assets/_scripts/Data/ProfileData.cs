using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Serialization;

namespace Ieedo
{

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

        public bool HasCardsWithStatus(CardStatus status)
        {
            return this.Count(x => x.Status == status) > 0;
        }

        public int NCardsWithStatus(CardStatus status)
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
    public class AppSettings
    {
        public bool TutorialNotCompleted;
        public string HostCountryLocale;
        public string NativeLocale;
        public bool SfxDisabled;
        public bool NotificationsDisabled;
        public bool AnalyticsDisabled;
    }

    #region Activities

    [Serializable]
    public class ActivitiesDataCollection : List<ActivityData>
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
        public int HiScore;
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
        public int Version;
        public AppSettings Settings;

        // State
        public int CurrentPoints;
        public CategoryDataCollection Categories;
        public CardDataCollection Cards;
        public ActivitiesDataCollection Activities;

        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendLine($"({Settings.HostCountryLocale} - {Settings.NativeLocale})");
            s.AppendLine($"Points: {CurrentPoints}");
            s.AppendLine(Categories.ToString());
            s.AppendLine(Cards.ToString());
            s.AppendLine(Activities.ToString());
            return s.ToString();
        }
    }
}
