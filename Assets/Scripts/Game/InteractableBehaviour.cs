using System.Collections.Generic;
using UnityEngine;

namespace DLIFR.Game
{
    public abstract class InteractableBehaviour : MonoBehaviour, IInteractable
    {
        public static List<InteractableBehaviour> behaviours = new List<InteractableBehaviour>();

        public virtual void OnEnable() 
        {
            behaviours.Add(this);
        }

        public virtual void OnDisable() 
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