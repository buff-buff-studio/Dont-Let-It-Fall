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
        public RectTransform fuelFill;

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
                fuelFill.sizeDelta = new Vector2(100f * shipFuelLevel.value/shipFuelMaxLevel.value, 25f);  
            };
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
    }
}