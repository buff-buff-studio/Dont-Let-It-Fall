using System;
using System.IO;
using UnityEngine;
using DLIFR.I18n;

namespace DLIFR.Data
{
    [CreateAssetMenu(fileName = "Settings", menuName = "DLIFR/Settings", order = 0)]
    public class Settings : ScriptableObject 
    {
        public static string filePath => $"{Application.persistentDataPath}/settings.json";

        [NonSerialized]
        private bool _loaded = false;

        public Value<float> volumeVfx;
        public Value<float> volumeMusic;
        public Value<bool> showTutorial;
        public Value<bool> Vsync;
        public Value<bool> fullScreen;
        public Value<bool> invertXY;
        public Value<int> graphicsQuality;
        public Value<bool> pfx;
        public Value<float> camSense;

        public Language language;

        public Language[] languages;

        public int languageIndex => Array.IndexOf(languages, language);

        [Serializable]
        public class Values
        {
            public float volumeVfx = 0.5f;
            public float volumeMusic = 0.5f;
            public int languageIndex;
            public bool showTutorial = true;
            public int qualityLevel = 3;
            public bool Vsync = true;
            public bool fullScreen = true;
            public bool invertXY = false;
            public float camSense = .5f;
            public bool pfx = true;
        }

        public void UpdateGraphicSettings()
        {
            QualitySettings.SetQualityLevel(graphicsQuality.value);
        }

        public void Load()
        {
            if(_loaded)
                return;

            _loaded = true;

            if(File.Exists(filePath))
            {
                Values values = JsonUtility.FromJson<Values>(File.ReadAllText(filePath));
                
                volumeVfx.value = values.volumeVfx;
                volumeMusic.value = values.volumeMusic;
                language = languages[values.languageIndex];
                showTutorial.value = values.showTutorial;
                graphicsQuality.value = values.qualityLevel;
                Vsync.value = values.Vsync;
                fullScreen.value = values.fullScreen;
                invertXY.value = values.invertXY;
                camSense.value = values.camSense;
                pfx.value = values.pfx;
            }
            else
            {
                volumeVfx.value = 0.5f;
                volumeMusic.value = 0.5f;
                showTutorial.value = true;
                graphicsQuality.value = 3;
                Vsync.value = true;
                fullScreen.value = true;
                invertXY.value = false;
                camSense.value = .5f;
                pfx.value = true;

                language = languages[0];

                Save();
            }

            UpdateGraphicSettings();
            
            Debug.Log($"[Settings] Loading from '{Application.persistentDataPath}'");
        }

        public void Save()
        {
            Values values = new Values();
            values.volumeVfx = volumeVfx.value;
            values.volumeMusic = volumeMusic.value;
            values.languageIndex = languageIndex;
            values.showTutorial = showTutorial.value;
            values.qualityLevel = graphicsQuality.value;
            values.Vsync = Vsync.value;
            values.fullScreen = fullScreen.value;
            values.invertXY = invertXY.value;
            values.camSense = camSense.value;
            values.pfx = pfx.value;

            File.WriteAllText(filePath, JsonUtility.ToJson(values, true));

            Debug.Log($"[Settings] Saving to '{Application.persistentDataPath}'");
        }
    }
}