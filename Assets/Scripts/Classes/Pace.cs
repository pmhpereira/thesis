using System.Collections.Generic;
using UnityEngine;

public class Pace
{
    public const string SLOW = "Slow";
    public const string NORMAL = "Normal";
    public const string FAST = "Fast";

    public static readonly List<string> values = new List<string>(new string[] {
        Pace.SLOW,
        Pace.NORMAL,
        Pace.FAST
    });
}
