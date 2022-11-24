using System;
using System.Collections.Generic;
using UnityEngine;

namespace DLIFR.Interface
{
    [Serializable]
    public class Window
    {
        public string id;

        public CanvasGroup group;
        public bool startOpen = false;

        public float openSpeed = 2f;
        public float closeSpeed = 2f;

        public void OnStartOpening()
        {
            group.gameObject.SetActive(true);
            group.interactable = false;
        }

        public void OnOpen()
        {
            group.interactable = true;
        }

        public void OnStartClosing()
        {
            group.interactable = false;
        }

        public void OnClose()
        {
            group.gameObject.SetActive(false);
        }
    }

    public class WindowManager : MonoBehaviour
    {
        public Window[] windows;

        [SerializeField, HideInInspector]
        public List<Window> _opening = new List<Window>();

        [SerializeField, HideInInspector]
        public List<Window> _closing = new List<Window>();

        [SerializeField, HideInInspector]
        public List<Window> _open = new List<Window>();

        protected virtual void Start()
        {
            foreach(Window window in windows)
            {
                SetOpen(window, window.startOpen, true);
            }
        }

        public void SetOpen(Window window, bool open, bool instantly = false)
        {
            if(window is null)
                return;

            _closing.Remove(window);
            _opening.Remove(window);
            _open.Remove(window);

            if(open)
            {
                if(instantly)
                {
                    window.group.alpha = 1;
                    window.OnOpen();
                    _open.Add(window);
                }
                else
                {
                    window.OnStartOpening();
                    _opening.Add(window);  
                }

                window.group.transform.SetAsLastSibling();
            }
            else
            {
                if(instantly)
                {
                    window.group.alpha = 0;
                    window.OnClose();
                }
                else
                {
                    window.OnStartClosing();
                    _closing.Add(window);
                }
            }
        }

        public Window GetWindow(string id)
        {
            foreach(Window window in windows)
                if(window.id == id)
                    return window;

            return null;
        }

        private void Update() 
        {
            for(int i = _opening.Count - 1; i >= 0; i --)
            {
                Window window = _opening[i];
                float f = window.group.alpha = Mathf.Clamp01(window.group.alpha + Time.deltaTime * window.openSpeed);
            
                if(f == 1)
                {
                    SetOpen(window, true, true);
                }
            }

            for(int i = _closing.Count - 1; i >= 0; i --)
            {
                Window window = _closing[i];
                float f = window.group.alpha = Mathf.Clamp01(window.group.alpha - Time.deltaTime * window.closeSpeed);
            
                if(f == 0)
                {
                    SetOpen(window, false, true);
                }
            }
        }

        public void OpenWindow(string id)
        {
            SetOpen(GetWindow(id), true);
        }

        public void CloseWindow(string id)
        {
            SetOpen(GetWindow(id), false);
        }

        public void OpenWindowInstantly(string id)
        {
            SetOpen(GetWindow(id), true, true);
        }

        public void CloseWindowInstantly(string id)
        {
            SetOpen(GetWindow(id), false, true);
        }

        public void CloseAll()
        {
            foreach(Window window in windows)
                SetOpen(window, false);
        }

        public void CloseAllInstantly()
        {
            foreach(Window window in windows)
                SetOpen(window, false, true);
        }
    }
}