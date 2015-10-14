using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Utility
{
    public class FPSCounter : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        private int fpsAccumulator = 0;
        private float fpsNextPeriod = 0;
        private int currentFps;
        const string display = "{0} FPS";
        private Text textCanvas;

        private void Start()
        {
            fpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
            textCanvas = GetComponent<Text>();
        }

        private void Update()
        {
            fpsAccumulator++;

            if (Time.realtimeSinceStartup > fpsNextPeriod)
            {
                currentFps = (int) (fpsAccumulator/fpsMeasurePeriod);
                fpsAccumulator = 0;
                fpsNextPeriod += fpsMeasurePeriod;
                textCanvas.text = string.Format(display, currentFps);
            }

            if(!GameManager.instance.debugMode)
            {
                textCanvas.text = "";
            }
        }
    }
}
