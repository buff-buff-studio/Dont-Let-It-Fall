using UnityEngine;
using DLIFR.Audio;

namespace DLIFR.Tests
{
    public class TESTAudioSystem : MonoBehaviour
    {
        private void Update() 
        {
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                AudioController.PlayMusic("music");
            }

            if(Input.GetKeyDown(KeyCode.Alpha2))
            {
                AudioController.PlayMusic("music2");
            }

            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                AudioController.PlayMusic("music", 0);
            }

            if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                AudioController.PlayMusic("music2", 0);
            }

            if(Input.GetKeyDown(KeyCode.Alpha5))
            {
                AudioController.PlayMusic(null);
            }

            if(Input.GetKeyDown(KeyCode.Alpha6))
            {
                AudioController.PlayMusic(null, 0);
            }

            if(Input.GetKeyDown(KeyCode.W))
            {
                AudioController.PlayAudio("click");
            }
        }
    }
}