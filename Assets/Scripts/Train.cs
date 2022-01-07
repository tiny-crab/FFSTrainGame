using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

public class Train : MonoBehaviour {
    Datastore _datastore;
    
    public double velocity;
    public double friction = 0.1;

    public List<GameObject> cars;
    void Start() {
        _datastore = GameObject.Find("Game").GetComponent<Datastore>();
        cars = Enumerable.Range(0, 3).Select(i => transform.Find($"Car{i}").gameObject).ToList();
    }

    void Update() {
        velocity = CalculateNewVelocity(_datastore.throttleValue, velocity);
        transform.position = CalculateNewPosition(velocity, transform.position);
        if (Utils.rng.Next(500) == 0 && velocity > 0.5) {
            RumbleCars();
        }
    }

    private void RumbleCars() {
        for (int i = 0; i < cars.Count; i++) {
            var tween = cars[i].transform.DOShakePosition(
                duration: 0.2f,
                strength: new Vector3(0, 0.05f, 0),
                vibrato: 0,
                randomness: 0,
                snapping: false,
                fadeOut: true
            );
            tween.SetDelay(0.2f * i);
            tween.Play();
        }
    }

    private static double CalculateNewVelocity(double throttleValue, double velocity) {
        var targetVelocity = throttleValue * 0.2f;
        return targetVelocity >= velocity
            // TODO introduce an exponential accel curve here instead of a linear 0.01 accel
            ? Math.Min(targetVelocity, velocity + 0.01) 
            : Math.Max(targetVelocity, velocity - 0.01);
    }

    private static Vector3 CalculateNewPosition(double velocity, Vector3 position) {
        return new Vector3 {
            x = position.x + (float)velocity,
            y = position.y,
            z = position.z
        };
    }
}
