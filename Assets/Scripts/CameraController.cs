using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DLIFR
{
    public class CameraController : MonoBehaviour
    {
        public float moveSpeed = 5;

        public Vector3 velocity = Vector3.zero;

        public Vector3 bounds = new Vector3(5, 0, 5);
        public Vector3 pivot;

        private void Update() 
        {
            velocity = Vector3.Lerp(velocity,  moveSpeed * new Vector3(Input.GetAxis("Horizontal"), Input.GetKey(KeyCode.LeftShift) ? -1 : (Input.GetKey(KeyCode.Space) ? 1 : 0), Input.GetAxis("Vertical")), Time.deltaTime * 10f);

            transform.position += velocity * Time.deltaTime;

            Vector3 p = transform.position - pivot;
            p.x = Mathf.Clamp(Mathf.Abs(p.x), 0, bounds.x) * Mathf.Sign(p.x);
            p.y = Mathf.Clamp(Mathf.Abs(p.y), 0, bounds.y) * Mathf.Sign(p.y);
            p.z = Mathf.Clamp(Mathf.Abs(p.z), 0, bounds.z) * Mathf.Sign(p.z);
            transform.position = p + pivot;
        }
    }

    
    #if UNITY_EDITOR
    [CustomEditor(typeof(CameraController))]
    public class CameraControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if(GUILayout.Button("Set Pivot"))
            {
                CameraController controller = ((CameraController) target);
                controller.pivot = controller.transform.position;
            }
        }
    }
    #endif
}