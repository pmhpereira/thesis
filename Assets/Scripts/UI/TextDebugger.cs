using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextDebugger : MonoBehaviour {

    public static TextDebugger instance;

    private Text textCanvas;

    private PlayerController playerController;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        textCanvas = GetComponent<Text>();
}

void Update()
    {
        string textToDisplay = "";

        textToDisplay += "1 | Collisions: " + playerController.stopOnCollision.ToString();
        textToDisplay += "\n";
        textToDisplay += "2 | Holes: " + ObstacleController.instance.spawnHoles;
        textToDisplay += "\n";
        textToDisplay += "3 | Obstacles: " + ObstacleController.instance.spawnObstacles;
        textToDisplay += "\n";
        textToDisplay += "R | Restart";
        textToDisplay += "\n";
        textToDisplay += "C | Camera Projection";
        textToDisplay += "\n";
        textToDisplay += "N-M | Camera Zoom";

        textCanvas.text = textToDisplay;
    }
}
