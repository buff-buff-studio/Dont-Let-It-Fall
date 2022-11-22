using System;
using UnityEngine;

namespace DLIFR.Data
{
    [CreateAssetMenu(fileName = "Shop", menuName = "DLIFR/Shop", order = 0)]
    public class Shop : ScriptableObject 
    {
        [Serializable]
        public class ShopEntry
        {
            public int type;
            public string unlocalizedName;

            public int price;
        }

        public ShopEntry[] entries;
    }
}