using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Prefabs : MonoBehaviour {
    // Stations and Tracks consist of a parent GameObject with multiple Tilemaps beneath
    // The Track child GO should have a Tilemap component that is wider than all other "decoration" tilemaps  
    public List<GameObject> stations = new List<GameObject>();
    public List<GameObject> tracks = new List<GameObject>();
    public List<GameObject> gates = new List<GameObject>();

    public void Awake() {
        stations = Enumerable.Range(0, 1).Select(i => Resources.Load<GameObject>($"Prefabs/Stations/Station{i}")).ToList();
        tracks = Enumerable.Range(0, 2).Select(i => Resources.Load<GameObject>($"Prefabs/Tracks/Track{i}")).ToList();
        gates = Enumerable.Range(0, 1).Select(i => Resources.Load<GameObject>($"Prefabs/Gates/Gate{i}")).ToList();
    }
}
