using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gismo.Core.Audio
{
    public class AudioToggleButton : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image image;
        [SerializeField] private Sprite audioEnabled;
        [SerializeField] private Sprite audioDisabled;

        [SerializeField] private GameObject audioPlayers;

        private const string AUDIOKEY = "Audio Enabled";

        private bool isAudioEnabled;

        private void Awake()
        {
            if(PlayerPrefs.GetInt(AUDIOKEY,0) == 0)
            {
                isAudioEnabled = true;
            }
            else
            {
                isAudioEnabled = false;
            }
            audioPlayers.SetActive(isAudioEnabled);
            UpdateUI();
        }

        public void OnToggle()
        {
            isAudioEnabled = !isAudioEnabled;
            audioPlayers.SetActive(isAudioEnabled);

            if (isAudioEnabled)
            {
                PlayerPrefs.SetInt(AUDIOKEY, 0);
            }
            else
            {
                PlayerPrefs.SetInt(AUDIOKEY, 1);
            }

            UpdateUI();
        }

        void UpdateUI()
        {
            if(isAudioEnabled)
            {
                image.sprite = audioEnabled;
            }
            else
            {
                image.sprite = audioDisabled;
            }
        }
    }
}