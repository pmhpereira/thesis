using UnityEngine;
using System.Collections.Generic;

public class ObstacleController: MonoBehaviour {

    public static ObstacleController instance;

    public GameObject groundPrefab;
    public GameObject obstaclePrefab;
    public GameObject ceilingPrefab;

    public List<GameObject> obstaclesPool;
    
    public float groundHeight;
    
    public float minStartingGroundDistance;
    public float maxStartingGroundDistance;
    private float groundDistance;

    public int minHoleLength;
    public int maxHoleLength;
    private int remainingHoleLength;

    public float holeSpawnInterval;
    private float holeInterval;
    public bool spawnHoles;

    public float obstacleSpawnInterval;
    private float obstacleInterval;
    public bool spawnObstacles;

    public bool spawnCeiling;
    public float ceilingHeight;

    public bool hideBlocksInHierarchy;

    private BoxCollider2D boxCollider;

    public PlayerController playerController;
    private float playerMoveSpeed;

    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }
            
        instance = this;

        groundDistance = minStartingGroundDistance;
        obstaclesPool = new List<GameObject>();

        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.offset = new Vector2(minStartingGroundDistance - 3, 0);

        obstacleInterval = 0;
        holeInterval = 0;

        playerMoveSpeed = playerController.moveSpeed;
    }

    void Start () {
        for (float i = groundDistance; i < maxStartingGroundDistance; i += groundPrefab.transform.localScale.x)
        {
            GameObject ground = (GameObject)Instantiate(groundPrefab);
            ground.transform.position = new Vector2(groundDistance, groundHeight);
            obstaclesPool.Add(ground);

            if(hideBlocksInHierarchy)
            {
                ground.hideFlags = HideFlags.HideInHierarchy;
            }

            ground.transform.SetParent(this.transform);

            if (spawnCeiling)
            {
                GameObject ceiling = (GameObject)Instantiate(ceilingPrefab);
                ceiling.transform.position = new Vector2(groundDistance, ceilingHeight);
                obstaclesPool.Add(ceiling);

                if (hideBlocksInHierarchy)
                {
                    ceiling.hideFlags = HideFlags.HideInHierarchy;
                }

                ceiling.transform.SetParent(this.transform);
            }

            groundDistance += groundPrefab.transform.localScale.x;
        }
	}

    void Update()
    {
        boxCollider.offset += new Vector2(playerMoveSpeed * Time.deltaTime, 0);

        ProcessInput();

        obstacleInterval += Time.deltaTime;
        holeInterval += Time.deltaTime;

        if (obstacleInterval > obstacleSpawnInterval && spawnObstacles)
        {
            obstacleInterval = 0;
            SpawnObstacle();
        }

        if (holeInterval > holeSpawnInterval && spawnHoles)
        {
            holeInterval = 0;
            SpawnHole();
        }
    }

    public void RemoveObstacle(GameObject obstacle) {
        obstaclesPool.Remove(obstacle);
        Destroy(obstacle);
    }

    public void SpawnObstacle()
    {
        int obstacleHeight = Random.Range(1, 3); 
        int obstacleWidth = Random.Range(1, 5);

        Vector3 groundScale = groundPrefab.transform.localScale;

        GameObject lastObstacle = GetLastObstacleWithTag("Obstacle");

        for (int x = 1; x <= obstacleWidth; x++)
        {
            for (int y = 1; y <= obstacleHeight; y++)
            {
                GameObject obstacle = (GameObject)Instantiate(obstaclePrefab);
                obstacle.transform.position = new Vector2(groundDistance + groundScale.x * x, groundHeight + groundScale.y * y);
                obstacle.tag = "Obstacle";
                obstaclesPool.Add(obstacle);

                if (hideBlocksInHierarchy)
                {
                    obstacle.hideFlags = HideFlags.HideInHierarchy;
                }

                obstacle.transform.SetParent(this.transform);

                if(y != obstacleHeight)
                {
                    obstacle.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
    }

    public void SpawnHole()
    {
        SpawnHole(Random.Range(minHoleLength, maxHoleLength + 1));
    }

    public void SpawnHole(int length)
    {
        remainingHoleLength = length;
    }

    public void RepositionGround()
    {
        int i;
        GameObject firstGround = GetFirstObstacleWithTag("Ground", out i);

        if (firstGround != null)
        {
            obstaclesPool.RemoveAt(i);
        }

        GameObject lastGround = GetLastObstacleWithTag("Ground");

        if (lastGround != null)
        {
            Vector2 newPosition = lastGround.transform.position;
            newPosition.x += groundPrefab.transform.localScale.x;
            newPosition.y = groundHeight;

            bool isHole = remainingHoleLength > 0;
            firstGround.GetComponent<MeshRenderer>().enabled = !isHole;
            firstGround.GetComponent<BoxCollider2D>().isTrigger = isHole;

            if (remainingHoleLength > 0)
            {
                remainingHoleLength--;
            }

            firstGround.transform.position = newPosition;
            groundDistance = newPosition.x;

            obstaclesPool.Add(firstGround);
        }
    }

    public void RepositionCeiling()
    {
        int i;
        GameObject firstCeiling = GetFirstObstacleWithTag("Ceiling", out i);

        if (firstCeiling != null)
        {
            obstaclesPool.RemoveAt(i);
        }

        GameObject lastCeiling = GetLastObstacleWithTag("Ceiling");

        if (lastCeiling != null)
        {
            Vector2 newPosition = lastCeiling.transform.position;
            newPosition.x += groundPrefab.transform.localScale.x;
            newPosition.y = ceilingHeight;

            firstCeiling.transform.position = newPosition;

            obstaclesPool.Add(firstCeiling);
        }
    }

    private GameObject GetFirstObstacleWithTag(string tag)
    {
        int index;
        return GetFirstObstacleWithTag(tag, out index);
    }

    private GameObject GetFirstObstacleWithTag(string tag, out int index)
    {
        for (int i = 0; i < obstaclesPool.Count; i++)
        {
            GameObject obstacle = obstaclesPool[i];

            if (obstacle.tag == tag)
            {
                index = i;
                return obstacle;
            }
        }

        index = -1;
        return null;
    }

    private GameObject GetLastObstacleWithTag(string tag)
    {
        int index;
        return GetLastObstacleWithTag(tag, out index);
    }

    private GameObject GetLastObstacleWithTag(string tag, out int index)
    {
        for (int i = obstaclesPool.Count - 1; i >= 0; i--)
        {
            GameObject obstacle = obstaclesPool[i];

            if (obstacle.tag == tag)
            {
                index = i;
                return obstacle;
            }
        }

        index = -1;
        return null;
    }

    void OnValidate()
    {
        foreach(GameObject block in obstaclesPool)
        {
            block.hideFlags = hideBlocksInHierarchy ? HideFlags.HideInHierarchy : HideFlags.None;
        }
    }

    void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            spawnObstacles = !spawnObstacles;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            spawnHoles = !spawnHoles;
        }
    }
}
