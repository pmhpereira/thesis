using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    [Range (0, 120)]
    public int frameRate;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }

        instance = this;
    }

    void Update()
    {
        ProcessInput();
    }
    
    void ProcessInput()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    void OnValidate()
    {

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = frameRate;
    }


    void RestartLevel()
    {
        Application.LoadLevel(Application.loadedLevel);
        Time.timeScale = 1;
    }
}
