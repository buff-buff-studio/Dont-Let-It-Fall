using UnityEngine;
using DLIFR.Game;
using DLIFR.Entities;

namespace DLIFR.Props
{
    public class Grabbable : InteractableBehaviour
    {
        public bool beingGrabbed = false;

        public override void OnInteract(int click, GameMatch match)
        {
            if(beingGrabbed) return;

            Crewmate c = match.currentCrewmate;
            if(c != null && c.carrying == null)
            {
                CrewmateGrabbingState state = new CrewmateGrabbingState();
                state.grabbable = this;
                match.currentCrewmate.state = state;   
            }
        }

        public override void OnUpdateInteractionDisplay(bool enabled, GameMatch match)
        {
            Crewmate c = match.currentCrewmate;
            if(c != null && c.carrying == null && !beingGrabbed)
            {
                gameObject.layer = LayerMask.NameToLayer("Interactable");
            }
            else
                gameObject.layer = 0;
        }
    }
}