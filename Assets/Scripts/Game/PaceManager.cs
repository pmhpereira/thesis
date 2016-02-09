using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PaceManager : MonoBehaviour
{
    public static PaceManager instance;

    public Dictionary<string, PaceInfo> pacesInfo;

    public int attemptsCount;
    public float[] attemptsWeights;

    private string snapshotsPath;

    private const string snapshotsFilePrefix = "PaceSnapshot_";

    [HideInInspector]
    public int linearRepetition;
    [HideInInspector]
    public float quadraticStart;
    [HideInInspector]
    public float logarithmicBase;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        SetPaces(new PaceNode[0]);

        attemptsCount = attemptsWeights.Length;

        if(Application.isEditor)
        {
            snapshotsPath = Application.dataPath + "/Resources";
        }
        else
        {
            snapshotsPath = Application.persistentDataPath;
        }

        snapshotsPath += "/Snapshots";
    }

    public void SetPaces(PaceNode[] paces)
    {
        pacesInfo = new Dictionary<string, PaceInfo>();

        foreach (PaceNode node in paces)
        {
            SetPacesInfo(node.paceName, new PaceInfo(node.paceName, node.instancesCount));
        }
    }

    public void SetPacesInfo(string paceName, PaceInfo paceInfo)
    {
        pacesInfo[paceName] = paceInfo;
    }

    void Update()
    {
        ProcessInput();
    }

    void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.F1)) SaveSnapshot(1);
        else if (Input.GetKeyDown(KeyCode.F2)) SaveSnapshot(2);
        else if (Input.GetKeyDown(KeyCode.F3)) SaveSnapshot(3);
        else if (Input.GetKeyDown(KeyCode.F4)) SaveSnapshot(4);

        if (Input.GetKeyDown(KeyCode.F5)) LoadSnapshot(1);
        else if (Input.GetKeyDown(KeyCode.F6)) LoadSnapshot(2);
        else if (Input.GetKeyDown(KeyCode.F7)) LoadSnapshot(3);
        else if (Input.GetKeyDown(KeyCode.F8)) LoadSnapshot(4);
    }

    public void SaveSnapshot(int slot)
    {
        string data = "";
        data += "Weights";
        for(int i = 0; i < attemptsWeights.Length; i++)
        {
            data += " " + attemptsWeights[i];
        }

        foreach(string key in pacesInfo.Keys)
        {
            data += "\n\n";
            data += "Pace " + key;
            
            data += "\n";
            List<int> attempts = pacesInfo[key].attempts;

            if(attempts.Count > 0)
            {
                data += "   ";
            }
            foreach(int a in attempts)
            {
                data += " " + a;
            }
        }

        data += "\n";

        string filePath = snapshotsPath + "/" + snapshotsFilePrefix + slot + ".txt";
        FileStream file = File.Open(filePath, FileMode.Create);
        foreach (byte b in Encoding.ASCII.GetBytes(data))
        {
            file.WriteByte(b);
        }
        file.Close();

        if(slot != 0)
        {
            Debug.Log("Saved snapshot " + slot);
        }
    }

    public void LoadSnapshot(int slot)
    {
        string filePath = snapshotsPath + "/" + snapshotsFilePrefix + slot + ".txt";
        if(!File.Exists(filePath))
        {
            return;
        }

        FileStream file = File.OpenRead(filePath);
        byte[] bytes = new byte[file.Length];
        file.Read(bytes, 0, bytes.Length);
        file.Close();

        string data = Encoding.ASCII.GetString(bytes, 0, bytes.Length);

        string[] lines = data.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        string paceName = null;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            int indentationLevel = line.Split(new string[] { "\t", "    " }, StringSplitOptions.None).Length - 1;
            string[] parameters = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parameters[0].StartsWith("//"))
            {
                continue;
            }

            if (indentationLevel == 0)
            {
                if (parameters[0] == "Weights")
                {
                    attemptsWeights = new float[parameters.Length - 1];

                    for (int p = 1; p < parameters.Length; p++)
                    {
                        attemptsWeights[p - 1] = float.Parse(parameters[p]);
                    }

                    paceName = null;
                }
                else if (parameters[0] == "Pace")
                {
                    if(parameters.Length > 1)
                    {
                        paceName = parameters[1];
                    }
                }
            }
            else if(indentationLevel == 1)
            {
                if(pacesInfo.ContainsKey(paceName))
                {
                    for (int p = 1; p < parameters.Length; p++)
                    {
                        pacesInfo[paceName].AddAttempt(int.Parse(parameters[p]) == 1);
                    }
                }
            }
        }

        if(slot != 0)
        {
            Debug.Log("Loaded snapshot " + slot);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PaceManager))]
public class PaceManagerEditor : Editor
{
    PaceManager controller;

    public override void OnInspectorGUI()
    {
        if(controller == null)
        {
            controller = (PaceManager)target;
        }

        DrawDefaultInspector();
        
        EditorGUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            if(GUILayout.Button(" % ", GUILayout.ExpandWidth(false)))
            {
                controller.attemptsWeights = WeightGenerator.Percentage(controller.attemptsCount);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            controller.linearRepetition = EditorGUILayout.IntField("Repetition", controller.linearRepetition);
            controller.linearRepetition = Math.Max(1, controller.linearRepetition);
            if(GUILayout.Button("Lin", GUILayout.ExpandWidth(false)))
            {
                controller.attemptsWeights = WeightGenerator.Linear(controller.attemptsCount, controller.linearRepetition);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            controller.quadraticStart = EditorGUILayout.FloatField("Start", controller.quadraticStart);
            controller.quadraticStart = Math.Max(0.1f, controller.quadraticStart);
            if(GUILayout.Button("Qua", GUILayout.ExpandWidth(false)))
            {
                controller.attemptsWeights = WeightGenerator.Quadratic(controller.attemptsCount, controller.quadraticStart);
            }
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        {
            controller.logarithmicBase = EditorGUILayout.FloatField("Base", controller.logarithmicBase);
            controller.logarithmicBase = Math.Max(2, controller.logarithmicBase);
            if(GUILayout.Button("Log", GUILayout.ExpandWidth(false)))
            {
                controller.attemptsWeights = WeightGenerator.Logarithmic(controller.attemptsCount, controller.logarithmicBase);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif