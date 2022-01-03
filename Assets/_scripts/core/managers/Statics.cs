using Ieedo.Utilities;

namespace Ieedo
{
    public class Statics : SingletonMonoBehaviour<Statics>
    {
        private static DataManager data; public static DataManager Data => data ??= new DataManager();
        private static CardsManager cards; public static CardsManager Cards => cards ??= new CardsManager();
    }
}