using System;
using System.Collections.Generic;
using UnityEngine;

namespace DLIFR.I18n
{
    [CreateAssetMenu(fileName = "Language", menuName = "DLIFR/Language", order = 0)]
    public class Language : ScriptableObject
    {
        public string displayName;

        [TextArea(20, 25)]
        public string content;

        [NonSerialized, HideInInspector]
        private Dictionary<string, string> _entries = new Dictionary<string, string>();
        
        [NonSerialized, HideInInspector]
        private bool _loaded = false;

        public void Load()
        {
            _entries.Clear();

            foreach(string line in content.Split('\n'))
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
                return value;
            }

            return $"<{text}>";
        }
    }
}