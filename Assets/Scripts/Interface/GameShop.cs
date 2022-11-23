using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DLIFR.Data;
using DLIFR.Interface.Widgets;

namespace DLIFR.Interface
{
    public class GameShop : MonoBehaviour
    {
        public Value<int> coins;

        public CanvasGroup canvasGroup;
        public Shop shop;

        public ShopSlider[] sliders;

        private void Update()
        {
            float valueSum = 0;
            GameObject obj = EventSystem.current.currentSelectedGameObject;

            for(int i = 0; i < sliders.Length - 1; i ++)
            {
                ShopSlider slider = sliders[i];
                valueSum += slider.slider.value;  

                slider.UpdateLabel(slider.slider.value, coins);
            }

            if(valueSum > 1)
            {
                for(int i = 0; i < sliders.Length - 1; i ++)
                {
                    ShopSlider slider = sliders[i];

                    if(obj != null && obj == slider.slider.gameObject)
                        continue;

                    float toRem = Math.Min(slider.slider.value, valueSum - 1f);
                    slider.SetValue(slider.slider.value - toRem, coins);
                    valueSum -= toRem;

                    if(valueSum <= 1f)
                        break;
                }
            }

            sliders[sliders.Length - 1].SetValue(1f - valueSum, coins);
        }

        public int GetValue(int section)
        {
            return Mathf.RoundToInt(sliders[section].slider.value * coins);
        }
    }
}