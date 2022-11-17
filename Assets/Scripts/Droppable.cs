using UnityEngine;

namespace DLIFR
{   
    public class Droppable : InteractableBehaviour
    {
        public override void OnInteract(int click)
        {
            Crewmate c = GameController.instance.currentCrewmate;
            if(c != null && c.carrying != null)
            {
                CrewmateDroppingState state = new CrewmateDroppingState();
                state.droppable = this;
                GameController.instance.currentCrewmate.state = state;   
            }
        }

        public override void OnUpdateInteractionDisplay(bool enabled)
        {
            Crewmate c = GameController.instance.currentCrewmate;
            if(c != null && c.carrying != null)
            {
                gameObject.layer = LayerMask.NameToLayer("Interactable");
            }
            else
                gameObject.layer = 0;
        }

        public virtual bool OnDrop(GameObject go)
        {
            return false;
        }
    }
}