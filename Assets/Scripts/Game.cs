using System;
using UniRx;
using UnityEngine;

public class Game : MonoBehaviour {
    private Datastore _datastore;

    private GameObject currentStation;

    // Start is called before the first frame update
    void Start() {
        _datastore = GetComponent<Datastore>();

        Observable.EveryUpdate()
            .Where(_ => _datastore.nextStation != null)
            .Subscribe(_ => {
                var stationStop = _datastore.nextStation.transform.Find("Stop").position.x;
                var trainStop = _datastore.train.transform.Find("Stop").position.x;
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
    }
}