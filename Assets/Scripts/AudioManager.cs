
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public Sound[] sounds;
    public Sound[] music;
    public Sound[] UI;
    public Sound currentSong;
    public static AudioManager instance;

    [System.Serializable]
    public class AudioTypes {
        public string name;
        public List<AudioSource> oneshots = new List<AudioSource>();

        public AudioTypes(string name_) {
            name = name_;
        }

        public void CheckExpired() {
            for(int i = oneshots.Count - 1; i >= 0; i--) {
                if(!oneshots[i])
                    oneshots.RemoveAt(i);
            }
        }
    }
    AudioTypes[] audioTypes;

    GameObject audioParent;
    void Awake() {
        if(instance == null)
            instance = this;
        else {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        InitializeSounds(sounds, Settings.Sounds / 100, 1.0f);

        //InitializeSounds(music, Settings.Music / 1000, 0.0f);
        //musicNum = Random.Range(0, music.Length - 1); //random starting position in list of songs
        //currentSong = music[musicNum];
        audioTypes = new AudioTypes[sounds.Length];
        for(int i = 0; i < audioTypes.Length; i++) {
            audioTypes[i] = new AudioTypes(sounds[i].name);
        }
        InvokeRepeating("CleanOneShots", 0, 0.05f);
        audioParent = new GameObject("Audio Sources");
        DontDestroyOnLoad(audioParent);
    }

    public static bool Main = false;
    private void LateUpdate() {
        if(Main) {
            StartCoroutine(BackgroundMusic());
            Main = false;
        }
    }

    void InitializeSounds(Sound[] sounds, float volumeScale, float spacial) {
        foreach(Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.volume *= volumeScale;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.spatialBlend = spacial;
        }
    }

    public void UpdateVolume() {
        foreach(Sound s in music) {
            s.source.volume = s.volume = Settings.Music / 1000;
        }
        foreach(Sound s in sounds) {
            s.source.volume = s.volume = Settings.Sounds / 100;
        }
        foreach(Sound s in UI) {
            s.source.volume = s.volume = Settings.Sounds / 100;
        }
    }

    public void Stop(string name) {
        Sound s = System.Array.Find(sounds, sounds => sounds.name == name);
        if(s == null) {
            Debug.LogWarning("Sound: \"" + name + "\" not found");
            return;
        }
        s.source.Stop();
    }

    public static void StaticStop(string name) {
        instance.Stop(name);
    }

    public static void StaticStopAllSounds() {
        foreach(var s in instance.sounds) {
            instance.Stop(s.name);
        }
    }

    AudioSource PlayClipAt(AudioClip clip_, Vector3 pos_, [UnityEngine.Internal.DefaultValue("1.0F")] float volume_) {
        var tmp = new GameObject("Oneshot Audio");
        tmp.transform.SetParent(audioParent.transform, true);
        tmp.transform.position = pos_;
        var source = tmp.AddComponent<AudioSource>();
        source.clip = clip_;
        source.volume = volume_;
        source.spatialBlend = 1;
        source.Play();
        Destroy(tmp, clip_.length);
        return source;
    }

    void CleanOneShots() {
        foreach(var t in audioTypes) {
            if(t != null) {
                t.CheckExpired();
            }
        }
    }

    public void PlayEffect(Sound[] soundsList, string name, Vector3 position) {
        Sound s = System.Array.Find(soundsList, sounds => sounds.name == name);
        if(s == null) {
            Debug.LogWarning("Sound: \"" + name + "\" not found");
            return;
        }
        for(int i = 0; i < audioTypes.Length - 1; i++) {
            var ii = audioTypes[i];
            if(ii.name == name) {
                if(ii.oneshots.Count <= 100) {
                    ii.oneshots.Add(PlayClipAt(s.clip, position, s.volume));
                }
            }
        }
    }

    public static void StaticPlayEffect(Sound[] soundsList, string name, Vector3 position) {
        instance.PlayEffect(soundsList, name, position);
    }

    public int musicNum;
    IEnumerator BackgroundMusic() {
        currentSong = music[musicNum];
        currentSong.source.PlayOneShot(currentSong.clip);

        musicNum++;
        if(musicNum > music.Length - 1)
            musicNum = 0;

        yield return new WaitForSecondsRealtime(currentSong.clip.length);

        StartCoroutine(BackgroundMusic());
    }

    public void NextSong() {
        currentSong.source.Stop();
        StopAllCoroutines();
        StartCoroutine(BackgroundMusic());
    }

    public static void StaticNextSong() {
        instance.NextSong();
    }

    public string GetSongName() {
        return currentSong.name;
    }

    public static string StaticGetSongName() {
        return instance.GetSongName();
    }
}


[System.Serializable]
public class Sound {

    public string name;

    public AudioClip clip;

    [Range(0.0f, 1.0f)]
    public float volume;
    [Range(0.1f, 3.0f)]
    public float pitch;

    [HideInInspector]
    public AudioSource source;
}
