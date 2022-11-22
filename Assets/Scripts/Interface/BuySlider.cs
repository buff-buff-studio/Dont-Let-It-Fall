using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DLIFR.Data;

namespace DLIFR.Interface
{
    public class BuySlider : MonoBehaviour
    {
        public Slider[] sliders;
        public RectTransform[] fillAreas;
        public TMP_Text[] fillValues;

        public Value<int> maxValue;

        private void OnEnable() 
        {
            float f = 1f/(sliders.Length + 1);
            
            for(int i = 0; i < sliders.Length; i ++)
            {   
                Slider slider = sliders[i];
                slider.maxValue = 1;
                slider.value = f * (i + 1);
            }
        }

        private void Update()
        {
            RectTransform transform = this.transform as RectTransform;

            float width = transform.sizeDelta.x;
            float height = transform.sizeDelta.y;

            float from = 0;

            for(int i = 0; i < fillAreas.Length; i ++)
            {
                float to = 1f;

                if(i < sliders.Length)
                {
                    Slider slider = sliders[i];

                    to = sliders[i].value;
                
                    if(to < from)
                    {
                        to = sliders[i].value = from;
                    }
                    else if(i < sliders.Length - 1)
                    {
                        float nextValue = sliders[i + 1].value;

                        if(to > nextValue)
                        {
                            to = sliders[i].value = nextValue;
                        }
                    }   
                }

                RectTransform fillAreaIn = fillAreas[i];
                fillAreaIn.sizeDelta = new Vector2((to - from) * width , height);
                fillAreaIn.anchoredPosition = new Vector2(from * width, 0);       
                
                fillValues[i].text = $"{Mathf.RoundToInt((to - from) * maxValue)}";

                from = to;             
            }

            /*
            RectTransform fillArea = fillAreas[fillAreas.Length - 1];
            fillArea.sizeDelta = new Vector2((1 - from) * width , height);
            fillArea.anchoredPosition = new Vector2(from * width, 0);
            fillValues[fillAreas.Length - 1].text = $"{(1f - from) * maxValue}";       
            */
        }
    }
}