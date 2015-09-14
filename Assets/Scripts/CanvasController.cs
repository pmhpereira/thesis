using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CanvasController : MonoBehaviour {

    public static CanvasController instance;

    public Text textCanvas;

    private PlayerController playerController;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
	}

    void Update()
    {
        string textToDisplay = "";

        textToDisplay += "stopOnCollision (C): " + playerController.stopOnCollision.ToString();
        textToDisplay += "\n";
        textToDisplay += "restart (R)";

        textCanvas.text = textToDisplay;
    }
}
