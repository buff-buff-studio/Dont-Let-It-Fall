using System;
using UnityEngine;
using TMPro;
using DLIFR.Data;

namespace DLIFR.Interface
{
    public class Shop : MonoBehaviour
    {
        public Value<float> checkPointDuration;
        public float startGameTime;
        public Value<float> gameTime;

        public Value<int> coins;

        [Serializable]
        public class ShopItem 
        {
            public GameObject[] itemPrefab;
           // public LootTable prefabs;

            public string name;
            public int price;
            public int maxCount;

            public GameObject GetPrefab()
            {
                return itemPrefab[UnityEngine.Random.Range(0, itemPrefab.Length)];
            }
        }

        public ShopItem[] items;
        public ShopItemWidget[] widgets;
        
        public GameObject prefabItem;
        public Transform itemHolder;
        public TMP_Text labelRemainingCoins;
        public TMP_Text labelRemainingTime;
        
        public void OnEnable()
        {
            startGameTime = gameTime.value;

            widgets = new ShopItemWidget[items.Length];
            int i = 0;
            foreach(ShopItem item in items)
            {
                ShopItemWidget widget = GameObject.Instantiate(prefabItem).GetComponent<ShopItemWidget>();
                widget.transform.parent = itemHolder;
                widget.shop = this;
                
                widget.price = item.price;
                widget.maxCount = item.maxCount;

                widget.labelTitle.text = item.name;
                widget.labelPrice.text = $"{item.price}$";
                widget.labelMaxCount.text = $"Max: {widget.maxCount}";

                widgets[i] = widget;
                i ++;
            }

            UpdateInterface();
        }

        private void OnDisable() 
        {
            foreach (Transform child in itemHolder) 
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void UpdateInterface()
        {
            int remainingMoney = coins.value;
            foreach(ShopItemWidget widget in widgets)
                remainingMoney -= widget.count * widget.price;

            foreach(ShopItemWidget widget in widgets)
                widget.UpdateInterface(remainingMoney);

            labelRemainingCoins.text = $"Remaning Coins: {remainingMoney}";
        }

        private void FixedUpdate() 
        {
            float duration = checkPointDuration.value - (gameTime.value - startGameTime);

            labelRemainingTime.text = $"Closing in {duration:F1}";
        }
    }
}