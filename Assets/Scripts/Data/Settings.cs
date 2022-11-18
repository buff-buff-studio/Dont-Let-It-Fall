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
        public Language language;

        [Serializable]
        public class Values
        {
            public float volumeVfx;
            public float volumeMusic;
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
            }
            else
            {
                volumeVfx.value = 0.5f;
                volumeMusic.value = 0.5f;

                Save();
            }
            
            Debug.Log($"[Settings] Loading from '{Application.persistentDataPath}'");
        }

        public void Save()
        {
            Values values = new Values();
            values.volumeVfx = volumeVfx.value;
            values.volumeMusic = volumeMusic.value;

            File.WriteAllText(filePath, JsonUtility.ToJson(values, true));

            Debug.Log($"[Settings] Saving to '{Application.persistentDataPath}'");
        }
    }
}