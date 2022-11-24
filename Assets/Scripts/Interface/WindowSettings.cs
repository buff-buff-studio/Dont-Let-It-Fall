using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DLIFR.Data;
using DLIFR.Interface.Widgets;

namespace DLIFR.Interface
{
    public class WindowSettings : MonoBehaviour
    {
        public static List<string> m_DropOptions = new List<string> { "Option 1", "Option 2"};

        public Settings settings;

        [Header("WIDGETS")]
        public Slider sliderVolumeVfx;
        public Slider sliderVolumeMusic;
        public TMPro.TMP_Dropdown dropdownLanguage;
        public Toggle toggleTutorial;

        private void OnEnable() 
        {
            settings.Load();

            sliderVolumeVfx.value = Mathf.RoundToInt(settings.volumeVfx.value * 10f);
            sliderVolumeMusic.value = Mathf.RoundToInt(settings.volumeMusic.value * 10f);
        
            m_DropOptions.Clear();
            foreach(var language in settings.languages)
            {
                m_DropOptions.Add(language.displayName);
            }

            dropdownLanguage.ClearOptions();
            dropdownLanguage.AddOptions(m_DropOptions);
            dropdownLanguage.value = settings.languageIndex; 

            toggleTutorial.isOn = settings.showTutorial;
        }

        private void OnDisable() 
        {
            settings.Save();
        }

        public void UpdateValues()
        {
            settings.volumeVfx.value = sliderVolumeVfx.value/10f;
            settings.volumeMusic.value = sliderVolumeMusic.value/10f;
            settings.showTutorial = toggleTutorial.isOn;
        }

        public void UpdateLanguage()
        {
            if(settings.languageIndex != dropdownLanguage.value)
            {
                settings.language.Unload();
                settings.language = settings.languages[dropdownLanguage.value];
            
                foreach(Label label in Label.LABELS)
                {
                    label.ReloadText();
                }
            } 
        }

        private void FixedUpdate() 
        {
            UpdateValues();
        }
    }
}