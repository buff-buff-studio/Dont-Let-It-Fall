using UnityEngine;

namespace DLIFR
{
    public class Grabbable : InteractableBehaviour
    {
        public bool beingGrabbed = false;

        public override void OnInteract(int click)
        {
            if(beingGrabbed) return;

            Crewmate c = GameController.instance.currentCrewmate;
            if(c != null && c.carrying == null)
            {
                CrewmateGrabbingState state = new CrewmateGrabbingState();
                state.grabbable = this;
                GameController.instance.currentCrewmate.state = state;   
            }
        }

        public override void OnUpdateInteractionDisplay(bool enabled)
        {
            Crewmate c = GameController.instance.currentCrewmate;
            if(c != null && c.carrying == null && !beingGrabbed)
            {
                gameObject.layer = LayerMask.NameToLayer("Interactable");
            }
            else
                gameObject.layer = 0;
        }
    }
}