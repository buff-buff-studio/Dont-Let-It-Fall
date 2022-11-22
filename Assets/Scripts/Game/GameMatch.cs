using UnityEngine;
using UnityEngine.Events;
using DLIFR.Data;
using DLIFR.Entities;
using DLIFR.Props;

namespace DLIFR.Game
{
    public class GameMatch : MonoBehaviour
    {
        [Header("REFERENCES")]
        public Transform ship;

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

        [Header("PREFABS")]
        public GameObject prefabBird;
        public GameObject testCargo;

        [Header("HANDLERS")]
        public UnityEvent onOpenShop;

        private void Awake() 
        {
            gameTicks.value = 0;
            coinCount.value = 0;
            shipFuelLevel.value = shipFuelMaxLevel.value;
        }

        private void Update() 
        {
            if(Input.GetKeyDown(KeyCode.B))
            {
                SpawnBird(testCargo);
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
            Debug.Log("opa");
            InteractableBehaviour.UpdateInteractionDisplay(true, this);
        }

        private void FixedUpdate() 
        {
            if(isOnShop.value)
                return;

            shipFuelLevel.value -= Time.fixedDeltaTime * fuelConsumptionRate;

            if((++gameTicks.value) % ticksPerDay == 0)
            {
                isOnShop = true;
                onOpenShop?.Invoke();
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
    }
}