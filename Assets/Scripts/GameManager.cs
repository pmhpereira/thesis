using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }

        instance = this;

        Application.targetFrameRate = 60;
    }

    void Update()
    {
        ProcessInput();
    }
    
    void ProcessInput()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }
    }

}
