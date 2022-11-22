using System;
using UnityEngine;
using DLIFR.Props;

namespace DLIFR.Data
{
    

    [CreateAssetMenu(fileName = "Shop", menuName = "DLIFR/Shop", order = 0)]
    public class Shop : ScriptableObject 
    {
        [Serializable]
        public class BuyShop
        {
            public int fuelBoxPrice = 10;
            public int cargoBitPrice = 10;
            public int crewmateRecruitPrice = 30;
        }

        [Serializable]
        public class SellShop
        {

        }

        public BuyShop buy;
        public SellShop sell;

        public bool Accepts(Cargo cargo, out int price)
        {
            price = 15;
            return true;
        }
    }
}