using UnityEngine;
using UnityEngine.UI;

public class TextDebugger : MonoBehaviour
{
    public static TextDebugger instance;

    private Text textCanvas;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        textCanvas = GetComponent<Text>();
    }

    void Update()
    {
        string textToDisplay = "";

        if(GameManager.instance.debugMode)
        {
            textToDisplay += "D | Debug Mode: " + GameManager.instance.debugMode;
            textToDisplay += "\n";
            textToDisplay += "1-9 | Spawn Patterns";
            textToDisplay += "\n";
            textToDisplay += "C | Collisions: " + PlayerController.instance.stopOnCollision;
            textToDisplay += "\n";
            textToDisplay += "H | Hide Blocks in Hierarchy: " + PatternManager.instance.hideBlocksInHierarchy;
            textToDisplay += "\n";
            textToDisplay += "P | Camera Projection";
            textToDisplay += "\n";
            textToDisplay += "N-M | Camera Zoom";
            textToDisplay += "\n";
            textToDisplay += "F12 | Node Editor";
            textToDisplay += "\n";
            textToDisplay += "Space | Pause";
            textToDisplay += "\n";
            textToDisplay += "R | Restart";
        }

        textCanvas.text = textToDisplay;
    }
}
