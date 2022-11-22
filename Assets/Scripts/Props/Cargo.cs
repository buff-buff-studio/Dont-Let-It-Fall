using UnityEngine;
using DLIFR.Physics;

namespace DLIFR.Props
{
    public class Cargo : Grabbable, IWeightBody
    {
        public const float VOID_HEIGHT = -15;

        public int itemType = 0;

        public float fuelValue = 0;
        
        public ParticleSystem explosionParticles;
        public ParticleSystem lightningParticles;

        private Rigidbody _rigidbody;

        public override void OnEnable() 
        {
            _rigidbody = GetComponent<Rigidbody>();

            WeightSimulator.AddBody(this);

            base.OnEnable();
        }

        public override void OnDisable() 
        {
            WeightSimulator.RemoveBody(this);

            base.OnDisable();
        }

        public float GetWeight()
        {
            return 1;
        }

        public Vector3 GetPosition()
        {
            return transform.localPosition;
        }

        private void FixedUpdate() 
        {
            if(transform.position.y <= VOID_HEIGHT)
            {
                Destroy(gameObject);
            }
        }

        public void Fire()
        {
            Instantiate(lightningParticles, transform.position + new Vector3(0,10,0), Quaternion.identity, transform);
            Destroy(gameObject, 5);
        }

        private void OnTriggerEnter(Collider other) 
        {   
            Area area = other.GetComponentInParent<Area>();

            if(area != null)
            {
                area.cargoes.Add(this);
                area.onChange?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other) 
        {      
            Area area = other.GetComponentInParent<Area>();

            if(area != null)
            {
                area.cargoes.Remove(this);
                area.onChange?.Invoke();
            }
        }
    }
}