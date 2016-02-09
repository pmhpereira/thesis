using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PatternManager : MonoBehaviour
{
    public static PatternManager instance;

    public GameObject groundPrefab;
    public float groundHeight;

    public GameObject ceilingPrefab;
    public float ceilingHeight;

    public float minStartingGroundDistance;
    public float maxStartingGroundDistance;
    private float groundDistance;
    private float ceilingDistance;

    public bool hideBlocksInHierarchy;

    private BoxCollider2D boxCollider;

    private GameObject[] patterns;
    public Dictionary<string, PatternInfo> patternsInfo;
    public List<string> patternsName;

    public int attemptsCount;
    public float[] attemptsWeights;

    private string snapshotsPath;

    private const string snapshotsFilePrefix = "Snapshot_";

    [HideInInspector]
    public int linearRepetition;
    [HideInInspector]
    public float quadraticStart;
    [HideInInspector]
    public float logarithmicBase;

    private int lastPostSpacing;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }

        instance = this;

        groundDistance = minStartingGroundDistance;
        ceilingDistance = groundDistance;

        groundHeight = groundPrefab.transform.position.y;
        ceilingHeight = ceilingPrefab.transform.position.y;

        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.offset = new Vector2(minStartingGroundDistance - 3, 0);

        SetPatterns(new GameObject[] { });

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

        SetupGenerator();
    }

    void SetupGenerator()
    {
        GameObject generated = new GameObject("Generator");
        generated.AddComponent<PatternGenerator>();
        generated.transform.SetParent(this.transform);
        generated.transform.position = new Vector3(0, -1000, 0);
    }

    public void SetPatterns(GameObject[] newPatterns)
    {
        patterns = newPatterns;
        patternsInfo = new Dictionary<string, PatternInfo>();
        patternsName = new List<string>(newPatterns.Length);

        foreach (GameObject pattern in patterns)
        {
            SetPatternsInfo(pattern.name, new PatternInfo(pattern.name));
        }
    }

    public void SetPatternsInfo(string patternName, PatternInfo patternInfo)
    {
        patternsInfo[patternName] = patternInfo;

        if(patternsName.Contains(patternName) == false)
        {
            patternsName.Add(patternName);
        }
    }

    void Start()
    {
        for (float i = groundDistance; i < maxStartingGroundDistance; i += groundPrefab.transform.localScale.x)
        {
            SpawnGround();
            SpawnCeiling();
        }

        StartCoroutine(CheckPatterns());
    }

    IEnumerator CheckPatterns()
    {
        GameObject firstGround = GetFirstObstacleWithTag("Ground");
        GameObject lastPattern = GetLastObstacleWithTag("Pattern");
        GameObject lastObstacle = GetLastObstacleWithTag("Obstacle", lastPattern);
        
        if(firstGround != null && lastPattern != null && lastObstacle != null)
        {
            if(lastObstacle.transform.position.x < firstGround.transform.position.x + (maxStartingGroundDistance - minStartingGroundDistance))
            {
                SpawnPace();
            }
        }
        else
        {
            SpawnPace();
        }

        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(CheckPatterns());
    }

    void Update()
    {
        boxCollider.offset += new Vector2(PlayerController.instance.moveSpeed * Time.deltaTime, 0);

        ProcessInput();
    }

    void SpawnPattern()
    {
        int index = UnityEngine.Random.Range(0, patterns.Length);
        SpawnPattern(index);
    }

    void SpawnPattern(int patternIndex, int paceIndex = -1)
    {
        if(patternIndex < 0 || patternIndex >= patterns.Length)
        {
            return;
        }

        GameObject pattern = patterns[patternIndex];

        if(!CanSpawn(pattern.name))
        {
            Debug.Log("Unresolved dependencies for pattern: " + pattern.name);
            return;
        }

        GameObject newPattern = Instantiate(pattern) as GameObject;
        newPattern.name = pattern.name;

        PatternController patternController = newPattern.GetComponent<PatternController>();
        float patternLength = patternController.length;

        newPattern.transform.localPosition = Vector3.zero;
        newPattern.transform.position += Vector3.right * groundDistance;
        newPattern.transform.SetParent(this.transform);
        newPattern.hideFlags = hideBlocksInHierarchy ? HideFlags.HideInHierarchy : HideFlags.None;

        patternController.SetupDebugMode();
        patternController.SetupBorderColliders();
        patternController.SetPaceIndex(paceIndex);

        groundDistance += patternLength;
        ceilingDistance += patternLength;
    }
    
    void SpawnPace()
    {
        int[] paceArguments = GenerateArguments(TreeManager.instance.GetRandomPaceSpawnerNode(), TreeManager.instance.GetRandomPatternSpawnerNode());

        if(paceArguments == null)
        {
            return;
        }

        int paceIndex = paceArguments[0];
        for(int i = 1; i < paceArguments.Length; i++)
        {
            if(i % 2 == 0)
            {
                SpawnPattern(paceArguments[i], paceIndex);
            }
            else
            {
                for(int g = 0; g < paceArguments[i]; g++)
                {
                    SpawnGround();
                    SpawnCeiling();
                }
            }
        }
    }

    void SpawnGround()
    {
        GameObject ground = (GameObject) Instantiate(groundPrefab);
        ground.transform.position = new Vector2(groundDistance, groundHeight);

        if (hideBlocksInHierarchy)
        {
            ground.hideFlags = HideFlags.HideInHierarchy;
        }

        ground.transform.SetParent(this.transform);
        ground.transform.SetAsLastSibling();
        groundDistance += groundPrefab.transform.localScale.x;
    }

    void SpawnCeiling()
    {
        GameObject ceiling = (GameObject) Instantiate(ceilingPrefab);
        ceiling.transform.position = new Vector2(ceilingDistance, ceilingHeight);

        if (hideBlocksInHierarchy)
        {
            ceiling.hideFlags = HideFlags.HideInHierarchy;
        }

        ceiling.transform.SetParent(this.transform);
        ceiling.transform.SetAsLastSibling();
        ceilingDistance += ceilingPrefab.transform.localScale.x;
    }

    public int[] GenerateArguments(PaceSpawnerNode paceSpawner, PatternSpawnerNode patternSpawner)
    {
        if(paceSpawner == null || patternSpawner == null)
        {
            return null;
        }

        PaceNode paceNode = TreeManager.instance.GetRandomPaceNode(paceSpawner);
        
        List<int> args = new List<int>();
        args.Add(TreeManager.instance.paceNodes.IndexOf(paceNode));

        int previousPostSpacing = 0;
        for(int i = 0; i < paceNode.instancesCount; i++)
        {
            int interval = 0;
            switch(Pace.values[paceNode.paceIndex])
            {
                case Pace.SLOW:
                    interval = UnityEngine.Random.Range(13, 20);
                    break;
                case Pace.NORMAL:
                    interval = UnityEngine.Random.Range(6, 13);
                    break;
                case Pace.FAST:
                    interval = UnityEngine.Random.Range(0, 6);
                    break;
                default:
                    break;
            }

            if(i == 0)
            {
                interval = Mathf.Max(interval, lastPostSpacing);
            }
            
            PatternNode patternNode = TreeManager.instance.GetRandomPatternNode(patternSpawner);
            int preSpacing = patterns[patternNode.patternIndex].GetComponent<PatternController>().preSpacing;

            interval = Mathf.Max(interval, Mathf.Max(preSpacing, previousPostSpacing));
            args.Add(interval);

            args.Add(patternNode.patternIndex);

            previousPostSpacing = patterns[patternNode.patternIndex].GetComponent<PatternController>().postSpacing;
        }

        lastPostSpacing = previousPostSpacing;

        return args.ToArray();
    }

    private GameObject GetFirstObstacleWithTag(string tag, GameObject parent = null)
    {
        int index;
        return GetFirstObstacleWithTag(tag, out index, parent == null ? this.gameObject : parent);
    }

    private GameObject GetFirstObstacleWithTag(string tag, out int index, GameObject parent)
    {
        index = -1;

        foreach(Transform obstacle in parent.transform)
        {
            index++;

            if (obstacle.tag == tag)
            {
                return obstacle.gameObject;
            }
        }

        return null;
    }

    private GameObject GetLastObstacleWithTag(string tag, GameObject parent = null)
    {
        int index;
        return GetLastObstacleWithTag(tag, out index, parent == null ? this.gameObject : parent);
    }

    private GameObject GetLastObstacleWithTag(string tag, out int index, GameObject parent)
    {
        index = -1;

        for (int i = parent.transform.childCount - 1; i >= 0; i--)
        {
            Transform obstacle = parent.transform.GetChild(i);
            index = i;

            if (obstacle.tag == tag)
            {
                return obstacle.gameObject;
            }
        }

        return null;
    }

    public void RepositionGround()
    {
        GameObject firstGround = GetFirstObstacleWithTag("Ground");
        GameObject lastGround = GetLastObstacleWithTag("Ground");

        if (firstGround != null)
        {
            float minimumGround = maxStartingGroundDistance - minStartingGroundDistance;

            for(float i = lastGround.transform.position.x - firstGround.transform.position.x; i < minimumGround; i++)
            {
                SpawnGround();
            }

            if (lastGround.transform.position.x - firstGround.transform.position.x <= minimumGround)
            {
                Vector2 newPosition = new Vector3(groundDistance, groundHeight, 0);

                firstGround.transform.position = newPosition;
                firstGround.transform.SetAsLastSibling();

                groundDistance += groundPrefab.transform.localScale.x;
            }
            else
            {
                Destroy(firstGround);
            }
        }
    }

    public void RepositionCeiling()
    {
        GameObject firstCeiling = GetFirstObstacleWithTag("Ceiling");
        GameObject lastCeiling = GetLastObstacleWithTag("Ceiling");

        if (firstCeiling != null)
        {
            float minimumGround = maxStartingGroundDistance - minStartingGroundDistance;

            for(float i = lastCeiling.transform.position.x - firstCeiling.transform.position.x; i < minimumGround; i++)
            {
                SpawnCeiling();
            }

            if (lastCeiling.transform.position.x - firstCeiling.transform.position.x <= minimumGround)
            {
                Vector2 newPosition = new Vector3(ceilingDistance, ceilingHeight, 0);

                firstCeiling.transform.position = newPosition;
                firstCeiling.transform.SetAsLastSibling();

                ceilingDistance += ceilingPrefab.transform.localScale.x;
            }
            else
            {
                Destroy(firstCeiling);
            }
        }
    }

    void UpdateBlocksVisibilityInHierarchy()
    {
        foreach (Transform pattern in transform)
        {
            pattern.hideFlags = hideBlocksInHierarchy ? HideFlags.HideInHierarchy : HideFlags.None;
        }
    }

    void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            hideBlocksInHierarchy = !hideBlocksInHierarchy;
            UpdateBlocksVisibilityInHierarchy();
        }

        if (Input.GetKeyDown(KeyCode.F1)) SaveSnapshot(1);
        else if (Input.GetKeyDown(KeyCode.F2)) SaveSnapshot(2);
        else if (Input.GetKeyDown(KeyCode.F3)) SaveSnapshot(3);
        else if (Input.GetKeyDown(KeyCode.F4)) SaveSnapshot(4);

        if (Input.GetKeyDown(KeyCode.F5)) LoadSnapshot(1);
        else if (Input.GetKeyDown(KeyCode.F6)) LoadSnapshot(2);
        else if (Input.GetKeyDown(KeyCode.F7)) LoadSnapshot(3);
        else if (Input.GetKeyDown(KeyCode.F8)) LoadSnapshot(4);

        if(GameManager.instance.isPaused)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            SpawnPattern();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) SpawnPattern(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) SpawnPattern(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) SpawnPattern(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) SpawnPattern(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) SpawnPattern(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) SpawnPattern(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) SpawnPattern(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) SpawnPattern(7);
        else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) SpawnPattern(8);
        else if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) SpawnPace();
    }

    public void SaveSnapshot(int slot)
    {
        string data = "";
        data += "Weights";
        for(int i = 0; i < attemptsWeights.Length; i++)
        {
            data += " " + attemptsWeights[i];
        }

        foreach(string key in patternsInfo.Keys)
        {
            data += "\n\n";
            data += "Pattern " + key;
            data += "\n";
            data += "    " + "Attempts";

            foreach(int a in patternsInfo[key].attempts)
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

        string patternName = null;

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

                    patternName = null;
                }
                else if (parameters[0] == "Pattern")
                {
                    patternName = parameters[1];
                }
            }
            else if(indentationLevel == 1)
            {
                if(patternsInfo.ContainsKey(patternName) && parameters[0] == "Attempts")
                {
                    for (int p = 1; p < parameters.Length; p++)
                    {
                        patternsInfo[patternName].AddAttempt(int.Parse(parameters[p]) == 1);
                    }
                }
            }
        }

        if(slot != 0)
        {
            Debug.Log("Loaded snapshot " + slot);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Transform parent = other.transform.parent;

        if (other.tag == "Ground" && parent.tag == "Untagged")
        {
            RepositionGround();
        }
        else if (other.tag == "Ceiling" && parent.tag == "Untagged")
        {
            RepositionCeiling();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Transform parent = other.transform.parent;

        if (parent.tag == "Pattern")
        {
            float blockLength = other.gameObject.GetComponentInParent<PatternController>().length;

            if (blockLength == other.transform.localPosition.x + other.transform.localScale.x)
            {
                Destroy(parent.gameObject);
            }
        }
    }

    bool CanSpawn(string patternName)
    {
        return TreeManager.instance.IsPatternEnabled(patternName);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PatternManager))]
public class PatternManagerEditor : Editor
{
    PatternManager controller;

    public override void OnInspectorGUI()
    {
        if(controller == null)
        {
            controller = (PatternManager)target;
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