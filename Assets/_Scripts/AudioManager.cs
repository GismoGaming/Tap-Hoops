using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Gismo.Core.Audio
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        [SerializeField] private Audio[] sounds;
        [SerializeField] private GameObject audioPlayer;
        [SerializeField] private AudioMixer mixer;
        private static Dictionary<string, float> soundTimerDictionary;

        public static AudioManager instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

            soundTimerDictionary = new Dictionary<string, float>();

            foreach (Audio sound in sounds)
            {
                sound.source = audioPlayer.AddComponent<AudioSource>();
                sound.source.clip = sound.clip;

                sound.source.volume = sound.volume;
                sound.source.pitch = sound.pitch;
                sound.source.loop = sound.isLoop;

                sound.source.playOnAwake = false;

                if (string.IsNullOrEmpty(sound.audioGroup))
                {
                    sound.source.outputAudioMixerGroup = mixer.FindMatchingGroups("Master")[0];
                }
                else
                {
                    sound.source.outputAudioMixerGroup = mixer.FindMatchingGroups($"Master/{sound.audioGroup}")[0];
                }

                if (sound.hasCooldown)
                {
                    Debug.Log(sound.name);
                    soundTimerDictionary[sound.name] = 0f;
                }
            }
        }

        public void PlayAudio(string name)
        {
            Audio sound = Array.Find(sounds, s => s.name == name);

            if (sound == null)
            {
                Debug.LogError("Sound " + name + " Not Found!");
                return;
            }

            if (!CanPlaySound(sound)) return;

            sound.source.Play();
        }

        public static void Play(string name)
        {
            instance.PlayAudio(name);
        }

        public void StopAudio(string name)
        {
            Audio sound = Array.Find(sounds, s => s.name == name);

            if (sound == null)
            {
                Debug.LogError("Sound " + name + " Not Found!");
                return;
            }

            sound.source.Stop();
        }

        public static void Stop(string name)
        {
            instance.StopAudio(name);
        }

        private static bool CanPlaySound(Audio sound)
        {
            if (!sound.source.gameObject.activeInHierarchy)
                return false;

            if (soundTimerDictionary.ContainsKey(sound.name))
            {
                float lastTimePlayed = soundTimerDictionary[sound.name];

                if (lastTimePlayed + sound.clip.length < Time.time)
                {
                    soundTimerDictionary[sound.name] = Time.time;
                    return true;
                }

                return false;
            }

            return true;
        }
    }
}