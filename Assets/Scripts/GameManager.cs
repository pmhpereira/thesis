using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public GameObject playerPrefab;


    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }

        instance = this;

        Application.targetFrameRate = 60;
    }

    void Start()
    {
        Instantiate(playerPrefab);
    }
}
