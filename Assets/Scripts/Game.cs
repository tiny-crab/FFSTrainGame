using System;
using UniRx;
using UnityEngine;

public class Game : MonoBehaviour {
    private Datastore _datastore;
    private MouseAndKeyboard _mouseAndKeyboard;

    private GameObject currentStation;
    private GameObject nextGate;

    // Start is called before the first frame update
    void Start() {
        _datastore = GetComponent<Datastore>();

        Observable.EveryFixedUpdate()
            .Where(_ => _datastore.nextStation != null)
            .Subscribe(_ => {
                var stationStop = _datastore.nextStation.transform.Find("Stop").position.x;
                var trainStop = _datastore.train.transform.Find("Nose").position.x;
                _datastore.distToNextStation.Value = stationStop - trainStop;

                if (_datastore.distToNextStation.Value >= 0) {
                    var dist = _datastore.distToNextStation.Value;
                    var distanceString = dist.ToString("0");
                    _datastore.roundedDistToNextStation.Value = (int)Math.Floor(dist);
                    switch (distanceString.Length) {
                        case 4:
                            _datastore.roundedDistToNextStation.Value = (int)Math.Floor(dist / 1000d) * 1000;
                            break;
                        case 3:
                            _datastore.roundedDistToNextStation.Value = (int)Math.Floor(dist / 100d) * 100;
                            break;
                        case 2:
                            _datastore.roundedDistToNextStation.Value = (int)Math.Floor(dist / 10d) * 10;
                            break;
                    }
                }

                _datastore.train.distanceText.text = _datastore.roundedDistToNextStation.Value + "m";
            });

        Observable.EveryFixedUpdate()
            .Where(_ => _datastore.posOfLastStation != Vector3.negativeInfinity)
            .Subscribe(_ => {
                var trainStop = _datastore.train.transform.Find("Nose").position.x;
                _datastore.distFromLastStation.Value = trainStop - _datastore.posOfLastStation.x;
            });

        Observable.EveryUpdate()
            .Where(_ => 
                _datastore.train.velocity == 0 
                && _datastore.distToNextStation.Value <= 15
                && currentStation != _datastore.nextStation)
            .Subscribe(_ => {
                _datastore.score += 30 - (int) Math.Floor(Math.Abs(_datastore.distToNextStation.Value));
                currentStation = _datastore.nextStation;
            });

        _datastore.currentTrack
            .Where(track => track.name.Contains("Gate"))
            .Subscribe(track => {
                nextGate = track;
            });

        Observable.EveryUpdate()
            .Where(_ => nextGate != null)
            .Where(_ => {
                var trainNose = _datastore.train.transform.Find("Nose").position.x;
                var gateThreshold = _datastore.currentTrack.Value.transform.Find("Stop").position.x;
                return trainNose >= gateThreshold;
            })
            .Subscribe(_ => {
                var velocityThreshold = _datastore.currentTrack.Value.GetComponent<Gate>().speedValue;
                Debug.Log($"Train going {_datastore.train.velocity} when passing gate with threshold {velocityThreshold}");
                if (Math.Abs(_datastore.train.velocity - velocityThreshold) < 0.5) {
                    _datastore.score++;
                }
                nextGate = null;
            });
    }
}