using UnityEngine;

public enum Direction { LT, LD, RT, RD }

public abstract class AppearanceSet : ScriptableObject
{
    // Idle frames
    public Sprite[] LT_idle;
    public Sprite[] LD_idle;
    public Sprite[] RT_idle;
    public Sprite[] RD_idle;

    // Walk frames
    public Sprite[] LT_walk;
    public Sprite[] LD_walk;
    public Sprite[] RT_walk;
    public Sprite[] RD_walk;

    // Digging frames
    public Sprite[] LT_digging;
    public Sprite[] LD_digging;
    public Sprite[] RT_digging;
    public Sprite[] RD_digging;

    // Watering frames
    public Sprite[] LT_watering;
    public Sprite[] LD_watering;
    public Sprite[] RT_watering;
    public Sprite[] RD_watering;

    public Sprite[] LT_harvest;
    public Sprite[] LD_harvest;
    public Sprite[] RT_harvest;
    public Sprite[] RD_harvest;


}
