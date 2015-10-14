using UnityEngine;
using System.Collections.Generic;
using System;

public class PatternGenerator : MonoBehaviour
{
    public static PatternGenerator instance;

    private string[] patternsTextArray;

    private Dictionary<string, GameObject> blocksMap;
    private Dictionary<string, GameObject> patternsMap;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        GameObject[]  blockPrefabs = Resources.LoadAll<GameObject>("Blocks");
        blocksMap = new Dictionary<string, GameObject>();
        patternsMap = new Dictionary<string, GameObject>();

        foreach (GameObject prefab in blockPrefabs)
        {
            blocksMap[prefab.name] = prefab;
        }

        string patternsText = Resources.Load<TextAsset>("Patterns").text;
        patternsText = patternsText.Replace("\r", "");

        string[] lines = patternsText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        GameObject pattern = null, block = null;

        GameObject generated = new GameObject("Generated");
        generated.transform.SetParent(this.transform);
        generated.SetActive(false);

        for(int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            int indentation = line.Split('\t').Length - 1;
            string[]  parameters = line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (indentation == 0)
            {
                pattern = new GameObject(parameters[0]);
                pattern.AddComponent<PatternController>();
                pattern.transform.SetParent(generated.transform);
                pattern.transform.tag = "Pattern";

                patternsMap[pattern.name] = pattern;
            }
            else if(indentation == 1)
            {
                block = Instantiate(blocksMap[parameters[0]]);
                block.name = parameters[0];
                block.transform.SetParent(pattern.transform);
            }
            else if(indentation == 2)
            {
                if(parameters[0].Equals("Position"))
                {
                    float x = float.Parse(parameters[1]);
                    float y = float.Parse(parameters[2]);
                    float z = 0;

                    block.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }

    public GameObject[] GetGeneratedPatterns()
    {
        return new List<GameObject>(patternsMap.Values).ToArray();
    }
}
