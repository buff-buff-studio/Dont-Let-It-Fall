using System;
using System.Linq;
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

        public string[] accepts;

        public BuyShop buy;

        public bool Accepts(Cargo cargo, out int price)
        {
            bool accepts = this.accepts.Contains(cargo.type);

            price = accepts ? cargo.sellPrice : 0;
            return accepts;
        }
    }
}