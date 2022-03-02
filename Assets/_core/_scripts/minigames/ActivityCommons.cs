using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ieedo.games
{
    public class ActivityCommons : MonoBehaviour
    {
        public static ActivityCommons I;
        public GameObject StaticsPrefab;

        public void Awake()
        {
            var statics = FindObjectOfType<Statics>();
            if (statics == null)
                Instantiate(StaticsPrefab, transform);

            if (I == null)
                I = this;

        }

    }
}
