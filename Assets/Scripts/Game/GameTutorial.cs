using UnityEngine;
using TMPro;
using DLIFR.Data;

namespace DLIFR.Game
{
    [System.Serializable]
    public class TutorialPage
    {
        [HideInInspector, SerializeField]
        public bool valid = false;

        [Header("MAIN")]
        public string text;
        public bool shouldPauseGame = true;
        public bool shouldTimePass = false;

        [Header("FOCUS")]
        public bool shouldFocusOnto;
        public Transform focusOnto;

        [Header("FOCUS - OFFSET")]
        public bool useCustomFocusOffset = false;
        public Vector3 customFocusOffset = Vector3.zero;

        [Header("SWIPE")]
        public bool shouldSwipe = false;
        public Transform swipeTarget; 
    }

    public class GameTutorial : MonoBehaviour
    {
        [Header("REFERENCES")]
        public Settings settings;
        public Value<bool> gameIsPaused;
        public Value<bool> shouldTimePass;
        public Material swipeMaterial;
        public GameObject swipe;
        public TMP_Text textLabel;

        [Header("STATE")]
        public TutorialPage currentPage;

        public Vector3 focusOffset = Vector3.back * 8 + Vector3.up * 3;

        private void Start() 
        {
            Display(currentPage);
        }

        public void Display(TutorialPage page)
        {
            gameIsPaused.value = true;

            currentPage = page;
            page.valid = true;

            swipe.SetActive(currentPage.shouldSwipe);
            textLabel.text = settings.language.GetEntry(page.text);

            gameIsPaused.value = page.shouldPauseGame;
            shouldTimePass.value = page.shouldTimePass;
        }

        private void Update() 
        {
            float deltaTime = Time.unscaledDeltaTime;
            swipeMaterial.SetFloat("_Radius", 50);

            if(currentPage.valid)
            {
                Camera camera = Camera.main;

                if(currentPage.shouldSwipe)
                {
                    Collider renderer = currentPage.swipeTarget.GetComponent<Collider>();
                    Vector3 colliderA = camera.WorldToScreenPoint(renderer.bounds.min);
                    Vector3 colliderB = camera.WorldToScreenPoint(renderer.bounds.max);

                    float radius = Vector2.Distance(colliderA, colliderB) + 20;

                    Vector3 screenPos = camera.WorldToScreenPoint(currentPage.swipeTarget.position);
                    
                    swipeMaterial.SetVector("_Position", new Vector4(screenPos.x, Screen.height - screenPos.y, 0, 0));
                    swipeMaterial.SetFloat("_Radius", radius);
                }
               
                if(currentPage.shouldFocusOnto)
    	        {                
                    Vector3 offset = currentPage.useCustomFocusOffset ? currentPage.customFocusOffset : focusOffset;

                    camera.transform.position = Vector3.Lerp(
                        camera.transform.position,
                        currentPage.focusOnto.position + offset,
                        deltaTime * 5f
                    );
                }
            }
        }
    }
}