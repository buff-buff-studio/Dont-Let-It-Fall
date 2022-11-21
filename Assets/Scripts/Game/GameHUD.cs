using UnityEngine;
using DLIFR.Data;

namespace DLIFR.Game
{
    public class GameHUD : MonoBehaviour
    {
        public Value<int> ticksPerDay = 50 * 24;

        public Value<int> gameTicks;

        public Value<float> dayTime;
        public Value<int> dayNumber;

        private void OnEnable() 
        {
            gameTicks.variable.onChange += () =>
            {
                float time = (gameTicks * 24f)/(ticksPerDay);
                dayTime.value = time % 24;
                dayNumber.value = Mathf.FloorToInt(time / 24f) + 1;
            };
        }
    }
}