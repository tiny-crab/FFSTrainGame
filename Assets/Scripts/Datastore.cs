using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Datastore : MonoBehaviour {
    public UnityEngine.Camera mainCamera;
    
    public double throttleValue = 0;

    public MessageBroker events;

    public Train train;
    public GameObject nextStation = null;
    public ReactiveProperty<GameObject> currentTrack = new ReactiveProperty<GameObject>();
    public DoubleReactiveProperty distToNextStation = new DoubleReactiveProperty(Double.MinValue);
    public IntReactiveProperty roundedDistToNextStation = new IntReactiveProperty(0);

    public int score;
}
