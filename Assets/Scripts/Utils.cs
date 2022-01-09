using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Utils {

    public static System.Random rng = new System.Random();

    public static T getRandomElement<T>(this IEnumerable<T> list) {
        return list.OrderBy(i => rng.Next()).First();
    }

    public static List<T> getManyRandomElements<T>(this IEnumerable<T> list, int number) {
        return list.OrderBy(i => rng.Next()).Take(number).ToList();
    }

    public class TrackBounds {
        public double minXBound;
        public double maxXBound;
    }
    public static TrackBounds trackBounds(this GameObject track) {
        var trackTilemap = track.transform.Find("Track").GetComponent<Tilemap>();
        
        if (trackTilemap == null) 
            return null;
        
        var leftBound = trackTilemap.localBounds.min.x + track.transform.position.x;
        var rightBound = trackTilemap.localBounds.max.x + track.transform.position.x;

        return new TrackBounds {
            minXBound = leftBound,
            maxXBound = rightBound,
        };
    }
}