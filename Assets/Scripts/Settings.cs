
using UnityEngine;

public static class Settings {

    [Range(10f, 1000f)]
    public static float Sensitivity = 200;

    [Range(30, 120)]
    public static float FOV_Current = 60;

    public static float FOV_Base = 60;

    public static bool Paused = false;
}
