using System;
using System.Collections.Generic;
using UnityEngine;
using DLIFR.Data;
using DLIFR.Entities;
using DLIFR.Props;
using DLIFR.Interface;
using DLIFR.Interface.Widgets;

namespace DLIFR.Game
{
    public class GameMatch : MonoBehaviour
    {   
        [Serializable]
        public class CargoBitEntry
        {
            public float chance = 0.75f;
            public GameObject[] prefabs;
        }

        //If a item is not valid for selling, the game just takes with and do not pays you
        public const bool GAME_SELLS_EVERYTHING = true;
        //If you do a wrong split on the shop, the game keeps  you the remainder
        public const bool GAME_GIVES_BACK_MONEY = false;

        [Header("REFERENCES")]
        public Transform ship;
        public Area sellArea;
        public Canvas canvas;
        public GameShop gameShop;

        [Header("SETTINGS")]
        public Value<int> ticksPerDay = 50 * 24;
        public LayerMask selectableMask;
        public Value<float> fuelConsumptionRate = 1;

        [Header("STATE")]
        public Crewmate currentCrewmate;
        public Value<int> coinCount;
        public Value<int> gameTicks;
        public Value<float> shipFuelLevel;
        public Value<float> shipFuelMaxLevel;

        public Value<bool> isOnShop = false;
        public Value<bool> isPaused = false;

        public Value<bool> shouldTimePass;

        public Shop nextShop;

        [Header("PREFABS")]
        public GameObject prefabBird;
        public GameObject sellDisplay;
        public GameObject[] prefabFuelBox;
        public CargoBitEntry[] prefabCargoBox;
        public GameObject prefabCrewmate;

        private void Awake() 
        {
            gameTicks.value = 0;
            coinCount.value = 0;
            shipFuelLevel.value = shipFuelMaxLevel.value;

            isPaused.value = false;
            shouldTimePass.value = true;
        }

        private void OnEnable() 
        {
            isPaused.variable.onChange += () => 
            {
                Time.timeScale = isPaused ? 0 : 1f;
                Time.fixedDeltaTime = isPaused ? 0 : 0.02f;
            };
        }

        private void Update() 
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                isPaused.value = !isPaused.value;
            }

            bool m0 = Input.GetMouseButtonUp(0);
            bool m1 = Input.GetMouseButtonUp(1);

            if(m0 || m1)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(UnityEngine.Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 30f, selectableMask))
                {            
                    IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                    interactable?.OnInteract(m0 ? 0 : 1, this);

                    if(currentCrewmate != null && hit.collider.gameObject.tag == "Ground")
                    {
                        currentCrewmate.OnClickOnGround(hit.point);
                    }
                }

                UpdateInteractionDisplay();
            }  
        }

        public void UpdateInteractionDisplay()
        {
            InteractableBehaviour.UpdateInteractionDisplay(true, this);
        }

        private void FixedUpdate() 
        {
            if(isOnShop.value || isPaused.value || !shouldTimePass.value)
                return;

            shipFuelLevel.value -= Time.fixedDeltaTime * fuelConsumptionRate;

            if((++gameTicks.value) % ticksPerDay == 0)
            {
                OpenShop();
            }
        }

        #region Birds
        public DeliveryBird SpawnBird(GameObject carryingPrefab)
        {
            GameObject bird = GameObject.Instantiate(prefabBird);
            GameObject carrying = GameObject.Instantiate(carryingPrefab);

            DeliveryBird db = bird.GetComponent<DeliveryBird>();
            db.carrying = carrying.transform;
            db.RefreshCarrying();

            Cargo cargo = carrying.GetComponent<Cargo>();
            cargo.transform.parent = ship;

            return db;
        }
        #endregion

        public void OpenShop()
        {
            List<Cargo> keep = new List<Cargo>();

            int coins = 0;

            foreach(Cargo cargo in sellArea.cargoes)
            {
                if(nextShop.Accepts(cargo, out int price) || GAME_SELLS_EVERYTHING)
                {
                    ShowSellDisplay(cargo.transform.position, price);
                    GameObject.Destroy(cargo.gameObject);
                    coins += price;
                }
                else
                {
                    keep.Add(cargo);
                }
            }

            coinCount.value += coins;

            sellArea.cargoes.Clear();
            sellArea.cargoes.AddRange(keep);

            gameShop.gameObject.SetActive(true);
            isOnShop = true;
        }

        public void CloseShop()
        {
            #region Input Data
            int fuelValue = gameShop.slider.GetValue(0);
            int cargoValue = gameShop.slider.GetValue(1);
            int recruitValue = gameShop.slider.GetValue(2);
            int keepValue = gameShop.slider.GetValue(3);
            #endregion

            #region Calculation
            if(!GAME_GIVES_BACK_MONEY)
                coinCount.value = keepValue;

            int spentMoney = 0;

            //Buy the things
            Shop shop = nextShop;
            int fuelBoxPrice = shop.buy.fuelBoxPrice;
            int cargoBitPrice = shop.buy.cargoBitPrice;
            int recruitPrice = shop.buy.crewmateRecruitPrice;

            int fuelBoxes = fuelValue/fuelBoxPrice;
            int cargoBits = cargoValue/cargoBitPrice;
            int crewmates = recruitValue/recruitPrice;

            spentMoney = (fuelBoxes * fuelBoxPrice) + (cargoBits * cargoBitPrice) + (crewmates * recruitPrice);

            if(GAME_GIVES_BACK_MONEY)
                coinCount.value -= spentMoney;
            #endregion

            #region Delivery
            //Spawn fuel
            for(int i = 0; i < fuelBoxes; i ++)
                SpawnBird(GetRandom(prefabFuelBox));

            //Spawn cargo
            while(cargoBits > 0)
            {
                int lm = Mathf.Min(cargoBits - 1, prefabCargoBox.Length - 1);
                for(int i = lm; i >= 0; i --)
                {
                    CargoBitEntry entry = prefabCargoBox[i];

                    if(i == 0 || UnityEngine.Random.Range(0f, 0.999f) < entry.chance)
                    {
                        cargoBits -= lm + 1;
                       
                        SpawnBird(GetRandom(entry.prefabs));
                    }
                }
            }

            //Spawn crewmate
            for(int i = 0; i < crewmates; i ++)
            {
                SpawnBird(prefabCrewmate);
            }
            #endregion

            gameShop.gameObject.SetActive(false);
            isOnShop = false;
        }

        public void ShowSellDisplay(Vector3 position, int price)
        {
            GameObject go = GameObject.Instantiate(sellDisplay);
            go.transform.parent = canvas.transform;

            SellDisplay display = go.GetComponent<SellDisplay>();
            display.worldPosition = position;
            display.SetPrice(price);
        }

        public GameObject GetRandom(GameObject[] array)
        {
            return array[UnityEngine.Random.Range(0, array.Length)];
        }
    }
}