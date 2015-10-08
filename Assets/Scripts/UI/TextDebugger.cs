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

        textToDisplay += "C | Collisions: " + playerController.stopOnCollision.ToString();
        textToDisplay += "\n";
        textToDisplay += "R | Restart";
        textToDisplay += "\n";
        textToDisplay += "P | Camera Projection";
        textToDisplay += "\n";
        textToDisplay += "N-M | Camera Zoom";
        textToDisplay += "\n";
        textToDisplay += "1-9 | Spawn Patterns";

        textCanvas.text = textToDisplay;
    }
}
