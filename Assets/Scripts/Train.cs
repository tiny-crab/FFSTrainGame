using System;
using System.Collections.Generic;
using UnityEngine;

public class Train : MonoBehaviour {
    Datastore _datastore;
    
    public double velocity;
    public double friction = 0.1;
    void Start() {
        _datastore = GameObject.Find("Game").GetComponent<Datastore>();
    }

    void Update() {
        velocity = CalculateNewVelocity(_datastore.throttleValue, velocity);
        transform.position = CalculateNewPosition(velocity, transform.position);
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
