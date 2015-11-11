using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;
using System.IO;
using System.Text;

public class PatternGenerator : MonoBehaviour
{
    public static PatternGenerator instance;

    private string[] patternsTextArray;

    private Dictionary<string, GameObject> blocksMap;
    private Dictionary<string, GameObject> patternsMap;
    private Dictionary<string, PatternInfo> patternsInfo;

    private GameObject generated;

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
        if (generated != null)
        {
            Destroy(generated);
        }

        LoadPatternsFromFile();
        LoadPatternsInfoFromFile();
        UpdatePatternManager();
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

        generated = new GameObject("Generated");
        generated.transform.SetParent(this.transform);
        generated.transform.position = new Vector3(0, -1000, 0);
        //generated.SetActive(false);

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
                pattern.transform.SetParent(generated.transform);
                pattern.transform.tag = "Pattern";
                pattern.transform.localPosition = new Vector3(0, height, 0);
                patternsMap[pattern.name] = pattern;
                patternsInfo[pattern.name] = new PatternInfo(pattern.name);

                height += 5f;
            }
            else if (indentationLevel == 1)
            {
                block = Instantiate(blocksMap[parameters[0]]);
                block.name = parameters[0];
                block.transform.SetParent(pattern.transform);
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

    void LoadPatternsInfoFromFile()
    {
        string patternsInfoText = Resources.Load<TextAsset>("Patterns_Info").text;
        patternsInfoText = patternsInfoText.Replace("\r", "");

        string[] lines = patternsInfoText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int lastCommentIndendation = -1;
        string lastPatternName = null;

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
                lastPatternName = parameters[0];
            }
            else if (indentationLevel == 1)
            {
                if (parameters[0] == "Tags")
                {
                    string[] tags = new string[parameters.Length - 1];
                    Array.Copy(parameters, 1, tags, 0, tags.Length);
                    patternsInfo[lastPatternName].AddTags(tags);
                }
                else if (parameters[0] == "Dependencies")
                {
                    Dependency[] dependencies = new Dependency[parameters.Length - 1];

                    for (int d = 1; d < parameters.Length; d++)
                    {
                        string[] dependencyParams = parameters[d].Split('|');
                        string patternName = dependencyParams[0];
                        int tagIndex = int.Parse(dependencyParams[1]);
                        Mastery mastery = Mastery.FromId(dependencyParams[2]);

                        dependencies[d - 1] = new Dependency(patternName, tagIndex, mastery);
                    }

                    patternsInfo[lastPatternName].AddDependencies(dependencies);
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            CameraController.instance.sceneViewFollow = false;
            SceneView.lastActiveSceneView.pivot = generated.transform.position;
        }
    }

    public void SaveAll()
    {
        if (generated == null)
        {
            return;
        }

        string data = "";

        foreach (Transform pattern in generated.transform)
        {
            data += pattern.name;
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

[CustomEditor(typeof(PatternGenerator))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("Save to File") || Input.GetKeyDown(KeyCode.S))
        {
            PatternGenerator.instance.SaveAll();
        }
        else if (GUILayout.Button("Load from File") || Input.GetKeyDown(KeyCode.L))
        {
            PatternGenerator.instance.LoadAll();
        }

        GUI.enabled = true;
    }
}
