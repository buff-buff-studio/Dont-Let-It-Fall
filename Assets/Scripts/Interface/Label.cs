using System.Text.RegularExpressions;
using UnityEngine;
using DLIFR.Data;
using DLIFR.I18n;
using TMPro;

namespace DLIFR.Interface
{
    public class Label : MonoBehaviour
    {
        static readonly Regex translationPattern = new Regex(@"\[([^\}]+)\]", RegexOptions.Compiled);
        static readonly Regex variablesPattern = new Regex(@"\{([^\}]+)\}", RegexOptions.Compiled);
        
        public Variable[] values;

        [SerializeField]
        private string _rawText;
        private TMP_Text text;

        private void OnEnable() 
        {
            text = GetComponent<TMP_Text>();
            _rawText = text.text;

            foreach(Variable var in values)
            {
                var.onChange += ReloadText;
            }

            GameController.onInit += ReloadText;
        }

        private void OnDisable() 
        {
            text.text = _rawText;
        }

        public void ReloadText()
        {
            Language language = GameController.instance.language;

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