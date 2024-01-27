using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CDCGameKit
{
    public class SFXController : MonoBehaviour
    {
        TaskManager tm;
        Dictionary<string, AudioClip> clipLookup;
        ObjectPool pool;

        private AudioListener _listener;
        public AudioListener listener
        {
            get
            {
                if (_listener == null)
                {
                    _listener = GameObject.FindFirstObjectByType<AudioListener>();
                    if (_listener == null) Debug.LogWarning(this.GetType().ToString() + " - No AudioListner in this scene.");
                }
                return _listener;
            }
        }

#if UNITY_EDITOR
        //For monitoring
        [SerializeField] int tasksRunning, sourcesActive;
#endif
        #region Unity Events
        private void Awake()
        {
            _DestroyOrInit();
        }

        private void Update()
        {
            tm.Update();
            pool.Update();
            _MuteControls(); // AudioListener has volume controls -> integrate for refactor
#if UNITY_EDITOR
            tasksRunning = tm.tasksRunning;
            sourcesActive = pool.activePool.Count;
#endif
        }
        #endregion

        #region Support Functions
        bool _lastMute;
        void _MuteControls()
        {
            if (VolumePreferences.mute != _lastMute)
            {
                foreach (var active in pool.activePool) active.GetComponent<AudioSource>().mute = VolumePreferences.mute;
            }
        }
        void _DestroyOrInit() 
        {
            var found = GameObject.FindObjectsOfType<SFXController>();
            if (found.Length > 1) DestroyImmediate(this.gameObject);
            else 
            {
                DontDestroyOnLoad(this.gameObject);
                tm = new TaskManager();
                clipLookup = new Dictionary<string, AudioClip>();

                _Config();
            }
        }

        void _Config()
        {
            GameObject audioSoure = new GameObject("AudioSource GO (Prefab)");
            AudioSource AS = audioSoure.AddComponent<AudioSource>();
            AS.playOnAwake = false;
            AS.loop = false;
            AS.spatialBlend = 0;
            AS.rolloffMode = AudioRolloffMode.Custom;
            AS.SetCustomCurve(AudioSourceCurveType.CustomRolloff, SFX.SPATIALCURVE);
            AS.maxDistance = 10f;
            AS.spread = 40;
            AS.dopplerLevel = 0;
            audioSoure.SetActive(false);
            pool = new ObjectPool(audioSoure);

            pool.Initialize(ReadyToDeactivate, OnCreate, OnActivate, OnDeactivate);
        }

        AudioClip GetClip(string name)
        {
            AudioClip clip = null;
            clipLookup.TryGetValue(name, out clip);

            if (clip != null) return clip;
            else
            {
                clip = Resources.Load<AudioClip>("SFX/" + name);
                if (clip != null) return clip;
                else 
                {
                    Debug.LogError(this.GetType().ToString() + " ERROR - no clip called [" + name + "] found in Resources/SFX");
                    return null;
                }
            }
        }
        AudioClip GetDialogue(string name)
        {
            AudioClip clip = null;
            clipLookup.TryGetValue(name, out clip);

            if (clip != null) return clip;
            else
            {
                clip = Resources.Load<AudioClip>("Dialogue/" + name);
                if (clip != null) return clip;
                else
                {
                    Debug.LogError(this.GetType().ToString() + " ERROR - no clip called [" + name + "] found in Resources/Dialogue");
                    return null;
                }
            }
        }
        #endregion

        #region Public OneShot Functions
        #region 2D SFX
        private AudioSource _OneShot2D(string name)
        {
            var SFXGO = pool.GetOne();
            var AS = SFXGO.GetComponent<AudioSource>();
            var clip = GetClip(name);   // basic clip

            if (clip != null)   // 2D setup
            {
                AS.loop = false;
                AS.priority = 0;
                AS.spatialBlend = 0;
                AS.Stop();
                AS.clip = clip;
                AS.Play();
            }
            return AS;
        }
        public void OneShotUI(string name)
        {
            var AS = _OneShot2D(name);
            AS.volume = VolumePreferences.mute ? 0 : VolumePreferences.master * VolumePreferences.ui;   // UI volumes
        }
        public void OneShotDialogue(string name)
        {
            // Does not use _OneShot2D because clips are in Resources/Dialogue
            var SFXGO = pool.GetOne();
            var AS = SFXGO.GetComponent<AudioSource>();
            var clip = GetDialogue(name);   // dialogue clip

            if (clip != null)   // 2D setup
            {
                AS.loop = false;
                AS.priority = 50;
                AS.spatialBlend = 0;
                AS.volume = VolumePreferences.mute ? 0 : VolumePreferences.master * VolumePreferences.dialogue;  // dialogue volumes
                AS.Stop();
                AS.clip = clip;
                AS.Play();
            }
        }
        public void OneShotEnv(string name)
        {
            var AS = _OneShot2D(name);
            AS.volume = VolumePreferences.mute ? 0 : VolumePreferences.master * VolumePreferences.environment;  // env volumes
        }
        public void OneShotSFX(string name)
        {
            var AS = _OneShot2D(name);
            AS.volume = VolumePreferences.mute ? 0 : VolumePreferences.master * VolumePreferences.sfx;  // sfx volumes
        }
        #endregion
        #region 3D SFX
        protected GameObject _OneShotEnv(string name)
        {
            var SFXGO = pool.GetOne();
            var AS = SFXGO.GetComponent<AudioSource>();
            var clip = GetClip(name);   // basic clip

            if (clip != null)   // 3D setup
            {
                AS.loop = false;
                AS.priority = 100;
                AS.spatialBlend = 1f;
                AS.volume = VolumePreferences.mute ? 0 : VolumePreferences.master * VolumePreferences.environment;  // env volumes
                AS.Stop();
                AS.clip = clip;
                AS.Play();
            }
            return SFXGO;
        }
        protected GameObject _OneShotSFX(string name)
        {
            var SFXGO = pool.GetOne();
            var AS = SFXGO.GetComponent<AudioSource>();
            var clip = GetClip(name);   // basic clip

            if (clip != null)   // 3D setup
            {
                AS.loop = false;
                AS.priority = 200;
                AS.spatialBlend = 1;
                AS.volume = VolumePreferences.mute ? 0 : VolumePreferences.master * VolumePreferences.sfx;  // env volumes
                AS.Stop();
                AS.clip = clip;
                AS.Play();
            }
            return SFXGO;
        }

        public void OneShotEnv(string name, Vector3 where)
        {
            if (listener == null)
            {
                Debug.LogWarning(this.GetType().ToString() + " - Cannot schedule oneshot. No listener.");
                return;
            }
            else 
            { 
                var delay = new DelayTask(name, where, DelayTask.SpatialSoundType.Environmental, this, listener);
                tm.Do(delay);
            }
        }
        public void OneShotSFX(string name, Vector3 where)
        {
            if (listener == null)
            {
                Debug.LogWarning(this.GetType().ToString() + " - Cannot schedule oneshot. No listener.");
                return;
            }
            else
            {
                var delay = new DelayTask(name, where, DelayTask.SpatialSoundType.SFX, this, listener);
                tm.Do(delay);
            }
        }
        public void OneShotFollowEnv(string name, Transform who)
        {
            var go = _OneShotEnv(name);
            var task = new FollowTask(who, go.transform);
            tm.Do(task);
        }
        public void OneShotFollowSFX(string name, Transform who)
        {
            var go = _OneShotSFX(name);
            var task = new FollowTask(who, go.transform);
            tm.Do(task);
        }
        #endregion
        #endregion

        #region Public Loop Functions
        public void LoopEnv(string name, Vector3 where)
        {
            var go = _OneShotEnv(name);
            var AS = go.GetComponent<AudioSource>();
            AS.loop = true;
            go.transform.position = where;
        }
        public void LoopSFX(string name, Vector3 where)
        {
            var go = _OneShotSFX(name);
            var AS = go.GetComponent<AudioSource>();
            AS.loop = true;
            go.transform.position = where;
        }
        public void LoopSFX(string name, Transform who)
        {
            var go = _OneShotSFX(name);
            var AS = go.GetComponent<AudioSource>();
            AS.loop = true;
            var task = new FollowTask(who, go.transform);
            tm.Do(task);
        }
        #endregion

        #region Object Pool Handlers
        public bool ReadyToDeactivate(GameObject toTest)
        {
            //Debug.Log(toTest.name + " isPlaying is " + toTest.GetComponent<AudioSource>().isPlaying);
            return !toTest.GetComponent<AudioSource>().isPlaying;
        }
        public void OnCreate(GameObject created)
        {
            created.gameObject.name = "AudioSource GO";
            created.transform.SetParent(transform);
        }
        public void OnActivate(GameObject activated)
        {
            activated.SetActive(true);
            var AS = activated.GetComponent<AudioSource>();
            AS.mute = VolumePreferences.mute;
        }
        public void OnDeactivate(GameObject deactivated)
        {
            deactivated.SetActive(false);
            var AS = deactivated.GetComponent<AudioSource>();
            AS.mute = VolumePreferences.mute;
        }
        #endregion

        #region Tasks
        public class DelayTask : Task
        {
            const float SPEEDOFSOUD = 343 * .5f;
            float speedOfSoundFactor;
            string name;
            Vector3 sourcePosition;
            float delayTime, timer;
            public enum SpatialSoundType { Environmental, SFX };
            private SpatialSoundType type;
            SFXController context;
            AudioListener listener;
            public DelayTask(string name, Vector3 sourcePosition, SpatialSoundType type, SFXController context, AudioListener listener, float speedOfSoundFactor = 1)
            {
                this.name = name;
                this.sourcePosition = sourcePosition;
                this.type = type;
                this.context = context;
                this.listener = listener;
                this.speedOfSoundFactor = Mathf.Clamp01(speedOfSoundFactor);
                CalculateDelay();
            }

            internal override void Update()
            {
                timer += Time.deltaTime;
                if (timer >= delayTime)
                {
                    PlayDelayed();
                    SetStatus(TaskStatus.Success);
                }
            }

            public void CalculateDelay()
            {
                delayTime = Vector3.Distance(listener.transform.position, sourcePosition) / (SPEEDOFSOUD * speedOfSoundFactor);
            }

            public void PlayDelayed()
            {
                GameObject go = null;
                if (type == SpatialSoundType.Environmental) go = context._OneShotEnv(name);
                else go = context._OneShotSFX(name);
                go.transform.position = sourcePosition;
            }
        }

        public class FollowTask : Task
        {
            Transform toFollow;
            Transform source;

            public FollowTask(Transform who, Transform sourceTransform)
            {
                this.toFollow = who;
                this.source = sourceTransform;
            }
            
            internal override void Update()
            {
                if (source.gameObject.activeSelf == false)
                {
                    SetStatus(TaskStatus.Success);
                    return;
                }
                else if (toFollow == null)
                {
                    source.GetComponent<AudioSource>().Stop();
                    SetStatus(TaskStatus.Success);
                    return;
                }
                else source.position = toFollow.position;
            }
        }
        #endregion
    }

    public static class SFX
    {
        private static SFXController _instance;
        public static SFXController instance 
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("SFX Controller");
                    _instance = go.AddComponent<SFXController>();
                    VolumePreferences.LoadFromPrefs();
                }
                return _instance;
            }
        }

        private static AnimationCurve _curve;
        public static AnimationCurve SPATIALCURVE
        {
            get
            {
                if (_curve == null) _curve = new AnimationCurve(new Keyframe(5, 1, 0, -.13f), new Keyframe(14, .3f, -.04f, -.04f), new Keyframe(30, 0, 0, 0));
                return _curve;
            }
        }
    }

    public static class VolumePreferences
    {
        private static bool loadAttempted;

        private static float _master;
        private static float _ui;
        private static float _environment;
        private static float _music;
        private static float _dialogue;
        private static float _sfx;
        private static bool _mute;

        public static float master { get { return _master; } set { if (value != _master) { _master = value; if (loadAttempted) SaveToPrefs(); } } }
        public static float ui { get { return _ui; } set { if (value != _ui) { _ui = value; if (loadAttempted) SaveToPrefs(); } } }
        public static float environment { get { return _environment; } set { if (value != _environment) { _environment = value; if (loadAttempted) SaveToPrefs(); } } }
        public static float music { get { return _music; } set { if (value != _music) { _music = value; if (loadAttempted) SaveToPrefs(); } } }
        public static float dialogue { get { return _dialogue; } set { if (value != _dialogue) { _dialogue = value; if (loadAttempted) SaveToPrefs(); } } }
        public static float sfx { get { return _sfx; } set { if (value != _sfx) { _sfx = value; if (loadAttempted) SaveToPrefs(); } } }
        public static bool mute { get { return _mute; } set { if (value != _mute) { _mute = value; if (loadAttempted) SaveToPrefs(); } } }

        public static void SaveToPrefs()
        {
            PlayerPrefs.SetFloat("volume_master", master);
            PlayerPrefs.SetFloat("volume_ui", ui);
            PlayerPrefs.SetFloat("volume_environment", environment);
            PlayerPrefs.SetFloat("volume_music", music);
            PlayerPrefs.SetFloat("volume_dialogue", dialogue);
            PlayerPrefs.SetFloat("volume_sfx", sfx);
            PlayerPrefs.SetInt("volume_mute", mute ? 1 : 0);
        }

        public static void LoadFromPrefs()
        {
            master = PlayerPrefs.GetFloat("volume_master", 1);
            ui = PlayerPrefs.GetFloat("volume_ui", 1);
            environment = PlayerPrefs.GetFloat("volume_environment", 1);
            music = PlayerPrefs.GetFloat("volume_music", 1);
            dialogue = PlayerPrefs.GetFloat("volume_dialogue", 1);
            sfx = PlayerPrefs.GetFloat("volume_sfx", 1);
            mute = PlayerPrefs.GetInt("volume_mute", 0) == 1 ? true : false;
        }
    }
}
