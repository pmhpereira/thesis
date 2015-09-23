using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public GameObject dataHolder;

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
        Time.timeScale = 1;
        Application.LoadLevel(Application.loadedLevel);
        PlayerPrefs.Save();
    }
}
