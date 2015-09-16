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

        textToDisplay += "(C)ollisions: " + playerController.stopOnCollision.ToString();
        textToDisplay += "\n";
        textToDisplay += "(R)estart";

        textCanvas.text = textToDisplay;
    }
}
