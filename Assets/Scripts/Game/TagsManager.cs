using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

public class TagsManager : MonoBehaviour
{
    public static TagsManager instance;

    public Dictionary<string, TagInfo> tagsInfo;
    public List<string> tagsName;

    [HideInInspector]
    public int savedAttempts;
    public float[] attemptsWeights;

    private string snapshotsPath;

    private const string snapshotsFilePrefix = "TagsSnapshot_";

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        SetTags(Tag.values.ToArray());

        savedAttempts = attemptsWeights.Length;

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
            SetPatternsInfo(tag, new TagInfo(tag));
        }
    }

    public void SetPatternsInfo(string tagName, TagInfo tagInfo)
    {
        tagsInfo[tagName] = tagInfo;

        if(tagsName.Contains(tagName) == false)
        {
            tagsName.Add(tagName);
        }
    }

    void Update()
    {
        ProcessInput();
    }

    void ProcessInput()
    {
        if(GameManager.instance.isPaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F1)) SaveSnapshot(1);
        else if (Input.GetKeyDown(KeyCode.F2)) SaveSnapshot(2);
        else if (Input.GetKeyDown(KeyCode.F3)) SaveSnapshot(3);
        else if (Input.GetKeyDown(KeyCode.F4)) SaveSnapshot(4);

        if (Input.GetKeyDown(KeyCode.F5)) LoadSnapshot(1);
        else if (Input.GetKeyDown(KeyCode.F6)) LoadSnapshot(2);
        else if (Input.GetKeyDown(KeyCode.F7)) LoadSnapshot(3);
        else if (Input.GetKeyDown(KeyCode.F8)) LoadSnapshot(4);
    }

    void SaveSnapshot(int slot)
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
            data += "Tag " + key;

            List<int> attempts = tagsInfo[key].attempts;

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

        Debug.Log("Saved snapshot " + slot);
    }

    void LoadSnapshot(int slot)
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
                    tagName = parameters[1];
                }
            }
            else if(indentationLevel == 1)
            {
                if(tagsInfo.ContainsKey(tagName))
                {
                    for (int p = 1; p < parameters.Length; p++)
                    {
                        tagsInfo[tagName].AddAttempt(int.Parse(parameters[p]) == 1);
                    }
                }
            }
        }

        Debug.Log("Loaded snapshot " + slot);
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
