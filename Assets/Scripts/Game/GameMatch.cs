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

        [Header("STATE")]
        public Value<int> coinCount;
        public Value<int> gameTicks;
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
        }

        private void Update() 
        {
            if(Input.GetKeyDown(KeyCode.B))
            {
                SpawnBird(testCargo);
            }
        }

        private void FixedUpdate() 
        {
            if(isOnShop.value)
                return;

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