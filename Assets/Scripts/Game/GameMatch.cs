using System.Collections.Generic;
using UnityEngine;
using DLIFR.Data;
using DLIFR.Entities;
using DLIFR.Props;
using DLIFR.Interface.Widgets;

namespace DLIFR.Game
{
    public class GameMatch : MonoBehaviour
    {   
        //If a item is not valid for selling, the game just takes with and do not pays you
        public const bool GAME_SELLS_EVERYTHING = true;
        //If you do a wrong split on the shop, the game keeps  you the remainder
        public const bool GAME_GIVES_BACK_MONEY = false;

        [Header("REFERENCES")]
        public Transform ship;
        public Area sellArea;
        public Canvas canvas;

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

        public Shop nextShop;

        [Header("PREFABS")]
        public GameObject prefabBird;
        public GameObject testCargo;
        public GameObject sellDisplay;

        private void Awake() 
        {
            gameTicks.value = 0;
            coinCount.value = 0;
            shipFuelLevel.value = shipFuelMaxLevel.value;

            isPaused.value = false;
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
            if(Input.GetKeyDown(KeyCode.B))
            {
                SpawnBird(testCargo);
            }

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
            if(isOnShop.value || isPaused.value)
                return;

            shipFuelLevel.value -= Time.fixedDeltaTime * fuelConsumptionRate;

            if((++gameTicks.value) % ticksPerDay == 0)
            {
                isOnShop = true;
                OnOpenShop();
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

        public void OnOpenShop()
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
        }

        public void ShowSellDisplay(Vector3 position, int price)
        {
            GameObject go = GameObject.Instantiate(sellDisplay);
            go.transform.parent = canvas.transform;

            SellDisplay display = go.GetComponent<SellDisplay>();
            display.worldPosition = position;
            display.SetPrice(price);
        }
    }
}