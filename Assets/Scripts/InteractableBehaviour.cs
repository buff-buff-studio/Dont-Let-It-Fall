using System.Collections.Generic;
using UnityEngine;
using DLIFR.Game;

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

        public virtual void OnInteract(int click, GameMatch match) {}
        public virtual void OnUpdateInteractionDisplay(bool enabled, GameMatch match) {}

        public static void UpdateInteractionDisplay(bool enabled, GameMatch match)
        {
            foreach(InteractableBehaviour behaviour in behaviours)
            {
                behaviour.OnUpdateInteractionDisplay(enabled, match);
            }
        }
    }

    public interface IInteractable
    {
        void OnInteract(int click, GameMatch match);
        void OnUpdateInteractionDisplay(bool enabled, GameMatch match);
    }
}