﻿using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Range (30, 120)]
    public int frameRate;

    public bool debugMode, isPaused;

    private float oldTimeScale;

    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }

        instance = this;

        OnValidate();
    }

    void Update()
    {
        ProcessInput();
    }
    
    void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            debugMode = !debugMode;
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            SetPause(!isPaused);
        }

        if(Input.GetKeyDown(KeyCode.F12))
        {
            TreeManager.instance.ToogleNodeEditor();
        }
    }

    public void SetPause(bool paused)
    {
        isPaused = paused;

        if (isPaused)
        {
            oldTimeScale = Time.timeScale;
        }

        Time.timeScale = isPaused ? 0 : oldTimeScale;
    }

    void OnValidate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;

        SetPause(isPaused);
    }

    void RestartLevel()
    {
        Time.timeScale = 1;
        Application.LoadLevel(Application.loadedLevel);
        PlayerPrefs.Save();
    }
}
