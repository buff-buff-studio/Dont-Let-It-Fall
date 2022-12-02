using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DLIFR.Data;
using DLIFR.Interface.Widgets;

namespace DLIFR.Game
{
    public class GameHUD : MonoBehaviour
    {
        [Header("REFERENCES")]
        public GameMatch match;
        public RectTransform sellItems;
        public Image fuelFill;
        public Image timeClock;

        [Header("SETTINGS")]
        public Value<int> ticksPerDay = 50 * 24;

        [Header("STATE")]
        public Value<int> gameTicks;
        public Value<float> dayTime;
        public Value<int> dayNumber;
        public Value<float> shipFuelLevel;
        public Value<float> shipFuelMaxLevel;

        [Header("PREFABS")]
        public GameObject prefabSellItem;

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
                fuelFill.fillAmount = shipFuelLevel.value / shipFuelMaxLevel.value;
            };
            
            dayTime.variable.onChange += ClockUpdate;
        }

        public void UpdateShopWishlist()
        {
            foreach(Transform children in sellItems)
            {
                Destroy(children.gameObject);
            }   

            foreach(GameMatch.CargoType type in match.types)
            {
                if(match.nextShop.Accepts(type.type))
                {
                    GameObject widget = GameObject.Instantiate(prefabSellItem);
                    widget.transform.parent = sellItems;
                    widget.transform.localScale = Vector3.one;
                    
                    SellItem sellItem = widget.GetComponent<SellItem>();
                    sellItem.sprite.sprite = type.sprite;
                    sellItem.labelPrice.text = $"{type.value}";
                }
            }
        }
        
        private void ClockUpdate()
        {
            var time = dayTime.value/24;
            if (time >= .5f)
            {
                if (timeClock.fillClockwise)
                    timeClock.fillClockwise = false;

                timeClock.fillAmount = 1 - ((time - .5f) / .5f);
            }
            else
            {
                if (!timeClock.fillClockwise)
                    timeClock.fillClockwise = true;

                timeClock.fillAmount = (time) / .5f;
            }
        }
    }
}