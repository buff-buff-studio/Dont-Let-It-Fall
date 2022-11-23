using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DLIFR.Interface.Widgets
{
    public class ShopSlider : MonoBehaviour 
    {
        public TMP_Text value;
        public Slider slider;

        public void SetValue(float value, int coins)
        {
            slider.value = value;
            UpdateLabel(value, coins);
        }

        public void UpdateLabel(float value, int coins)
        {
            this.value.text = $"{Mathf.RoundToInt(value * coins)}";
        }
    }
}