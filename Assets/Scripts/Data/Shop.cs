using System;
using System.Linq;
using UnityEngine;
using DLIFR.Props;
using DLIFR.Game;

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

        public bool Accepts(GameMatch match, Cargo cargo, out int price)
        {
            bool accepts = Accepts(cargo.type);

            price = accepts ? cargo.bitCount * match.types.Where((a) => a.type == cargo.type).First().value : 0;
            return accepts;
        }

        public bool Accepts(string type)
        {
            return this.accepts.Contains(type);
        }
    }
}