using System;
using System.Collections.Generic;
using System.Linq;

public static class Utils {

    public static System.Random rng = new System.Random();

    public static T getRandomElement<T>(this IEnumerable<T> list) {
        return list.OrderBy(i => rng.Next()).First();
    }

    public static List<T> getManyRandomElements<T>(this IEnumerable<T> list, int number) {
        return list.OrderBy(i => rng.Next()).Take(number).ToList();
    }
}