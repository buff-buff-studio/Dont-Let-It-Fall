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
        public Slider sliderSense;
        public TMPro.TMP_Dropdown dropdownLanguage;
        public TMPro.TMP_Dropdown dropdownGraphics;
        public Toggle toggleTutorial;
        public Toggle togglePFX;
        public Toggle toggleVsync;
        public Toggle toggleFullscreen;
        public Toggle toggleFInvertXY;

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

            if(toggleTutorial != null)
                toggleTutorial.isOn = settings.showTutorial;
            
            UpdateQualityDropDown();
        }

        public void OnUpdateQualityDropdown()
        {
            settings.graphicsQuality.value = dropdownGraphics.value;
            settings.UpdateGraphicSettings();
        }

        public void UpdateQualityDropDown()
        {
            if(dropdownGraphics == null)
                return;

            dropdownGraphics.ClearOptions();

            List<string> graphics = new List<string>();
            dropdownGraphics.ClearOptions();

            foreach(string quality in QualitySettings.names)
            {
                string q = "graphics.quality." + quality.ToLower().Replace(" ","_");
            
                graphics.Add(settings.language.GetEntry(q));
            }

            dropdownGraphics.AddOptions(graphics);
            dropdownGraphics.value = settings.graphicsQuality.value;
        }

        private void OnDisable() 
        {
            settings.Save();
        }

        public void UpdateValues()
        {
            settings.volumeVfx.value = sliderVolumeVfx.value/10f;
            settings.volumeMusic.value = sliderVolumeMusic.value/10f;
            settings.camSense.value = sliderSense.value;
            settings.Vsync.value = toggleVsync.isOn;
            settings.fullScreen.value = toggleFullscreen.isOn;
            settings.invertXY.value = toggleFInvertXY.isOn;
            settings.pfx.value = togglePFX.isOn;

            if(toggleTutorial != null)
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

            UpdateQualityDropDown();
        }

        private void FixedUpdate() 
        {
            UpdateValues();
        }
    }
}