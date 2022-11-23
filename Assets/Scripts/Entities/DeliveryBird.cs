using UnityEngine;

namespace DLIFR.Entities
{
    public class DeliveryBird : MonoBehaviour
    {
        public Vector3 target;
        public Vector3 speed = Vector3.zero;
        public int stage = 0;

        public Transform carrying;

        private void Start() 
        {
            transform.position = Quaternion.Euler(0, Random.Range(90f, 270f), 0) * transform.forward * 12 + new Vector3(0, 5, 5);

            target = new Vector3(
                Random.Range(-4f, 4f),
                2f,
                Random.Range(-4f, 4f)
            );

            RefreshCarrying();
        }

        public void RefreshCarrying()
        {
            if(carrying != null)
            {
                Rigidbody rb = carrying.GetComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.detectCollisions = false;

                carrying.transform.parent = transform;
                carrying.transform.localPosition = new Vector3(0, 0f, 0.55f);
                carrying.transform.localEulerAngles = Vector3.zero;
            }
        }

        public void Update()
        {
            Vector3 delta = this.target - transform.position;
            delta.y = 0;
            float distance = delta.magnitude;

            Vector3 target = this.target;
            switch(stage)
            {
                case 0:
                    target.y += distance/2f;

                    transform.forward = delta.normalized;

                    transform.position = Vector3.Lerp(
                        transform.position,
                        target,
                        Time.deltaTime * 1f
                    );

                    if(distance < 0.75f)
                    {
                        DropDelivery();
                        break;
                    }
                    break;

                case 1:
                    transform.localEulerAngles += new Vector3(0, Mathf.Sign(transform.position.x) * 30f * Time.deltaTime, 0); 
                    speed += (transform.up + transform.forward * 2f).normalized * Time.deltaTime * 4f;
                    transform.position += speed * Time.deltaTime;

                    if(transform.position.y > 8f)
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
        }

        public void DropDelivery()
        {
            stage = 1;

            Rigidbody rb = carrying.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.detectCollisions = true;

            carrying.transform.parent = null;
            carrying = null;
        }
    }
}