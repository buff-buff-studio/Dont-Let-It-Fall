using UnityEngine;
using DLIFR.Game;
using DLIFR.Entities;

namespace DLIFR.Props
{   
    public class Droppable : InteractableBehaviour
    {
        public override void OnInteract(int click, GameMatch match)
        {
            if(!match.CanDoAction("drop", gameObject)) return;
            
            Crewmate c = match.currentCrewmate;
            if(c != null && c.carrying != null)
            {
                CrewmateDroppingState state = new CrewmateDroppingState();
                state.droppable = this;
                match.currentCrewmate.state = state;   
            }
        }

        public override void OnUpdateInteractionDisplay(bool enabled, GameMatch match)
        {
            Crewmate c = match.currentCrewmate;
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