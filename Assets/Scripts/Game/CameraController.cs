﻿/*
	This class controls different visibility settings for the game camera
*/
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    private new Camera camera;

    public bool sceneViewFollow;

    public enum Splitscreen
    {
        None,
        Vertical,
    }

    Splitscreen splitscreen;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        camera = GetComponent<Camera>();

        LoadFromDataHolder();
        
        splitscreen = Splitscreen.None;
    }

    void Update()
    {
        ProcessInput();
        UpdateGame();
    }

    void ProcessInput()
    {
        if (Input.GetKey(KeyCode.N))
        {
            if (camera.orthographic)
            {
                camera.orthographicSize += 0.02f;
            }
            else
            {
                camera.fieldOfView += 0.1f;
            }

            SaveToPlayerPrefs();
        }

        if (Input.GetKey(KeyCode.M))
        {
            if (camera.orthographic)
            {
                camera.orthographicSize -= 0.02f;
            }
            else
            {
                camera.fieldOfView -= 0.1f;
            }

            SaveToPlayerPrefs();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            camera.orthographic = !camera.orthographic;

            if (camera.orthographic)
            {
                camera.orthographicSize = Mathf.Tan(Mathf.Deg2Rad * camera.fieldOfView / 2) * Mathf.Abs(camera.transform.position.z);
            }
            else
            {
                camera.fieldOfView = 2 * Mathf.Atan(camera.orthographicSize / Mathf.Abs(camera.transform.position.z)) * Mathf.Rad2Deg;
            }

            SaveToPlayerPrefs();
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            sceneViewFollow = !sceneViewFollow;
        }

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.V))
        {
            SceneView.lastActiveSceneView.orthographic = !SceneView.lastActiveSceneView.orthographic;
        }
        #endif
    }

    public void ToogleSplitscreen()
    {
        switch(splitscreen)
        {
            case Splitscreen.None:
                splitscreen = Splitscreen.Vertical;
                break;
            case Splitscreen.Vertical:
                splitscreen = Splitscreen.None;
                break;
        }

        UpdateSplitscreen();
    }

    void UpdateSplitscreen()
    {
        Rect rect = new Rect();

        if(splitscreen == Splitscreen.None)
        {
            rect.width = 1;
            rect.height = 1;
        }
        else if(splitscreen == Splitscreen.Vertical)
        {
            rect.width = 1;
            rect.height = 0.5f;
        }

        rect.x = 0;
        rect.y = 1 - rect.height;
		camera.rect = rect;
    }

    void UpdateGame()
    {
        camera.transform.position += new Vector3(PlayerController.instance.moveSpeed * Time.deltaTime, 0, 0);

        #if UNITY_EDITOR
        if (sceneViewFollow && SceneView.lastActiveSceneView)
        {
            SceneView.lastActiveSceneView.pivot = camera.transform.position;
        }
        #endif
    }

    void SaveToPlayerPrefs()
    {
        PlayerPrefs.SetInt("cameraOrtographic", camera.orthographic ? 1 : 0);
        PlayerPrefs.SetFloat("cameraFieldOfView", camera.fieldOfView);
        PlayerPrefs.SetFloat("cameraOrthographicSize", camera.orthographicSize);
    }

    void LoadFromDataHolder()
    {
        if(PlayerPrefs.HasKey("cameraOrtographic"))
        {
            camera.orthographic = PlayerPrefs.GetInt("cameraOrtographic") == 1;
            camera.fieldOfView = PlayerPrefs.GetFloat("cameraFieldOfView");
            camera.orthographicSize = PlayerPrefs.GetFloat("cameraOrthographicSize");
        }
    }
}
