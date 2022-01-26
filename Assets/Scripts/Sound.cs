using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour {
    private Datastore _datastore;
    private AudioMixer _audioMixer;
    private AudioMixerGroup _masterAudioMixerGroup;
    
    public List<AudioClip> natureClips;
    public AudioSource natureSource;
    
    public List<AudioClip> pianoClips;
    public AudioSource pianoSource;
    
    public List<AudioClip> synthClips;
    public AudioSource synthSource;
    void Start() {
        _datastore = GetComponent<Datastore>();
        _audioMixer = Resources.Load<AudioMixer>("MainMixer");
        _masterAudioMixerGroup = _audioMixer.FindMatchingGroups("Master")[0];
        
        (natureSource, natureClips) = loadClips(1, "Sounds/station_ambience_");
        (pianoSource, pianoClips) = loadClips(1, "Sounds/piano_ambience_");
        (synthSource, synthClips) = loadClips(1, "Sounds/synth_ambience_");
        
        natureSource.gameObject.name = "Ambience source";
        natureSource.volume = 0.2f;
        natureSource.clip = natureClips.Single();
        natureSource.loop = true;
        natureSource.Play();
        
        pianoSource.gameObject.name = "Piano source";
        pianoSource.volume = 0.2f;
        pianoSource.clip = pianoClips.Single();
        pianoSource.loop = true;
        pianoSource.Play();
        
        synthSource.gameObject.name = "Synth source";
        synthSource.volume = 0.2f;
        synthSource.clip = synthClips.Single();
        synthSource.loop = true;
        synthSource.Play();
    }

    void FixedUpdate() {
        var distance = Math.Min(_datastore.distFromLastStation.Value, _datastore.distToNextStation.Value);
        if (distance > 100) {
            natureSource.volume = 0;
        }
        else {
            natureSource.volume = (float) (-Math.Min(Math.Abs(distance), 100) + 100) / 100;
        }
        synthSource.volume = Math.Min(0.3f, 0.3f - natureSource.volume);
        pianoSource.volume = synthSource.volume +
                             synthSource.volume *
                             (float)((Math.Min(Math.Abs(_datastore.train.velocity),
                                          _datastore.train.maxVelocity) -
                                      _datastore.train.maxVelocity) /
                                     _datastore.train.maxVelocity);

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
