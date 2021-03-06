using System;
using System.Linq;
using System.Reflection;
using Ieedo.Utilities;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Ieedo.Test
{
    public class Test_DataManager : MonoBehaviour
    {
        [Button("PerformAllTests")]
        public void PerformAllTests()
        {
            Test_LoadDatabase();
            Test_SaveProfile();
            Test_LoadProfile();
        }

        [Button("Test_LoadDatabase")]
        private void Test_LoadDatabase()
        {
            Log.Info(MethodBase.GetCurrentMethod().Name);

            Statics.Data.LoadDatabase();
            Statics.Data.PrintAll<CategoryDefinition>();
            Statics.Data.PrintAll<ActivityDefinition>();
            Statics.Data.PrintAll<AssessmentQuestionDefinition>();
        }

        [Button("Test_CreateProfile")]
        private void Test_CreateProfile()
        {
            Log.Info(MethodBase.GetCurrentMethod().Name);
            Statics.Data.CreateNewProfile(new ProfileDescription()
            {
                Name = "TEST",
                Country = "en",
                Language = Language.English,
            });
        }

        [Button("Test_SaveProfile")]
        private void Test_SaveProfile()
        {
            Log.Info(MethodBase.GetCurrentMethod().Name);
            Statics.Data.SaveProfile();
        }

        [Button("Test_LoadProfile")]
        private void Test_LoadProfile()
        {
            Log.Info(MethodBase.GetCurrentMethod().Name);
            Statics.Data.LoadProfile();
        }


        [Button("Test_CreateCard")]
        private void Test_CreateCard()
        {
            Log.Info(MethodBase.GetCurrentMethod().Name);

            Statics.Cards.GenerateTestCards(1);
        }


        [Button("Test_DeleteAllCards")]
        private void Test_DeleteAllCards()
        {
            Log.Info(MethodBase.GetCurrentMethod().Name);

            Statics.Data.DeleteAllCardDefinitions();

            Log.Err(Statics.Data.CardDefinitions.ToJoinedString());
        }

        [Button("Test_AssignCard")]
        private void Test_AssignCard()
        {
            Log.Info(MethodBase.GetCurrentMethod().Name);

            Statics.Cards.AssignCard(0);
            Log.Info(Statics.Data.Profile.Cards.ToString());
        }


        [Button("PrintApplicationData")]
        public void PrintApplicationData()
        {
            Log.Info(MethodBase.GetCurrentMethod().Name);
            Statics.Data.PrintAll<CategoryDefinition>();
            Statics.Data.PrintAll<ActivityDefinition>();
            Statics.Data.PrintAll<AssessmentQuestionDefinition>();
            Log.Info("N cards found: " + Statics.Data.CardDefinitions.Count);
            Log.Info(Statics.Data.CardDefinitions.ToJoinedString());
        }

        [Button("PrintProfileState")]
        public void PrintProfileState()
        {
            Log.Info(MethodBase.GetCurrentMethod().Name);
            Log.Info(Statics.Data.Profile.ToString());
        }
    }
}
