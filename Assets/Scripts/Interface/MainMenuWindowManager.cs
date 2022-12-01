using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DLIFR.Audio;

namespace DLIFR.Interface
{
    public class MainMenuWindowManager : WindowManager
    {
        public bool goToGame = false;

        public float loadGame = 2f;

        protected override void Start()
        {
            Time.timeScale = 1f;
            var open = windows[0].openSpeed;
            windows[0].openSpeed = .5f;
            SetOpen(windows[0], false, true);
            
            foreach(Window window in windows)
            {
                SetOpen(window, window.startOpen, !window.startOpen);
            }

            StartCoroutine(ResetMainMenuSpeed(open));
        }

        private IEnumerator ResetMainMenuSpeed(float open)
        {
            yield return new WaitForSeconds(2f);
            windows[0].openSpeed = open;
        }

        public void PlayGame()
        {
            AudioController.PlayMusic(null);
            goToGame = true;
        }

        public void CloseGame()
        {
            Application.Quit();
        }

        private void FixedUpdate() 
        {
            if(goToGame)
            {
                loadGame -= Time.fixedDeltaTime;

                if(loadGame <= 0)
                {
                    SceneManager.LoadScene("Scenes/Game");
                }
            }
        }
    }
}