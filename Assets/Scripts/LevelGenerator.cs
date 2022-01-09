using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelGenerator : MonoBehaviour {
    private Prefabs _prefabs;
    private Datastore _datastore;

    public Grid grid;

    public int trackBufferSize = 3;
    public int numGates = 1;
    public List<GameObject> generatedTracks;
    public double trainRerenderBoundary = 0;

    public void Start() {
        _prefabs = GetComponent<Prefabs>();
        _datastore = GetComponent<Datastore>();

        generatedTracks = GenerateNewTracks(1, _prefabs.stations, grid.transform);
        CreateNextTrackSet();
        _datastore.nextStation = generatedTracks.FindLast(track => track.name.Contains("Station"));

        _datastore.distToNextStation.Where(dist => dist <= -15).Subscribe(_ => {
            _datastore.nextStation = generatedTracks.FindLast(track => track.name.Contains("Station"));
        });
    }

    public void Update() {
        if (_datastore.train.transform.position.x > trainRerenderBoundary) {
            CleanUpTracks();
            CreateNextTrackSet();
        }
    }

    public void CreateNextTrackSet() {
        Enumerable.Range(0, numGates).ToList().ForEach(i => {
            generatedTracks.AddRange(GenerateNewTracks(trackBufferSize, _prefabs.tracks, grid.transform));
            generatedTracks.AddRange(GenerateNewTracks(1, _prefabs.gates, grid.transform));
        });
        
        generatedTracks.AddRange(GenerateNewTracks(1, _prefabs.stations, grid.transform));

        var originTrack = generatedTracks.First().transform;
        var nextTrackPositions = CalculateTrackPositions(originTrack.position.x, generatedTracks);
        for (int i = 0; i < generatedTracks.Count; i++) {
            generatedTracks[i].transform.position = nextTrackPositions[i];
        }
        // Get the second-to-last track origin
        trainRerenderBoundary = generatedTracks.Skip(generatedTracks.Count - 2).First().transform.position.x;
    }

    public void CleanUpTracks() {
        // we want to only delete the tracks that are out of view. The train will be on the boundary between the second-to-last
        // and last tracks at the moment we recycle. So we want to avoid deleting the second-to-last right in front of the player.
        // TODO could delete and add as the camera moves
        var tracksToDelete = generatedTracks.Take(generatedTracks.Count - 2).ToList();
        tracksToDelete.ForEach(Destroy);
        generatedTracks = generatedTracks.Skip(tracksToDelete.Count).ToList();
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