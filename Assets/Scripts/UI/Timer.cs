﻿using UnityEngine;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour
{
    private Text textCanvas;
    private float timer;

    void Awake()
    {
        textCanvas = GetComponent<Text>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        TimeSpan timeSpan = TimeSpan.FromSeconds(timer);
        string timeText = string.Format("{0:D1}:{1:D2}.{2:D3}", (int) timeSpan.TotalMinutes, timeSpan.Seconds, timeSpan.Milliseconds);

        if(GameManager.instance.isPaused)
        {
            timeText += "\n";
            timeText += "Paused";
        }

        textCanvas.text = timeText;
    }
}
