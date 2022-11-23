using System;
using System.Collections.Generic;
using UnityEngine;

namespace DLIFR.I18n
{
    [CreateAssetMenu(fileName = "Language", menuName = "DLIFR/Language", order = 0)]
    public class Language : ScriptableObject
    {
        public string displayName;
        public string fileName;

        [NonSerialized, HideInInspector]
        private Dictionary<string, string> _entries = new Dictionary<string, string>();
        
        [NonSerialized, HideInInspector]
        private bool _loaded = false;

        public void Unload()
        {
            _entries.Clear();
            _loaded = false;
        }

        public void Load()
        {
            _entries.Clear();

            TextAsset lang = Resources.Load($"Languages/{fileName}") as TextAsset;
    
            foreach(string line in lang.text.Split('\n'))
            {
                if(line.StartsWith("#"))
                    continue;

                string[] str = line.Split('=');
                
                if(str.Length == 2)
                {
                    _entries[str[0].Trim()] = str[1].Trim();
                }
            }

            _loaded = true;
        }

        public string GetEntry(string text)
        {
            if(!_loaded)
                Load();

            if(_entries.TryGetValue(text, out string value))
            {
                value = value.Replace("<§","<b><color=red>");
                value = value.Replace("§>","</color></b>");

                return value;
            }

            return $"<{text}>";
        }
    }
}