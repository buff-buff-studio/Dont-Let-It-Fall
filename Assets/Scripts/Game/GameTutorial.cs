using UnityEngine;
using DLIFR.Data;

namespace DLIFR.Game
{
    [System.Serializable]
    public class TutorialPage
    {
        public bool valid = false;
        public bool shouldFocus;

        public Transform focusOnto;
        public string text;
    }

    public class GameTutorial : MonoBehaviour
    {
        public Value<bool> gameIsPaused;
        public Material swipeMaterial;
        public TutorialPage currentPage;

        public Vector3 offset = Vector3.back * 8 + Vector3.up * 3;

        public void Display(TutorialPage page)
        {
            gameIsPaused.value = true;

            currentPage = page;
            page.valid = true;
        }

        private void Update() 
        {
            float deltaTime = Time.unscaledDeltaTime;
            swipeMaterial.SetFloat("_Radius", 50);

            if(currentPage.valid)
            {
                Camera camera = Camera.main;

                Collider renderer = currentPage.focusOnto.GetComponent<Collider>();
                Vector3 colliderA = camera.WorldToScreenPoint(renderer.bounds.min);
                Vector3 colliderB = camera.WorldToScreenPoint(renderer.bounds.max);

                float radius = Vector2.Distance(colliderA, colliderB) + 20;

                Vector3 screenPos = camera.WorldToScreenPoint(currentPage.focusOnto.position);
                
                swipeMaterial.SetVector("_Position", new Vector4(screenPos.x, Screen.height - screenPos.y, 0, 0));
                swipeMaterial.SetFloat("_Radius", radius);

                camera.transform.position = Vector3.Lerp(
                    camera.transform.position,
                    currentPage.focusOnto.position + offset,
                    deltaTime * 5f
                );
            }
        }
    }
}