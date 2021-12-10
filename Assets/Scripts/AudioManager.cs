using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

    public static AudioManager instance;

    public AudioMixerGroup mixerGroup;

    public Sound[] gunSounds;
    public Sound[] effects;
    public Sound[] meleeGrunts;

    void Awake() {
        if(instance != null) {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeSounds(gunSounds);
        InitializeSounds(effects);
        InitializeSounds(meleeGrunts);
    }

    void InitializeSounds(Sound[] array) {
        foreach(Sound s in array) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;

            s.source.outputAudioMixerGroup = mixerGroup;
        }
    }

    public void PlaySound(string sound, Sound[] array) {
        Sound s = Array.Find(array, item => item.name == sound);
        if(s == null) {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.Play();
    }

    public AudioSource PlayAtLocation(string sound, Sound[] array, Transform transform) {
        Sound s = Array.Find(array, item => item.name == sound);
        if(s == null) {
            Debug.LogWarning("Sound: " + sound + " not found!");
            return null;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        var g = new GameObject(sound);
        g.transform.SetParent(transform, false);

        var source = g.AddComponent<AudioSource>();
        source.clip = s.clip;
        source.loop = s.loop;
        source.volume = s.source.volume;
        source.pitch = s.source.pitch;
        source.outputAudioMixerGroup = mixerGroup;
        source.spatialBlend = 1f;

        source.Play();
        Destroy(g, source.clip.length + 0.5f);
        return source;
    }

}

[Serializable]
public class Sound {

    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = .5f;
    [Range(0f, 1f)]
    public float volumeVariance = .1f;

    [Range(.1f, 3f)]
    public float pitch = 1f;
    [Range(0f, 1f)]
    public float pitchVariance = .1f;

    public bool loop = false;

    public AudioMixerGroup mixerGroup;

    [HideInInspector]
    public AudioSource source;

}
