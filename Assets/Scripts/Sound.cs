using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour {
    private Datastore _datastore;
    private AudioMixer _audioMixer;
    private AudioMixerGroup _masterAudioMixerGroup;
    
    public List<AudioClip> ambienceClips;
    public AudioSource ambienceSource;
    void Start() {
        _datastore = GetComponent<Datastore>();
        _audioMixer = Resources.Load<AudioMixer>("MainMixer");
        _masterAudioMixerGroup = _audioMixer.FindMatchingGroups("Master")[0];
        
        (ambienceSource, ambienceClips) = loadClips(1, "Sounds/station_ambience_");
        
        ambienceSource.gameObject.name = "Ambience source";
        ambienceSource.volume = 0.2f;
        ambienceSource.clip = ambienceClips.Single();
        ambienceSource.loop = true;
        ambienceSource.Play();
    }

    void FixedUpdate() {
        var distance = Math.Min(_datastore.distFromLastStation.Value, _datastore.distToNextStation.Value);
        if (distance > 100) {
            ambienceSource.volume = 0;
        }
        else {
            ambienceSource.volume = (float) (-Math.Min(Math.Abs(distance), 100) + 100) / 100;    
        }
        
    }
    
    (AudioSource, List<AudioClip>) loadClips(int numClips, string pathPrefix, string numSuffixLength="D2", int startIndex=1) {
        var clips = new List<AudioClip>();
        var source = new GameObject();
        source.transform.parent = this.transform;
        source.AddComponent<AudioSource>();
        for (var i = startIndex; i < startIndex + numClips; i++) {
            clips.Add(Resources.Load<AudioClip>($"{pathPrefix}{i.ToString(numSuffixLength)}"));
        }
        AudioSource audioSource = source.GetComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = _masterAudioMixerGroup;
        return (audioSource, clips);
    }
}
