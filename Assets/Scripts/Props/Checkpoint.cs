using UnityEngine;
using DLIFR.Data;

namespace DLIFR.Props
{   
    public class Checkpoint : MonoBehaviour
    {
        public Value<float> worldTime;
        public Value<float> timeMultiplier;
        public Value<float> checkPointDuration;

        public float speed = 0;
        public int day = 0;

        public bool inCheckpoint = false;

        private void Update() 
        {   
            GameController instance = GameController.instance;
            float worldTime = this.worldTime.value;
            //Wind effect
            float y = Mathf.Sin(worldTime * Mathf.Deg2Rad * 5) * 1f;

            float f = worldTime - day * 24;
            
            if(f < 24)
            {
                transform.position = new Vector3(instance.lineX - (1f - instance.travelCurve.Evaluate(f)) * 80, y, 0);
            }
            else if(f > 36)
            {
                day = Mathf.FloorToInt(worldTime/24f);

                if(inCheckpoint)
                {
                    inCheckpoint = false;
                }

                transform.position = new Vector3(instance.lineX - (1f - instance.travelCurve.Evaluate(f - checkPointDuration.value * timeMultiplier.value)) * 80, y, 0);
            }
            else if((f - 24)/timeMultiplier.value < checkPointDuration.value)
            {
                if(!inCheckpoint)
                {
                    instance.OnCheckpointBegins();
                    inCheckpoint = true;
                }

                transform.position = new Vector3(instance.lineX, y, 0);
            }
            else
            {
                if(inCheckpoint)
                {
                    inCheckpoint = false;
                }

                transform.position = new Vector3(instance.lineX - (1f - instance.travelCurve.Evaluate(f - checkPointDuration.value * timeMultiplier.value)) * 80, y, 0);
            }

        }
    }
}