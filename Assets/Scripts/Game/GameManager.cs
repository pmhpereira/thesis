using UnityEngine;
using UnityEngine.SceneManagement;

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
            CameraController.instance.ToogleSplitscreen();
            TreeManager.instance.ToogleNodeEditor();
        }

        if (Input.GetKeyDown(KeyCode.F1)) SaveSnapshot(1);
        else if (Input.GetKeyDown(KeyCode.F2)) SaveSnapshot(2);
        else if (Input.GetKeyDown(KeyCode.F3)) SaveSnapshot(3);
        else if (Input.GetKeyDown(KeyCode.F4)) SaveSnapshot(4);

        if (Input.GetKeyDown(KeyCode.F5)) LoadSnapshot(1);
        else if (Input.GetKeyDown(KeyCode.F6)) LoadSnapshot(2);
        else if (Input.GetKeyDown(KeyCode.F7)) LoadSnapshot(3);
        else if (Input.GetKeyDown(KeyCode.F8)) LoadSnapshot(4);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }

    public void SaveSnapshot(int slot)
    {
        PatternManager.instance.SaveSnapshot(slot);
        TagsManager.instance.SaveSnapshot(slot);
        PaceManager.instance.SaveSnapshot(slot);
    }

    public void LoadSnapshot(int slot)
    {
        PatternManager.instance.LoadSnapshot(slot);
        TagsManager.instance.LoadSnapshot(slot);
        PaceManager.instance.LoadSnapshot(slot);
    }
}
