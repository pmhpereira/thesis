using UnityEngine;
using System.Collections.Generic;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Text;

public class PatternGenerator : MonoBehaviour
{
    public static PatternGenerator instance;

    private string[] patternsTextArray;

    private Dictionary<string, GameObject> blocksMap;
    private Dictionary<string, GameObject> patternsMap;
    private Dictionary<string, PatternInfo> patternsInfo;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;
        LoadAll();
    }

    public void LoadAll()
    {
        ResetAll();
        LoadPatternsFromFile();
        UpdatePatternManager();
    }

    void ResetAll()
    {
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }

        PatternManager.instance.SetPatterns(new GameObject[0]);
    }

    void LoadPatternsFromFile()
    {
        GameObject[] blockPrefabs = Resources.LoadAll<GameObject>("Blocks");
        blocksMap = new Dictionary<string, GameObject>();
        patternsMap = new Dictionary<string, GameObject>();
        patternsInfo = new Dictionary<string, PatternInfo>();

        foreach (GameObject prefab in blockPrefabs)
        {
            blocksMap[prefab.name] = prefab;
        }

        string patternsText = Resources.Load<TextAsset>("Patterns").text;
        patternsText = patternsText.Replace("\r", "");

        string[] lines = patternsText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        GameObject pattern = null, block = null;

        int lastCommentIndendation = -1;
        float height = 0;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            int indentationLevel = line.Split(new string[] { "\t", "    " }, StringSplitOptions.None).Length - 1;
            string[] parameters = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parameters[0].StartsWith("//"))
            {
                lastCommentIndendation = indentationLevel;
                continue;
            }

            if (lastCommentIndendation >= 0 && indentationLevel > lastCommentIndendation)
            {
                continue;
            }
            else
            {
                lastCommentIndendation = -1;
            }

            if (indentationLevel == 0)
            {
                pattern = new GameObject(parameters[0]);
                pattern.AddComponent<PatternController>();
                pattern.transform.SetParent(this.transform);
                pattern.transform.tag = "Pattern";
                pattern.transform.localPosition = new Vector3(0, height, 0);
                patternsMap[pattern.name] = pattern;
                patternsInfo[pattern.name] = new PatternInfo(pattern.name);

                height -= 10f;
            }
            else if (indentationLevel == 1)
            {
                if(parameters[0] == "Spacing")
                {
                    pattern.GetComponent<PatternController>().preSpacing = int.Parse(parameters[1]);
                    pattern.GetComponent<PatternController>().postSpacing = int.Parse(parameters[2]);
                }
                else
                {
                    block = Instantiate(blocksMap[parameters[0]]);
                    block.name = parameters[0];
                    block.transform.SetParent(pattern.transform);
                }
            }
            else if (indentationLevel == 2)
            {
                if (parameters[0] == "Position")
                {
                    float x = float.Parse(parameters[1]);
                    float y = float.Parse(parameters[2]);
                    float z = 0;

                    block.transform.localPosition = new Vector3(x, y, z);
                }
            }
        }
    }

    void UpdatePatternManager()
    {
        PatternManager.instance.SetPatterns(new List<GameObject>(patternsMap.Values).ToArray());

        foreach (GameObject pattern in patternsMap.Values)
        {
            PatternManager.instance.SetPatternsInfo(pattern.name, patternsInfo[pattern.name]);
        }
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E) && SceneView.lastActiveSceneView)
        {
            CameraController.instance.sceneViewFollow = false;
            SceneView.lastActiveSceneView.pivot = this.transform.position;
        }
        #endif
    }

    public void SaveAll()
    {
        SavePatternsToFile();
        AssetDatabase.Refresh();
    }

    private void SavePatternsToFile()
    {
        string data = "";

        foreach (Transform pattern in this.transform)
        {
            data += pattern.name;
            data += "\n";

            data += "    ";
            data += "Spacing ";
            data += pattern.GetComponent<PatternController>().preSpacing + " " + pattern.GetComponent<PatternController>().postSpacing;
            data += "\n";
            foreach (Transform block in pattern)
            {
                data += "    ";
                data += block.name.Split(new string[] { " (" }, StringSplitOptions.RemoveEmptyEntries)[0];
                data += "\n";

                data += "    ";
                data += "    ";
                data += "Position " + block.localPosition.x + " " + block.localPosition.y;
                data += "\n";
            }

            data += "\n";
        }

        string filePath = Application.dataPath + "/Resources/Patterns.txt";
        FileStream file = File.Open(filePath, FileMode.Create);
        foreach (byte b in Encoding.ASCII.GetBytes(data))
        {
            file.WriteByte(b);
        }
        file.Close();

        Debug.Log("Saved patterns to file: " + filePath);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PatternGenerator))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Save to File"))
        {
            PatternGenerator.instance.SaveAll();
        }
        else if (GUILayout.Button("Load from File"))
        {
            PatternGenerator.instance.LoadAll();
        }

        GUI.enabled = true;
    }
}
#endif