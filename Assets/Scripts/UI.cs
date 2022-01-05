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
            Slider = canvas.transform.Find("Slider").GetComponent<Slider>(),
        };

        _trainControlUI.Slider.OnValueChangedAsObservable().Subscribe(newVal => {
            _datastore.throttleValue = newVal;
        });
    }
}

public class TrainControlUI {
    public Slider Slider;
}
