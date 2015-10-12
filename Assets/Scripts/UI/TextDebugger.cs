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

        if(GameManager.instance.debugMode)
        {
            textToDisplay += "D | Debug Mode: " + GameManager.instance.debugMode.ToString();
            textToDisplay += "\n";
            textToDisplay += "1-9 | Spawn Patterns";
            textToDisplay += "\n";
            textToDisplay += "C | Collisions: " + playerController.stopOnCollision.ToString();
            textToDisplay += "\n";
            textToDisplay += "H | Hide Blocks in Hierarchy: " + PatternManager.instance.hideBlocksInHierarchy.ToString();
            textToDisplay += "\n";
            textToDisplay += "P | Camera Projection";
            textToDisplay += "\n";
            textToDisplay += "N-M | Camera Zoom";
            textToDisplay += "\n";
            textToDisplay += "R | Restart";
        }

        textCanvas.text = textToDisplay;
    }
}
