using UnityEngine;
using DLIFR.Data;

namespace DLIFR.Props
{
    public class Engine : Droppable
    {
        public Value<float> fuelLevel;
        public Value<float> maxFuelLevel;
        public override bool OnDrop(GameObject go)
        {
            Debug.Log("aa");
            
            Cargo cargo = go.GetComponent<Cargo>();

            if(cargo == null)
                return false;

            float cargoFuelLevel = cargo.fuelValue;

            if(cargoFuelLevel == 0)
                return false;

            GameController instance = GameController.instance;

            fuelLevel.value = Mathf.Min(fuelLevel.value + cargoFuelLevel, maxFuelLevel.value);
        
            return true;
        }
    }
}