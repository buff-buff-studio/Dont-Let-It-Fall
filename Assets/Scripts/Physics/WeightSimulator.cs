using UnityEngine;
using System.Collections.Generic;

namespace DLIFR.Physics
{
    public interface IWeightBody
    {
        float GetWeight();
        Vector3 GetPosition();
    }

    public class WeightSimulator : MonoBehaviour
    {
        public static Vector3 centerOfMass;

        private static List<IWeightBody> bodies = new List<IWeightBody>();
        
        public static void AddBody(IWeightBody body)
        {
            bodies.Add(body);
        }

        public static void RemoveBody(IWeightBody body)
        {
            bodies.Remove(body);
        }

        public static void RecalculateCenterOfMass(Transform tr)
        {
            float weightSum = 0;
            Vector3 positionSum = Vector3.zero;

            foreach(IWeightBody body in bodies)
            {
                float weight = body.GetWeight();
                weightSum += weight;
                positionSum += body.GetPosition() * weight;
            }

            centerOfMass = tr.rotation * positionSum/weightSum;
        }

        public Transform looker;

        [Range(0f, 1f)]
        public float weight = 0;
        private void FixedUpdate() 
        {
            RecalculateCenterOfMass(transform);

            looker.up = Vector3.Lerp(Vector3.up, centerOfMass, weight);
        }
    }
}