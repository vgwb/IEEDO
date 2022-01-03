using System.Linq;
using System.Reflection;
using Ieedo.Utilities;
using UnityEngine;

namespace Ieedo.Test
{
    public class Test_DataManager : MonoBehaviour
    {
        [ContextMenu("PerformAllTests")]
        public void PerformAllTests()
        {
            Test_LoadDatabase();
            Test_SaveProfile();
            Test_LoadProfile();
        }

        [ContextMenu("Test_LoadDatabase")]
        private void Test_LoadDatabase()
        {
            Log.Err(MethodBase.GetCurrentMethod().Name);

            Statics.Data.LoadDatabase();
            Statics.Data.PrintAll<CategoryDefinition>();
            Statics.Data.PrintAll<ActivityDefinition>();
        }

        [ContextMenu("Test_CreateProfile")]
        private void Test_CreateProfile()
        {
            Log.Err(MethodBase.GetCurrentMethod().Name);
            Statics.Data.CreateNewProfile(new ProfileDescription()
            {
                Name = "TEST",
                Country = "en",
                Language = Language.English,
            });
        }

        [ContextMenu("Test_SaveProfile")]
        private void Test_SaveProfile()
        {
            Log.Err(MethodBase.GetCurrentMethod().Name);
            Statics.Data.SaveProfile();
        }

        [ContextMenu("Test_LoadProfile")]
        private void Test_LoadProfile()
        {
            Log.Err(MethodBase.GetCurrentMethod().Name);
            Statics.Data.LoadProfile();
        }


        [ContextMenu("Test_CreateCard")]
        private void Test_CreateCard()
        {
            Log.Err(MethodBase.GetCurrentMethod().Name);

            Statics.Data.LoadCards();
            Statics.Data.AddCardDefinition(new CardDefinition
            {
                Category = CategoryID.A,
                Description = new LocalizedString {DefaultText = "TEST " + Random.Range(0,9999)},
                Difficulty = 2,
                Title = new LocalizedString {DefaultText = "TITLE"},
            });

            Log.Err(Statics.Data.Cards.ToJoinedString());
        }

        [ContextMenu("Test_AssignCard")]
        private void Test_AssignCard()
        {
            Log.Err(MethodBase.GetCurrentMethod().Name);

            Statics.Cards.AssignCard(0);
            Log.Err(Statics.Data.Profile.Cards.ToString());
        }


        [ContextMenu("PrintApplicationData")]
        public void PrintApplicationData()
        {
            Statics.Data.PrintAll<CategoryDefinition>();
            Statics.Data.PrintAll<ActivityDefinition>();
            Statics.Data.PrintAll<AssessmentQuestionDefinition>();
            Log.Info(Statics.Data.Cards.ToJoinedString());
        }

        [ContextMenu("PrintProfileState")]
        public void PrintProfileState()
        {
            Log.Info(Statics.Data.Profile.ToString());
        }
    }
}