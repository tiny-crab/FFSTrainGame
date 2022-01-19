using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class Train : MonoBehaviour {
    private Datastore _datastore;

    public double velocity;
    public double targetVelocity;
    public double maxVelocity = 0.25;
    public float boostTriggeredTimestamp;
    public float boostDelta;

    public List<GameObject> cars;
    public TextMesh distanceText;
    void Start() {
        _datastore = GameObject.Find("Game").GetComponent<Datastore>();
        distanceText = transform.Find("DistanceText").GetComponent<TextMesh>();
        cars = Enumerable.Range(0, 3).Select(i => transform.Find($"Car{i}").gameObject).ToList();

        _datastore.brake.Subscribe(brakeActive => {
            if (velocity != 0) return;
            if (brakeActive) {
                boostTriggeredTimestamp = Time.time;
                boostDelta = 0;
            }
            else {
                boostDelta = Time.time - boostTriggeredTimestamp;
                boostTriggeredTimestamp = 0;
            }
        });
    }

    void FixedUpdate() {
        targetVelocity = maxVelocity * CalculateBoostVelocity(boostDelta);
        velocity = CalculateNewVelocity();
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

    private double CalculateNewVelocity() {
        if (_datastore.brake.Value) {
            return Math.Max(0, velocity - 0.02);
        }

        var linearAccel = 0.01;
        if (boostDelta != 0) {
            linearAccel *= CalculateBoostAcceleration(boostDelta);
        }

        if (targetVelocity > velocity) {
            // TODO introduce an exponential accel curve here instead of a linear acceleration value
            return Math.Min(targetVelocity, velocity + linearAccel);
        } else {
            targetVelocity = maxVelocity;
            boostDelta = 0;
            return Math.Max(targetVelocity, velocity - 0.01);
        }
    }

    private static Vector3 CalculateNewPosition(double velocity, Vector3 position) {
        return new Vector3 {
            x = position.x + (float)velocity,
            y = position.y,
            z = position.z
        };
    }

    private double CalculateBoostAcceleration(float timestampDelta) {
        // only boost if brake has been held down for more than a second
        if (timestampDelta < 1) return 1;
        // clamp boost time to 5 seconds or less
        var delta = Math.Min(5, timestampDelta);
        // at one second, delta should yield 3 times acceleration on a diminishing returns curve
        return Math.Sqrt(delta) * 3;
    }

    private double CalculateBoostVelocity(float timestampDelta) {
        // only boost if brake has been held down for more than a second
        if (timestampDelta < 1) return maxVelocity;
        // clamp boost time to 5 seconds or less
        var delta = Math.Min(5, timestampDelta);
        // at one second, the max velocity should be 1.5 times max velocity on a diminishing returns curve
        return Math.Sqrt(delta) * 1;
    }
}
