using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    void Awake() {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }

        instance = this;
    }
}
