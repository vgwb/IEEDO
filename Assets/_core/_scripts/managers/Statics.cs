using Ieedo.Utilities;

namespace Ieedo
{
    public class Statics : SingletonMonoBehaviour<Statics>
    {
        private static DataManager data; public static DataManager Data => data ??= new DataManager();
        private static CardsManager cards; public static CardsManager Cards => cards ??= new CardsManager();
        private static ScreensManager screens; public static ScreensManager Screens => screens ??= new ScreensManager();
        private static InputManager input; public static InputManager Input => input ??= FindObjectOfType<InputManager>();

        private static AppManager app; public static AppManager App => app ??= FindObjectOfType<AppManager>();
        private static ScoreManager score; public static ScoreManager Score => score ??= FindObjectOfType<ScoreManager>();
        private static ModeManager mode; public static ModeManager Mode => mode ??= FindObjectOfType<ModeManager>();
        private static SessionFlowManager sessionFlow; public static SessionFlowManager SessionFlow => sessionFlow ??= FindObjectOfType<SessionFlowManager>();
        private static ActivityFlowManager activityFlow; public static ActivityFlowManager ActivityFlow => activityFlow ??= FindObjectOfType<ActivityFlowManager>();
        private static AnalyticsService analytics; public static AnalyticsService Analytics => analytics ??= FindObjectOfType<AnalyticsService>();
        private static NotificationService notifications; public static NotificationService Notifications => notifications ??= FindObjectOfType<NotificationService>();
    }
}
