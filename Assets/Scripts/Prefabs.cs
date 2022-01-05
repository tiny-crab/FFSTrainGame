using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Prefabs : MonoBehaviour {
    // Stations and Tracks consist of a parent GameObject with multiple Tilemaps beneath
    public List<GameObject> stations = new List<GameObject>();
    public List<GameObject> tracks = new List<GameObject>();

    public void Start() {
        stations = Enumerable.Range(0, 0).Select(i => Resources.Load<GameObject>($"Prefabs/Stations/Station{i}")).ToList();
        tracks = Enumerable.Range(0, 2).Select(i => Resources.Load<GameObject>($"Prefabs/Tracks/Track{i}")).ToList();
    }
}
