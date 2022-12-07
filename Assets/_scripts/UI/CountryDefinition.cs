using UnityEngine;

namespace Ieedo
{
    [CreateAssetMenu(fileName = "Country", menuName = "Ieedo/CountryDefinition")]
    public class CountryDefinition : ScriptableObject, IDefinition
    {
        public Sprite Flag;
        public string Code;
        public int Id => 0;
    }
}
