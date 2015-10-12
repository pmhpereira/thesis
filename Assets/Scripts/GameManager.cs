using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [Range (30, 120)]
    public int frameRate;

    public bool debugMode;

    void Awake() {
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
    }

    void OnValidate()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;
    }

    void RestartLevel()
    {
        Time.timeScale = 1;
        Application.LoadLevel(Application.loadedLevel);
        PlayerPrefs.Save();
    }
}
