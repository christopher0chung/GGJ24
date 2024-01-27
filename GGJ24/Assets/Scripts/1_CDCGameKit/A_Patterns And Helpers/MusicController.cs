using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDCGameKit
{
    public class MusicController : MonoBehaviour
    {
        private AudioSource AS1;
        private AudioSource AS2;

        private bool AS1IsMain;

        TaskManager tm;

        private void Awake()
        {
            AS1 = gameObject.AddComponent<AudioSource>();          
            AS2 = gameObject.AddComponent<AudioSource>();          
            
            AS1.loop = AS2.loop = true;
            AS1.playOnAwake = AS2.playOnAwake = false;
            AS1.spatialBlend = AS2.spatialBlend = 0;
            AS1.volume = AS2.volume = VolumePreferences.mute ? 0 : VolumePreferences.master * VolumePreferences.music;
            
            AS1.Stop();
            AS2.Stop();

            tm = new TaskManager();
        }

        private void Update()
        {
            if (VolumePreferences.mute != AS1.mute) AS1.mute = AS2.mute = VolumePreferences.mute;
        }

        public void CrossFadeNewTrack(string name)
        {
            AudioSource nextSource = AS1IsMain ? AS2 : AS1;
        }
    }

    public static class Music
    {
        private static MusicController _instance;
        public static MusicController instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("Music Controller");
                    _instance = go.AddComponent<MusicController>();
                    VolumePreferences.LoadFromPrefs();
                }
                return _instance;
            }
        }
    }
}
