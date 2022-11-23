using System;
using UnityEngine;
using DLIFR.Props;

namespace DLIFR.Entities
{
    [Serializable]
    public class CrewmateState
    {
        public Crewmate crewmate;

        public virtual void OnEnter() {}
        public virtual void OnLeave() {}

        public virtual void OnTick(float deltaTime) {}

        public virtual Vector3 OnRecalculatePath(Vector3 target) => target;
        public virtual Vector3 OnStartWalking(Vector3 target) => target;
        public virtual void OnEndWalking(bool done) {}

        public virtual float GetWalkSpeed() => crewmate.walkSpeed;
    }

    [Serializable]
    public class CrewmateIdleState : CrewmateState
    {
        public float maxWalkDistance = 2f;
        public Vector3 origin;

        public float minDelayToWalk = 1f;
        public float maxDelayToWalk = 3f;

        public float timeLimitToReachTarget = 4f;

        public float _waitFor = 0f;

        public override void OnEnter()
        {
            origin = crewmate.ship.InverseTransformPoint(crewmate.transform.position);
        }

        public Vector3 GetTarget()
        {
            Vector3 target = origin + new Vector3(
                UnityEngine.Random.Range(-1f, 1f),
                0,
                UnityEngine.Random.Range(-1f, 1f)
            ).normalized * maxWalkDistance;

            return crewmate.ship.TransformPoint(target);
        }

        public override void OnTick(float deltaTime)
        {
            if((_waitFor -= deltaTime) <= 0)
            {  
                crewmate.WalkTo(GetTarget());
                _waitFor = timeLimitToReachTarget;
            }
        }

        public override void OnEndWalking(bool done)
        {
            _waitFor = UnityEngine.Random.Range(minDelayToWalk, maxDelayToWalk);
        }

        public override float GetWalkSpeed()
        {
            return 1f;
        }
    }

    [Serializable]
    public class CrewmateGrabbingState : CrewmateState
    {
        public Grabbable grabbable;

        public override void OnEnter()
        {
            crewmate.WalkTo(grabbable.transform.position);
            grabbable.beingGrabbed = true;
        }

        public override void OnLeave()
        {
            grabbable.beingGrabbed = false;
        }

        public override Vector3 OnRecalculatePath(Vector3 target)
        {
            return grabbable.transform.position;
        }

        public override void OnEndWalking(bool done)
        {
            crewmate.match.CanDoAction("crewmate_walk_end", crewmate.gameObject);

            if(grabbable is Vault)
            {
                int salary = crewmate.salaryRechargePrice;

                if(crewmate.match.coinCount.value >= salary)
                {
                    
                    crewmate.match.coinCount.value -= salary;
                    crewmate.salaryTime.value = crewmate.salaryRechargeTime.value;
                }
            }
            else
            {
                crewmate.SetCarrying(grabbable.gameObject);
            }

            crewmate.match.UpdateInteractionDisplay();
            crewmate.state = new CrewmateIdleState();
        }
    }

    [Serializable]
    public class CrewmateWalkingToState : CrewmateState
    {
        public Vector3 target;

        public override void OnEnter()
        {
            crewmate.targetHolder.gameObject.SetActive(true);
            crewmate.WalkTo(target);
        }

        public override void OnLeave()
        {
            crewmate.targetHolder.gameObject.SetActive(false);
        }

        public override void OnEndWalking(bool done)
        {
            crewmate.match.CanDoAction("crewmate_walk_end", crewmate.gameObject);

            if(crewmate.carrying != null)
                crewmate.DropCarrying();

            crewmate.match.UpdateInteractionDisplay();
            crewmate.state = new CrewmateIdleState();
        }
    }

    [Serializable]
    public class CrewmateDroppingState : CrewmateState
    {
        public Droppable droppable;

        public override void OnEnter()
        {
            crewmate.WalkTo(droppable.transform.position);
        }

        public override Vector3 OnRecalculatePath(Vector3 target)
        {
            return droppable.transform.position;
        }

        public override void OnEndWalking(bool done)
        {
            crewmate.match.CanDoAction("crewmate_walk_end", crewmate.gameObject);
            
            if(droppable.OnDrop(crewmate.carrying))
            {
                GameObject.Destroy(crewmate.carrying);
                crewmate.carrying = null;

                crewmate.match.UpdateInteractionDisplay();
                crewmate.state = new CrewmateIdleState();
            }
        }
    }
}