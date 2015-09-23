﻿using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public static CameraController instance;

    private new Camera camera;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        camera = GetComponent<Camera>();

        LoadFromDataHolder();
    }

    void Update()
    {
        ProcessInput();
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

        if (Input.GetKeyDown(KeyCode.C))
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