using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour {
    private Prefabs _prefabs;
    private Datastore _datastore;

    public Grid grid;

    public int tracksBetweenLandmarks = 3;
    public int numGates = 1;
    public List<GameObject> generatedTracks;
    public double endOfTracksetTrigger = 0;
    public int trackActivationBuffer = 3;
    public IntReactiveProperty currentTrackIndex = new IntReactiveProperty(0);

    public void Start() {
        _prefabs = GetComponent<Prefabs>();
        _datastore = GetComponent<Datastore>();

        generatedTracks = GenerateNewTracks(1, _prefabs.stations, grid.transform);
        CreateNextTrackSet();
        _datastore.nextStation = generatedTracks.FindLast(track => track.name.Contains("Station"));

        _datastore.distToNextStation.Where(dist => dist <= -15).Subscribe(_ => {
            _datastore.posOfLastStation = _datastore.nextStation.transform.Find("Stop").position;
            _datastore.nextStation = generatedTracks.FindLast(track => track.name.Contains("Station"));
        });

        currentTrackIndex.Subscribe(newVal => {
            _datastore.currentTrack.Value = generatedTracks[newVal];
            var tracksToDelete = generatedTracks.Take(Math.Max(newVal - trackActivationBuffer, 0)).ToList();
            var tracksToActivate = generatedTracks.Skip(newVal + 1).Take(trackActivationBuffer).ToList();
            var tracksToDeactivate = generatedTracks.Skip(newVal + 1 + trackActivationBuffer).ToList();
        
            tracksToDelete.ForEach(track => {
                generatedTracks.Remove(track);
                Destroy(track);
            });
            tracksToActivate.ForEach(track => {
                track.SetActive(true);
            });
            tracksToDeactivate.ForEach(track => {
                track.SetActive(false);
            });
        });
    }

    public void Update() {
        if (_datastore.train.transform.position.x > endOfTracksetTrigger) {
            CreateNextTrackSet();
        }

        var trainNose = _datastore.train.transform.Find("Nose").position.x;

        currentTrackIndex.Value = generatedTracks.FindIndex(track => {
            var bounds = track.trackBounds();
            return trainNose >= bounds.minXBound && trainNose < bounds.maxXBound;
        });
    }

    public void CreateNextTrackSet() {
        Enumerable.Range(0, numGates).ToList().ForEach(i => {
            generatedTracks.AddRange(GenerateNewTracks(tracksBetweenLandmarks, _prefabs.tracks, grid.transform));
            generatedTracks.AddRange(GenerateNewTracks(1, _prefabs.gates, grid.transform));
            generatedTracks.Last().GetComponent<Gate>().speedValue = Enumerable.Range(0, 4).getRandomElement();
        });
        
        generatedTracks.AddRange(GenerateNewTracks(1, _prefabs.stations, grid.transform));

        var originTrack = generatedTracks.First().transform;
        var nextTrackPositions = CalculateTrackPositions(originTrack.position.x, generatedTracks);
        for (int i = 0; i < generatedTracks.Count; i++) {
            generatedTracks[i].transform.position = nextTrackPositions[i];
        }
        // Get the second-to-last track origin
        endOfTracksetTrigger = generatedTracks.Skip(generatedTracks.Count - 2).First().transform.position.x;
    }

    private static List<GameObject> GenerateNewTracks(int total, List<GameObject> possibleTracks, Transform parent) {
        return Enumerable.Range(0, total)
            .Select(i => Instantiate(possibleTracks.getRandomElement(), parent))
            .ToList();
    }

    private static List<Vector3> CalculateTrackPositions(float initX, List<GameObject> pregeneratedTracks) {
        var newPositions = new List<Vector3>();
        pregeneratedTracks.Aggregate(
            initX,
            (nextX, track) => {
                var trackPos = track.transform.position;
                newPositions.Add(new Vector3 {
                    x = nextX,
                    y = trackPos.y,
                    z = trackPos.z,
                });
                return nextX + track.transform.Find("Track").GetComponent<Tilemap>().size.x;
            });
        return newPositions;
    }
}