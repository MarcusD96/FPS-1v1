using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Settings {

    [Range(10f, 1000f)]
    public static float Sensitivity = 300;

    [Range(30, 120)]
    public static float FOV = 60;

    public static bool Paused = false;
}
