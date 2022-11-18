using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DLIFR.Data;

namespace DLIFR.Audio
{
    public class AudioController : MonoBehaviour 
    {
        private static AudioController _instance;

        [Header("SETTINGS")]
        public int audioSourceCount = 5;
        public Value<float> volumeVfx;
        public Value<float> volumeMusic; 

        public bool startPlayingMusic = false;
        public string startPlayingMusicId = "";

        [Header("AUDIOS")]
        public AudioInterface audioInterface;
        private Dictionary<string, AudioClip> _clipsLookUp = new Dictionary<string, AudioClip>();

        [Header("STATE")]
         public float musicVolumeMultiplier = 1f;
        public AudioSource currentMusicSource;

        public List<AudioSource> freeAudioSources = new List<AudioSource>();
        public List<AudioSource> busyAudioSources = new List<AudioSource>();


        private void Awake() 
        {
            for(int i = 0; i < audioSourceCount; i ++)
            {
                GameObject empty = new GameObject();
                empty.transform.parent = transform;
                empty.name = $"Audio Source {i}";

                AudioSource source = empty.AddComponent<AudioSource>();
                freeAudioSources.Add(source);
                empty.SetActive(false);
            } 
        }

        private void OnEnable() 
        {
            _instance = this;

            volumeVfx.variable.onChange += () => {
                float v = volumeVfx.value;

                foreach(AudioSource source in busyAudioSources)
                {
                    source.volume = v;
                }
            };

            _clipsLookUp.Clear();
            foreach(AudioInterface.Audio audio in audioInterface.audios)
            {
                _clipsLookUp.Add(audio.id, audio.clip);
            }

            if(startPlayingMusic && currentMusicSource == null)
            {
                _PlayMusic(startPlayingMusicId, 0);
            }
        }

        private AudioClip GetClip(string audio)
        {
            if(audio == null)
                return null;

            if(_clipsLookUp.TryGetValue(audio, out AudioClip clip))
                return clip;
            
            return null;
        }

        private AudioSource SetupAudioSource()
        {
            if(freeAudioSources.Count > 0)
            {
                AudioSource source = freeAudioSources[0];
                freeAudioSources.RemoveAt(0);
                busyAudioSources.Add(source);

                source.gameObject.SetActive(true);

                return source;
            }

            return null;
        }

        public void _PlayMusic(string audio, float fade = 1f)
        {
            StartCoroutine(ChangeMusic(audio, fade));
        }

        private IEnumerator ChangeMusic(string audio, float fade)
        {
            bool isNew = currentMusicSource == null;

            AudioSource source = isNew ? SetupAudioSource() : currentMusicSource;

            if(source != null)
            {
                if(!isNew)
                {
                    while(musicVolumeMultiplier > 0)
                    {
                        musicVolumeMultiplier -= 1f/fade * 0.02f;
                        yield return new WaitForSeconds(0.02f);
                    }
                           
                    musicVolumeMultiplier = 1;
                }     
                else
                {
                    source.loop = true;
                    source.transform.parent = this.transform;
                    source.transform.localPosition = Vector3.zero;
                    
                    busyAudioSources.Remove(source); 
                    source.gameObject.name += "(Music)"; 

                    currentMusicSource = source;
                }

                source.volume = volumeMusic.value * musicVolumeMultiplier;
                source.clip = GetClip(audio);
                source.Play();    
            }   

            Debug.Log($"[AudioController] Playing Music {audio}!");
        }

        public void _PlayAudio(string audio)
        {
            _PlayAudio(audio, null, Vector3.zero);
        }

        public void _PlayAudio(string audio, Vector3 position)
        {
            _PlayAudio(audio, null, position);
        }
        
        public void _PlayAudio(string audio, Transform transform, Vector3 offset)
        {
            AudioSource source = SetupAudioSource();

            if(source != null)
            {
                source.loop = false;
            
                if(transform is null)
                {
                    source.transform.parent = this.transform;
                    source.transform.position = offset;
                }
                else
                {
                    source.transform.parent = transform;
                    source.transform.localPosition = offset;
                }

                source.volume = volumeVfx.value;
                source.clip = GetClip(audio);
                source.Play();

                Debug.Log($"[AudioController] Playing {audio} at {transform} + {offset}");
            
                return;
            }

            Debug.LogWarning($"[AudioController] Failed to play '{audio}': No available audio sources!");
        }

        private void FixedUpdate() 
        {
            for(int i = 0; i < busyAudioSources.Count; i ++)
            {
                AudioSource source = busyAudioSources[i];

                if(source == null || source is null)
                {
                    busyAudioSources.RemoveAt(i);
                    i --;
                }
                else if(!source.isPlaying)
                {
                    busyAudioSources.RemoveAt(i);
                    freeAudioSources.Add(source);

                    source.transform.parent = transform;
                    source.gameObject.SetActive(false);
                    i --;
                }
            }

            if(currentMusicSource != null)
                currentMusicSource.volume = volumeMusic.value * musicVolumeMultiplier;
        }
    
        #region Static Methods
        public static void PlayAudio(string audio, Transform transform, Vector3 offset)
        {
            _instance?._PlayAudio(audio, transform, offset);
        }

        public static void PlayAudio(string audio,Vector3 position)
        {
            _instance?._PlayAudio(audio, position);
        }

        public static void PlayAudio(string audio)
        {
            _instance?._PlayAudio(audio);
        }

        public static void PlayMusic(string audio, float fade = 1f)
        {
            _instance?._PlayMusic(audio, fade);
        }
        #endregion
    }
}