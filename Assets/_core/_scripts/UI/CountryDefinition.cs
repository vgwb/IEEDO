using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu(fileName = "Country", menuName = "Ieedo/CountryDefinition")]
    public class CountryDefinition : ScriptableObject, IDefinition
    {
        public int ID;
        public Sprite Flag;
        public string Code;
        public int Id => ID;
    }
}
