using System.Text.RegularExpressions;
using UnityEngine;
using DLIFR.Data;
using DLIFR.I18n;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DLIFR.Interface
{
    public class Label : MonoBehaviour
    {
        static readonly Regex translationPattern = new Regex(@"\[([^\}]+)\]", RegexOptions.Compiled);
        static readonly Regex variablesPattern = new Regex(@"\{([^\}]+)\}", RegexOptions.Compiled);
        
        public Variable[] values;
        public Settings settings;

        [SerializeField]
        private string _rawText;
        private TMP_Text text;

        #if UNITY_EDITOR
        void Reset()
        {
            string[] guids = AssetDatabase.FindAssets("t:Settings", new string[]{"Assets/Data"});
            foreach (string guid in guids) 
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                settings = AssetDatabase.LoadAssetAtPath (path, typeof(Settings)) as Settings;
                break;
            }
        }
        #endif

        private void OnEnable() 
        {
            text = GetComponent<TMP_Text>();
            _rawText = text.text;

            foreach(Variable var in values)
            {
                var.onChange += ReloadText;
            }

            ReloadText();
        }

        private void OnDisable() 
        {
            text.text = _rawText;
        }

        public void ReloadText()
        {
            Language language = settings.language;

            string text = translationPattern.Replace(_rawText, delegate(Match match)
            {
                string key = match.Groups[1].Value;
                return language.GetEntry(key);
            });

            this.text.text = variablesPattern.Replace(text, delegate(Match match)
            {
                string key = match.Groups[1].Value;

                string[] s = key.Split(':');

                if(s.Length == 1)
                    return values[int.Parse(key)].AsString();
                else
                    return values[int.Parse(s[0])].AsString(s[1]);
            });
        }
    }
}