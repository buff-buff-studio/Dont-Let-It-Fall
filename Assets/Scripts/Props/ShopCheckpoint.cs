using System;
using System.Collections.Generic;
using UnityEngine;
using DLIFR.Data;

namespace DLIFR.Props
{
    public class ShopCheckpoint : MonoBehaviour
    {
        public Value<int> gameTicks;
        public Value<int> gameTicksPerDay;

        public float center = -5f;
        public float offset = 20f;
        public float depth = 8f;

        public AnimationCurve valueCurve;

        private void Update() 
        {
            int gameTicks = this.gameTicks.value;
            int gameTicksPerDay = this.gameTicksPerDay.value;

            float f = gameTicks%gameTicksPerDay;
            
            float p = valueCurve.Evaluate(f / gameTicksPerDay);

            if(gameTicks < gameTicksPerDay/2f)
                p = -1;

            transform.position = new Vector3(offset * p, 0, depth);
        }

    }
}