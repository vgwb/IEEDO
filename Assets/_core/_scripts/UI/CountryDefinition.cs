using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu(fileName = "Country", menuName = "Ieedo/CountryData")]
    public class CountryData : ScriptableObject, IDefinition
    {
        public int ID;
        public Sprite Flag;
        public string Code;
        public int Id => ID;
    }
}
