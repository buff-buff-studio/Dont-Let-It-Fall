using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DLIFR.Interface
{
    public class ShopItemWidget : MonoBehaviour
    {
        public Shop shop;

        public int price;
        public int count;
        public int maxCount;

        public TMP_Text labelTitle;
        public TMP_Text labelPrice;
        public TMP_Text labelMaxCount;
        public TMP_Text labelCount;

        public Button buttonMinus;
        public Button buttonPlus;

        public void AddOne()
        {
            count ++;

            shop.UpdateInterface();
        }

        public void RemoveOne()
        {
            count --;

            shop.UpdateInterface();
        }

        public void UpdateInterface(int remainingMoney)
        {
            labelCount.text = $"{count}";
            buttonMinus.interactable = count > 0;
            buttonPlus.interactable = count < maxCount && remainingMoney >= price;
        }
    }
}