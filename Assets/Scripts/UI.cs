using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {
    public Canvas canvas;

    private Datastore _datastore;
    private TrainControlUI _trainControlUI;

    public void Start() {
        _datastore = GetComponent<Datastore>();

        _trainControlUI = new TrainControlUI {
            DistanceText = canvas.transform.Find("DistanceText").GetComponent<Text>(),
        };

        _datastore.distToNextStation.Subscribe(dist => {
            _trainControlUI.DistanceText.text = _datastore.roundedDistToNextStation + "m";
            
            var desiredPosition =
                _datastore.mainCamera.WorldToScreenPoint(_datastore.train.transform.Find("DistanceText").position);
            _trainControlUI.DistanceText.transform.position = desiredPosition;
        });
    }
}

public class TrainControlUI {
    public Slider Slider;
    public Text DistanceText;
}