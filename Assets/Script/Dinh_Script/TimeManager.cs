using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    //Singleton pattern
    public static TimeManager Instance { get; private set; }
    //Event đc phát khi 1 ngày mới bắt đầu
    public static event Action OnNewDay;

    public int currentDay { get; private set; } = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); //Giữ nguyên TimeManager khi load scene mới
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Hàm Test (xóa khi có cơ chế ngủ thật)

    public void PassDay()
    {
        currentDay++;
        OnNewDay?.Invoke();
        Debug.Log("A new day has begun! Current Day: " + currentDay);
    }
}
