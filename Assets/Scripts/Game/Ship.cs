using UnityEngine;

namespace DLIFR.Game
{
    public class Ship : MonoBehaviour
    {
        #region Public Fields
        [Header("SETTINGS")]
        public float balanceForce = 20f;

        public Transform targetRotation;
        public Transform shipNavMesh;
        public Transform shipCollider;
        #endregion

        #region Private Fields
        private Rigidbody _rigidbody;
        #endregion

        #region Private Methods
        /// <summary>
        /// Get and setup the needed components
        /// </summary>
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Main physics update section
        /// </summary>
        private void FixedUpdate()
        {
            transform.position = Vector3.zero;
            ApplyBalance();
        }

        /// <summary>
        /// Calculate needed angular velocity to balance the ship again
        /// </summary>
        private void ApplyBalance()
        {
            float inverseBalanceForce = 1f / balanceForce;
            Quaternion deltaRotation = Quaternion.identity * Quaternion.Inverse(targetRotation == null ? transform.rotation : (transform.rotation * Quaternion.Inverse(targetRotation.rotation)));

            float angle;
            Vector3 axis;

            deltaRotation.ToAngleAxis(out angle, out axis);

            if (float.IsInfinity(axis.x))
                return;

            if (angle > 180f) angle -= 360f;

            Vector3 angular = (0.9f * Mathf.Deg2Rad * angle / 1f) * axis.normalized;

            _rigidbody.angularVelocity = Vector3.Lerp(_rigidbody.angularVelocity, angular, Time.fixedDeltaTime * balanceForce);
            
            if(shipNavMesh != null)
            {
                shipCollider.position = shipNavMesh.position;
                shipCollider.rotation = shipNavMesh.rotation;
            }
        }
        #endregion
    }
}