using System;
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