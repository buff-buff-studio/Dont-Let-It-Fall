using System;
using System.Collections.Generic;
using UnityEngine;
using DLIFR.Interface;
using DLIFR.Entities;
using DLIFR.Props;
using DLIFR.Data;
using DLIFR.I18n;
/*
- Caixa que se movimenta - com carangueijo
- caixa congelada - que desliza

- voce clica em algum botao que seleciona o trabalhador livre mais proximo do seu cursor
*/

/*
Quando voce passa no checkpoint vem bichos voadores que dropam mercadorias
de acordo com seu dinheiro; uma parte para novas mercadorias e outra para itens basicos de sobrevivencia

Durante o checkpoint voce pode escolher o que comprar ?
- Tripulante (com limite depenendo do level)
- Mercadoria pra vender e conseguir mais moeda
- Combustivel
*/

/*
    Perguntas:
    - devo fazer aquele sistema do clique automatico?
    - se voce coloac um item sem valor de venda, ele vende mesmo assim?
    - as cargas ficam mais valiosas por quanto mais tempo voce mantem elas?
    - se voce pasasr o mouse em cima mostra informacao da caixa?
    - caixas diferentes com propriedades diferentes?
    - o combustivel deve continuar sendo gasto enquanto ta no checkpoint?
    - como a loja deveria ficar?
    - uou

    Alterar fixed update pra menos vezes e aumentar iteracao de fisicas
    criar um sistema que calcula sozinho um "peso" falso das coisas pra fazer o barco tomba mais sem problemas
*/
namespace DLIFR
{ 
    public class GameController : MonoBehaviour
    {
        public static Action onInit;

        public static GameController instance => _instance;
        private static GameController _instance;

        [Header("REFERENCES")]
        public Ship ship;
        public Area shipArea;
        public DLIFR.Interface.Shop shop;

        [Header("PREFABS")]
        public GameObject prefabBird;
        public GameObject prefabRock;
        public GameObject prefabBox;
        public GameObject prefabCrewmateEgg;

        [Header("SETTINGS")]
        public LayerMask selectableMask;
        public float lineX = -5f;
        public Value<float> maxFuelLevel = 30f;
        public Value<float> fuelConsumptionRate = 0.5f;

        public AnimationCurve travelCurve;
        private Value<float> checkPointDuration;
        
        [Header("STATE")]
        public Crewmate currentCrewmate;
        public Value<float> gameTime;
        public Value<float> worldTime;   
        public Value<int> coinCount;
        public Value<float> fuelLevel;
        public Value<int> sellingValue;

        private int fixedFrameCount = 0;

        public Value<float> timeMultiplier;

        private void Start() 
        {
            coinCount.value = 0;
            worldTime.value = 0;
            gameTime.value = 0;
            fuelLevel.value = maxFuelLevel.value;
        }

        public void OnEnable() 
        {
            _instance = this;

            shipArea.onChange = () => {
                int price = 0;

                foreach(Cargo cargo in shipArea.cargoes)
                    price += cargo.sellValue;
                
                sellingValue.value = price;
            };

            onInit?.Invoke();
        }

        public void Update()
        {
            //Update timer
            #region Timer
   
            //if(worldTime/24 < 1 || worldTime%24 > checkPointDuration.value)
            fuelLevel.value = Mathf.Max(0f, fuelLevel.value - Time.deltaTime * timeMultiplier.value * fuelConsumptionRate);
            if(fuelLevel.value == 0 && ship.enabled)
                ship.balanceForce = Mathf.Lerp(ship.balanceForce, 0, Time.deltaTime * 5f);
            #endregion

            bool m0 = Input.GetMouseButtonUp(0);
            bool m1 = Input.GetMouseButtonUp(1);

            if(Input.GetKeyDown(KeyCode.P))
            {
                timeMultiplier.value = timeMultiplier.value == 0.5f ? 5 : 0.5f;
            }

            if(Input.GetKeyDown(KeyCode.B))
            {
                SpawnBird(prefabRock);
            }

            if(m0 || m1)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(UnityEngine.Physics.Raycast(ray.origin, ray.direction, out RaycastHit hit, 30f, selectableMask))
                {
                    IInteractable interactable = hit.collider.gameObject.GetComponent<IInteractable>();
                    interactable?.OnInteract(m0 ? 0 : 1);

                    if(currentCrewmate != null && hit.collider.gameObject.tag == "Ground")
                    {
                        currentCrewmate.OnClickOnGround(hit.point);
                    }
                }
            }

            UpdateInteractionDisplay();
        }

        private void FixedUpdate() 
        {
            fixedFrameCount ++;

            gameTime.value += Time.fixedDeltaTime;
            worldTime.value += Time.fixedDeltaTime * timeMultiplier.value;
        }

        public void UpdateInteractionDisplay()
        {
            InteractableBehaviour.UpdateInteractionDisplay(true);
        }

        public void OnCheckpointBegins()
        {
            SellAllItems(shipArea);
            shop.gameObject.SetActive(true);
        }

        public void OnCheckpointEnds()
        {
            //Drop products
            int index = 0;
            foreach(ShopItemWidget widget in shop.widgets)
            {
                coinCount.value -= widget.count * widget.price;
                for(int i = 0 ; i < widget.count; i ++)
                {
                    SpawnBird(shop.items[index].GetPrefab());
                }

                index ++;
            }

            shop.gameObject.SetActive(false);
        }

        #region Inventory
        public void SellAllItems(Area area)
        {
            List<Cargo> toRemove = new List<Cargo>();
            foreach(Cargo o in area.cargoes)
            {
                if(o.sellValue > 0)
                    toRemove.Add(o);
            }

            foreach(Cargo o in toRemove)
            {
                coinCount.value += o.sellValue;
                Destroy(o.gameObject);
                area.cargoes.Remove(o);
            }

            area.onChange?.Invoke();
        }
        #endregion

        #region Birds
        public DeliveryBird SpawnBird(GameObject carryingPrefab)
        {
            GameObject bird = GameObject.Instantiate(prefabBird);
            GameObject carrying = GameObject.Instantiate(carryingPrefab);

            DeliveryBird db = bird.GetComponent<DeliveryBird>();
            db.carrying = carrying.transform;
            db.RefreshCarrying();

            return db;
        }
        #endregion
    }
}