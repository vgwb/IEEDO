using Ieedo.Utilities;

namespace Ieedo
{
    public class Statics : SingletonMonoBehaviour<Statics>
    {
        private static DataManager data; public static DataManager Data => data ??= new DataManager();
        private static CardsManager cards; public static CardsManager Cards => cards ??= new CardsManager();
        private static ScreensManager screens; public static ScreensManager Screens => screens ??= new ScreensManager();
        private static AppManager app; public static AppManager App => app ??= FindObjectOfType<AppManager>();
        private static ScoreManager score; public static ScoreManager Score => score ??= FindObjectOfType<ScoreManager>();
        private static AssessmentFlowManager assessmentFlow; public static AssessmentFlowManager AssessmentFlow => assessmentFlow ??= FindObjectOfType<AssessmentFlowManager>();
        private static ActivityFlowManager activityFlow; public static ActivityFlowManager ActivityFlow => activityFlow ??= FindObjectOfType<ActivityFlowManager>();

    }
}