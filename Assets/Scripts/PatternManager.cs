using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System;

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

    public PlayerController playerController;
    private float playerMoveSpeed;

    private GameObject[] patterns;

    public Dictionary<string, PatternInfo> patternsInfo;

    [HideInInspector]
    public int savedAttempts;
    public float[] attemptsWeights;

    private string snapshotsPath;

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

        playerMoveSpeed = playerController.moveSpeed;

        patterns = PatternGenerator.instance.GetGeneratedPatterns();

        patternsInfo = new Dictionary<string, PatternInfo>();

        foreach(GameObject pattern in patterns)
        {
            patternsInfo[pattern.name] = new PatternInfo(pattern.name);
        }

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

    void Start()
    {
        for (float i = groundDistance; i < maxStartingGroundDistance; i += groundPrefab.transform.localScale.x)
        {
            GameObject ground = (GameObject) Instantiate(groundPrefab);
            ground.transform.position = new Vector2(groundDistance, groundHeight);

            if (hideBlocksInHierarchy)
            {
                ground.hideFlags = HideFlags.HideInHierarchy;
            }

            ground.transform.SetParent(this.transform);
            ground.transform.SetAsLastSibling();

            GameObject ceiling = (GameObject) Instantiate(ceilingPrefab);
            ceiling.transform.position = new Vector2(ceilingDistance, ceilingHeight);

            if (hideBlocksInHierarchy)
            {
                ceiling.hideFlags = HideFlags.HideInHierarchy;
            }

            ceiling.transform.SetParent(this.transform);
            ceiling.transform.SetAsLastSibling();

            groundDistance += groundPrefab.transform.localScale.x;
            ceilingDistance += ceilingPrefab.transform.localScale.x;
        }
    }

    void Update()
    {
        boxCollider.offset += new Vector2(playerMoveSpeed * Time.deltaTime, 0);

        ProcessInput();
    }

    void SpawnPattern()
    {
        int index = UnityEngine.Random.Range(0, patterns.Length);
        SpawnPattern(index);
    }

    void SpawnPattern(int index)
    {
        if(index < 0 || index >= patterns.Length)
        {
            return;
        }

        GameObject newPattern = Instantiate(patterns[index]) as GameObject;
        newPattern.name = patterns[index].name;

        float patternLength = newPattern.GetComponent<PatternController>().length;

        newPattern.transform.position += Vector3.right * groundDistance;
        newPattern.transform.SetParent(this.transform);
        newPattern.hideFlags = hideBlocksInHierarchy ? HideFlags.HideInHierarchy : HideFlags.None;

        groundDistance += patternLength;
        ceilingDistance += patternLength;
    }

    private GameObject GetFirstObstacleWithTag(string tag)
    {
        int index;
        return GetFirstObstacleWithTag(tag, out index);
    }

    private GameObject GetFirstObstacleWithTag(string tag, out int index)
    {
        index = -1;

        foreach(Transform obstacle in transform)
        {
            index++;

            if (obstacle.tag == tag)
            {
                return obstacle.gameObject;
            }
        }

        return null;
    }

    private GameObject GetLastObstacleWithTag(string tag)
    {
        int index;
        return GetLastObstacleWithTag(tag, out index);
    }

    private GameObject GetLastObstacleWithTag(string tag, out int index)
    {
        index = -1;

        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform obstacle = transform.GetChild(i);
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

        if (firstGround != null)
        {
            Vector2 newPosition = new Vector3(groundDistance, groundHeight, 0);

            firstGround.transform.position = newPosition;
            firstGround.transform.SetAsLastSibling();

            groundDistance += groundPrefab.transform.localScale.x;
        }
    }

    public void RepositionCeiling()
    {
        GameObject firstCeiling = GetFirstObstacleWithTag("Ceiling");

        if (firstCeiling != null)
        {
            Vector2 newPosition = new Vector3(ceilingDistance, ceilingHeight, 0);

            firstCeiling.transform.position = newPosition;
            firstCeiling.transform.SetAsLastSibling();

            ceilingDistance += ceilingPrefab.transform.localScale.x;
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

        if (Input.GetKeyDown(KeyCode.F1)) SaveSnapshot(1);
        else if (Input.GetKeyDown(KeyCode.F2)) SaveSnapshot(2);
        else if (Input.GetKeyDown(KeyCode.F3)) SaveSnapshot(3);
        else if (Input.GetKeyDown(KeyCode.F4)) SaveSnapshot(4);

        if (Input.GetKeyDown(KeyCode.F5)) LoadSnapshot(1);
        else if (Input.GetKeyDown(KeyCode.F6)) LoadSnapshot(2);
        else if (Input.GetKeyDown(KeyCode.F7)) LoadSnapshot(3);
        else if (Input.GetKeyDown(KeyCode.F8)) LoadSnapshot(4);

        if (Input.GetKeyDown(KeyCode.H)) {
            hideBlocksInHierarchy = !hideBlocksInHierarchy;
            UpdateBlocksVisibilityInHierarchy();
        }
    }

    void SaveSnapshot(int slot)
    {
        string data = "";
        data += "Weights";
        for(var i = 0; i < attemptsWeights.Length; i++)
        {
            data += " " + attemptsWeights[i];
        }
        data += "\n";

        foreach(string key in patternsInfo.Keys)
        {
            data += "\n";
            data += key;

            List<int> attemps = patternsInfo[key].attempts;

            foreach (int attempt in attemps) {
                data += " " + attempt;
            }
        }
        data += "\n";

        string filePath = snapshotsPath + "/Snapshot" + slot + ".txt";
        FileStream file = File.OpenWrite(filePath);
        foreach (byte b in Encoding.ASCII.GetBytes(data))
        {
            file.WriteByte(b);
        }
        file.Close();

        Debug.Log("Saved snapshot " + slot);
    }

    void LoadSnapshot(int slot)
    {
        string filePath = snapshotsPath + "/Snapshot" + slot + ".txt";
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
                if(parameters[0] == "Weights")
                {
                    attemptsWeights = new float[parameters.Length - 1];

                    for(var p = 1; p < parameters.Length; p++)
                    {
                        attemptsWeights[p - 1] = float.Parse(parameters[p]);
                    }
                }
                else
                {
                    string patternName = parameters[0];

                    if (patternsInfo.ContainsKey(patternName))
                    {
                        PatternInfo info = new PatternInfo(patternName);
                        for (var p = 1; p < parameters.Length; p++)
                        {
                            info.AddAttempt(int.Parse(parameters[p]) == 1);
                        }
                        patternsInfo[patternName] = info;
                    }
                }
            }
        }

        Debug.Log("Loaded snapshot " + slot);
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
}
