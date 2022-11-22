using UnityEngine;
using UnityEngine.UI;
using DLIFR.Data;

namespace DLIFR.Game
{
    public class GameHUD : MonoBehaviour
    {
        [Header("SETTINGS")]
        public Value<int> ticksPerDay = 50 * 24;

        [Header("STATE")]
        public Value<int> gameTicks;
        public Value<float> dayTime;
        public Value<int> dayNumber;
        public Value<float> shipFuelLevel;
        public Value<float> shipFuelMaxLevel;

        public RectTransform fuelFill;

        private void OnEnable() 
        {
            gameTicks.variable.onChange += () =>
            {
                float time = (gameTicks * 24f)/(ticksPerDay);
                dayTime.value = time % 24;
                dayNumber.value = Mathf.FloorToInt(time / 24f) + 1;
            };

            shipFuelLevel.variable.onChange += () => 
            {
                fuelFill.sizeDelta = new Vector2(100f * shipFuelLevel.value/shipFuelMaxLevel.value, 25f);  
            };
        }
    }
}