using UnityEngine;
using UnityEngine.UI;
using DLIFR.Data;

namespace DLIFR.Interface
{
    public class WindowSettings : MonoBehaviour
    {
        public Settings settings;

        [Header("WIDGETS")]
        public Slider sliderVolumeVfx;
        public Slider sliderVolumeMusic;

        private void OnEnable() 
        {
            settings.Load();

            sliderVolumeVfx.value = settings.volumeVfx.value * 10f;
            sliderVolumeMusic.value = settings.volumeMusic.value * 10f;
        }

        private void OnDisable() 
        {
            settings.Save();
        }

        public void UpdateValues()
        {
            settings.volumeVfx.value = sliderVolumeVfx.value/10f;
            settings.volumeMusic.value = sliderVolumeMusic.value/10f;
        }
    }
}