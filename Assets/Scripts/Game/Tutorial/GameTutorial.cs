using UnityEngine;
using TMPro;
using DLIFR.Data;

namespace DLIFR.Game.Tutorial
{
    [System.Serializable]
    public class TutorialPage
    {
        [HideInInspector, SerializeField]
        public bool valid = false;

        [Header("MAIN")]
        public bool hasText = true;
        public string text;

        public bool shouldPauseGame = false;
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
        public GameMatch match;
        public Settings settings;
        public Value<bool> gameIsPaused;
        public Value<bool> shouldTimePass;
        public Material swipeMaterial;
        public GameObject swipe;
        public GameObject subtitle;
        public TMP_Text textLabel;

        [Header("STATE")]
        public TutorialPage currentPage;

        public Vector3 focusOffset = Vector3.back * 8 + Vector3.up * 3;

        public void Display(TutorialPage page)
        {
            gameIsPaused.value = true;

            currentPage = page;
            page.valid = true;

            swipe.SetActive(currentPage.shouldSwipe);
            subtitle.SetActive(currentPage.hasText);

            if(currentPage.hasText)
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
                    Collider collider = currentPage.swipeTarget.GetComponent<Collider>();
                    
                    float radius = 50;
                    Vector3 screenPos = Vector3.zero;

                    if(collider != null)
                    {                    
                        Vector3 colliderA = camera.WorldToScreenPoint(collider.bounds.min);
                        Vector3 colliderB = camera.WorldToScreenPoint(collider.bounds.max);
                        radius = Vector2.Distance(colliderA, colliderB) + 20;

                        screenPos = camera.WorldToScreenPoint(currentPage.swipeTarget.position);
                    
                        if(screenPos.z < 0.5f)
                        {
                            radius = 0;
                        }
                    }
                    else 
                    {
                        RectTransform transform = currentPage.swipeTarget.GetComponent<RectTransform>();
                    
                        if(transform != null)
                        {
                            screenPos = transform.position;
                            radius = Mathf.Max(transform.sizeDelta.x, transform.sizeDelta.y) + 20;
                        }
                        else
                        {
                            screenPos = camera.WorldToScreenPoint(currentPage.swipeTarget.position);
                        }
                    }

                    
                    
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

    public class GameTutorialSection : MonoBehaviour
    {
        [HideInInspector, SerializeField]
        public GameTutorial tutorial;
        public GameMatch match => tutorial.match;

        [SerializeField, HideInInspector]
        private int _page;

        public int page => _page;

        private void OnEnable() 
        {
            tutorial = GetComponentInParent<GameTutorial>();
            
            _page = GetStartingIndex();
            OnOpenPage(_page);
        }

        public virtual bool OnOpenPage(int page)
        {
            return true;
        }

        public virtual bool CanSkipPage()
        {
            return true;
        }

        public void NextPage()
        {
            _page ++;
            OnOpenPage(_page);
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(CanSkipPage())
                    NextPage();
            }
        }

        public virtual int GetStartingIndex()
        {
            return 0;
        }
    }
}