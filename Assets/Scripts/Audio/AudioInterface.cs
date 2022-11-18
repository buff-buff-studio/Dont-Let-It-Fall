using System;
using UnityEngine;

namespace DLIFR.Audio
{
    [CreateAssetMenu(fileName = "AudioInterface", menuName = "DLIFR/AudioInterface", order = 0)]
    public class AudioInterface : ScriptableObject 
    {
        [Serializable]
        public class Audio
        {
            public string id;
            public AudioClip clip;
        }

        public Audio[] audios;
    }
}