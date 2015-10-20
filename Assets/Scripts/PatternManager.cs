using UnityEngine;
using System.Collections.Generic;

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
        int index = Random.Range(0, patterns.Length);
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

        if (Input.GetKeyDown(KeyCode.H)) {
            hideBlocksInHierarchy = !hideBlocksInHierarchy;
            UpdateBlocksVisibilityInHierarchy();
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
}
