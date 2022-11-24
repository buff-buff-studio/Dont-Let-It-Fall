using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DLIFR.Data;
using DLIFR.Entities;
using DLIFR.Props;
using DLIFR.Interface;
using DLIFR.Interface.Widgets;
using DLIFR.Game.Tutorial;
using DLIFR.Audio;

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

        [Serializable]
        public class CargoType
        {
            public string type;
            public Sprite sprite;
            public int value;
        }

        [Serializable]
        public class Preset
        {
            public Transform[] crewmateSpawner;
            public Spawner[] boxSpawner;
        }

        //If a item is not valid for selling, the game just takes with and do not pays you
        public const bool GAME_SELLS_EVERYTHING = true;
        //If you do a wrong split on the shop, the game keeps  you the remainder
        public const bool GAME_GIVES_BACK_MONEY = true;

        [Header("REFERENCES")]
        public Ship ship;
        public Area sellArea;
        public Canvas canvas;
        public GameShop gameShop;
        public GameHUD gameHud;
        public GameTutorial gameTutorial;
        public GameObject pauseMenu;
        public Settings settings;
        public GameObject hudBindings;
        public CanvasGroup gameOverCanvas;

        [Header("SETTINGS")]
        public Value<int> ticksPerDay = 50 * 24;
        public LayerMask selectableMask;
        public LayerMask areaSelectableMask;
        public bool useOnlyAreas = false;
        public Value<float> fuelConsumptionRate = 1;
        public CargoType[] types;

        [Header("STATE")]
        public Crewmate currentCrewmate;
        public Value<int> coinCount;
        public Value<int> gameTicks;
        public Value<float> shipFuelLevel;
        public Value<float> shipFuelMaxLevel;

        public Value<bool> isOnShop = false;
        public Value<bool> isPaused = false;
        public Value<bool> showingTutorial = false;

        public Value<bool> shouldTimePass;

        public Shop nextShop;
        public Func<string, GameObject, bool> canDoAction;
        public bool wasPausedOnOpenPause = false;

        [Header("PREFABS")]
        public GameObject prefabBird;
        public GameObject sellDisplay;
        public GameObject[] prefabFuelBox;
        public CargoBitEntry[] prefabCargoBox;
        public GameObject prefabCrewmate;
        public Shop[] allShops;

        [Header("PRESETS")]
        public Transform[] crewmateSpawn;
        public Preset tutorialPreset;
        public Preset gamePreset;

        private void Awake() 
        {
            StartGame(settings.showTutorial);
            settings.showTutorial = false;
            settings.Save();
        }

        public void StartGame(bool tutorial)
        {
            AudioController.PlayMusic("music");
            
            hudBindings.SetActive(!tutorial);
            
            Rigidbody rb = ship.GetComponent<Rigidbody>();

            gameTicks.value = 0;
            coinCount.value = 0;
            shipFuelLevel.value = shipFuelMaxLevel.value;

            gameHud.dayNumber.value = 1;
            gameHud.dayTime.value = 0;

            isOnShop.value = false;
            gameShop.gameObject.SetActive(false);
            gameHud.gameObject.SetActive(true);

            showingTutorial.value = false;

            if(tutorial)
            {
                ship.HaveFuel = false;
                rb.isKinematic = true;

                showingTutorial.value = true;
                useOnlyAreas = false;
                isPaused.value = true;
                shouldTimePass.value = false;

                gameTutorial.gameObject.SetActive(true);

                LoadPreset(tutorialPreset);
            }
            else
            {
                ship.HaveFuel = true;
                rb.isKinematic = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                useOnlyAreas = false;
                isPaused.value = false;
                shouldTimePass.value = true;

                LoadPreset(gamePreset);
            }

            ChoseNewShop(!tutorial);
        }

        public void LoadPreset(Preset preset)
        {
            List<InteractableBehaviour> behaviours = new List<InteractableBehaviour>();

            foreach(var interactable in InteractableBehaviour.behaviours)
            {
                if(interactable is Crewmate || interactable is Cargo)
                {
                    behaviours.Add(interactable);
                }
            }

            foreach(var o in behaviours)
            {
                Destroy(o.gameObject);
            }

            foreach(Transform spawn in preset.crewmateSpawner)
            {
                GameObject.Instantiate(prefabCrewmate, spawn.transform.position, Quaternion.identity);
            }

            foreach(Spawner spawner in preset.boxSpawner)
                spawner.Spawn();
        }

        public void ChoseNewShop(bool random)
        {
            if(random)
                nextShop = allShops[UnityEngine.Random.Range(0, allShops.Length)];
            else
                nextShop = allShops[0];

            gameHud.UpdateShopWishlist();
        }

        private void OnEnable() 
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;

            isPaused.variable.onChange += () => 
            {
                Time.timeScale = isPaused ? 0 : 1f;
                Time.fixedDeltaTime = isPaused ? 0 : 0.02f;
            };

            shipFuelLevel.variable.onChange += () =>
            {
                if(showingTutorial) return;

                bool b = ship.HaveFuel = shipFuelLevel.value > 0;

                if(!b)
                {
                    shipFuelLevel.variable.onChange = null;
                    OnGameOver();
                }
            };
        }


        private void OnDisable() 
        {
            shipFuelLevel.variable.onChange = null;
            isPaused.variable.onChange = null;
        }

        private void Update() 
        {
            if(isOnShop)
            {
                gameShop.canvasGroup.alpha = Mathf.Lerp(
                    gameShop.canvasGroup.alpha,
                    1f,
                    Time.unscaledDeltaTime * 2f
                );
            }

            if(gameOverCanvas.gameObject.activeInHierarchy)
            {
                gameOverCanvas.alpha = Mathf.Lerp(
                    gameOverCanvas.alpha,
                    1f,
                    Time.unscaledDeltaTime
                );
            }
            else 
            {
                if(Input.GetKeyDown(KeyCode.P))
                {
                    TogglePaused();
                }
            }

            bool m0 = Input.GetMouseButtonUp(0);
            bool m1 = Input.GetMouseButtonUp(1);

            if((m0 || m1) && !isOnShop.value)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                LayerMask selectableMask = useOnlyAreas ? this.areaSelectableMask : this.selectableMask;
                
                Debug.Log("clicando sua putinha");

                if(UnityEngine.Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 30f, selectableMask))
                {    
                    Debug.Log("gghfhg");        
                    IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                    interactable?.OnInteract(m0 ? 0 : 1, this);
                    
                    if(currentCrewmate != null && hit.collider.gameObject.tag == "Ground")
                    {
                        AudioController.PlayAudio("ground_target");
                        currentCrewmate.OnClickOnGround(hit.point);
                    }
                }

                UpdateInteractionDisplay();
            }  
        }

        public void SetSelectedCrewmate(Crewmate crewmate)
        {
            if(currentCrewmate != null)
            {
                Crewmate.SetLayer(currentCrewmate.transform, 0);
            }

            currentCrewmate = crewmate;

            if(currentCrewmate != null)
            {        
                Crewmate.SetLayer(currentCrewmate.transform, LayerMask.NameToLayer("Selected"));
            }

            UpdateInteractionDisplay();
        }

        public void TogglePaused()
        {
            //Pause
            bool paused = !pauseMenu.activeInHierarchy;
            pauseMenu.SetActive(paused);

            if(paused)
            {
                wasPausedOnOpenPause = isPaused.value;
                isPaused.value = true;
            }
            else
            {
                isPaused.value = wasPausedOnOpenPause;
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
                CanDoAction("next_day", gameObject);
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
            if(cargo != null)
            cargo.transform.parent = ship.transform;

            return db;
        }
        #endregion

        public void OpenShop()
        {
            AudioController.PlayAudio("shop");

            gameShop.canvasGroup.alpha = 0;

            SetSelectedCrewmate(null);

            List<Cargo> keep = new List<Cargo>();

            int coins = 0;

            foreach(Cargo cargo in sellArea.cargoes)
            {
                if(nextShop.Accepts(this, cargo, out int price) || GAME_SELLS_EVERYTHING)
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
            gameHud.gameObject.SetActive(false);

            shouldTimePass.value = false;

            isOnShop = true;  
        }

        public void CloseShop()
        {
            if(!CanDoAction("close_shop", gameObject))
                return;

            AudioController.PlayAudio("cash");

            shouldTimePass.value = !showingTutorial.value;

            #region Input Data
            int fuelValue = gameShop.GetValue(0);
            int cargoValue = gameShop.GetValue(1);
            int recruitValue = gameShop.GetValue(2);
            int keepValue = gameShop.GetValue(3);
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

            Debug.Log($"Cargos: {cargoBits}");
            //Spawn cargo
            while(cargoBits > 0)
            {
                int lm = Mathf.Min(cargoBits - 1, prefabCargoBox.Length - 1);
                for(int i = lm; i >= 0; i --)
                {
                    CargoBitEntry entry = prefabCargoBox[i];

                    if(i == 0 || UnityEngine.Random.Range(0f, 0.999f) < entry.chance)
                    {
                        cargoBits -= i + 1;

                        Debug.Log($"Drop: {i + 1}");
                       
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
            gameHud.gameObject.SetActive(true);

            isOnShop = false;

            ChoseNewShop(!showingTutorial);
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

        public bool CanDoAction(string action, GameObject obj)
        {
            return canDoAction == null ? true : canDoAction.Invoke(action, obj);
        }

        public void OnGameOver()
        {
            isPaused.value = true;
            shouldTimePass.value = true;
            gameOverCanvas.gameObject.SetActive(true);

            AudioController.PlayAudio("game_over");
        }

        public void ResetGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene("Scenes/Menu");
        }
    }
}