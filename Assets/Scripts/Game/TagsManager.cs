using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TagsManager : MonoBehaviour
{
    public static TagsManager instance;

    public Dictionary<string, TagInfo> tagsInfo;
    public List<string> tagsName;

    public int attemptsCount;
    public float[] attemptsWeights;

    private string snapshotsPath;

    private const string snapshotsFilePrefix = "TagsSnapshot_";

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

        SetTags(Tag.values.ToArray());

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

    public void SetTags(string[] tags)
    {
        tagsInfo = new Dictionary<string, TagInfo>();
        tagsName = new List<string>(tags.Length);

        foreach (string tag in tags)
        {
            SetTagsInfo(tag, new TagInfo(tag));
        }
    }

    public void SetTagsInfo(string tagName, TagInfo tagInfo)
    {
        tagsInfo[tagName] = tagInfo;

        if(tagsName.Contains(tagName) == false)
        {
            tagsName.Add(tagName);
        }
    }

	// Saves the masteries of each mechanic to a file
    public void SaveSnapshot(int slot)
    {
        string data = "";
        data += "Weights";
        for(int i = 0; i < attemptsWeights.Length; i++)
        {
            data += " " + attemptsWeights[i];
        }

        foreach(string key in tagsInfo.Keys)
        {
            data += "\n\n";
            data += "Tag " + key.Replace(' ', '_');

            data += "\n";
            List<int> attempts = tagsInfo[key].attempts;
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

	// Loads the masteries for all the mechanics from a file
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

        string tagName = null;

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

                    tagName = null;
                }
                else if (parameters[0] == "Tag")
                {
                    tagName = parameters[1].Replace('_', ' ');
                }
            }
            else if(indentationLevel == 1)
            {
                if(tagsInfo.ContainsKey(tagName))
                {
                    for (int p = 0; p < parameters.Length; p++)
                    {
                        tagsInfo[tagName].AddAttempt(int.Parse(parameters[p]) == 1);
                    }
                }
            }
        }

        if(slot != 0)
        {
            Debug.Log("Loaded snapshot " + slot);
        }
    }

    public string PlayerStateToTag(PlayerState state)
    {
        if(state == PlayerState.JUMPING)
        {
            return Tag.Jump;
        }
        else if (state == PlayerState.DOUBLE_JUMPING)
        {
            return Tag.Double_Jump;
        }
        else if (state == PlayerState.SLIDING)
        {
            return Tag.Slide;
        }
        else if (state == PlayerState.DASHING)
        {
            return Tag.Dash;
        }

        return null;
    }

    public List<string> PlayerStateToTags(PlayerState[] playerStates)
    {
        List<string> tags = new List<string>();

        foreach(PlayerState state in playerStates)
        {
            string tag = PlayerStateToTag(state);

            if(tag != null)
            {
                tags.Add(tag);
            }
        }

        return tags;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(TagsManager))]
public class TagsManagerEditor : Editor
{
    TagsManager controller;

    public override void OnInspectorGUI()
    {
        if(controller == null)
        {
            controller = (TagsManager)target;
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