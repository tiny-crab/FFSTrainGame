using System;
using UniRx;
using UnityEngine;

public class Datastore : MonoBehaviour {
    public UnityEngine.Camera mainCamera;
    
    public BoolReactiveProperty brake = new BoolReactiveProperty(); // if brake is being applied

    public MessageBroker inputEvents = new MessageBroker();

    public Train train;
    public GameObject nextStation = null;
    public Vector3 posOfLastStation = Vector3.negativeInfinity;
    public ReactiveProperty<GameObject> currentTrack = new ReactiveProperty<GameObject>();
    public DoubleReactiveProperty distToNextStation = new DoubleReactiveProperty(Double.MinValue);
    public DoubleReactiveProperty distFromLastStation = new DoubleReactiveProperty(0);
    public IntReactiveProperty roundedDistToNextStation = new IntReactiveProperty(0);

    public int score;
}
