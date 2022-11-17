using UnityEngine;
using DLIFR.Data;

namespace DLIFR.Interface
{
    public class SettingsController : MonoBehaviour
    {
        public Settings settings;

        private void Start() 
        {
            settings.Load();
        }
    }
}