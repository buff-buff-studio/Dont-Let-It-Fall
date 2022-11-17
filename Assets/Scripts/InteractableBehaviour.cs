using UnityEngine;
using System.Collections.Generic;

namespace DLIFR
{
    public abstract class InteractableBehaviour : MonoBehaviour, IInteractable
    {
        public static List<InteractableBehaviour> behaviours = new List<InteractableBehaviour>();

        private void OnEnable() 
        {
            behaviours.Add(this);
        }

        private void OnDisable() 
        {
            behaviours.Remove(this);
        }

        public virtual void OnInteract(int click) {}
        public virtual void OnUpdateInteractionDisplay(bool enabled) {}

        public static void UpdateInteractionDisplay(bool enabled)
        {
            foreach(InteractableBehaviour behaviour in behaviours)
            {
                behaviour.OnUpdateInteractionDisplay(enabled);
            }
        }
    }

    public interface IInteractable
    {
        void OnInteract(int click);
        void OnUpdateInteractionDisplay(bool enabled);
    }
}