using UnityEngine;
using System.Collections.Generic;

public class ObstacleController: MonoBehaviour {

    public GameObject groundPrefab;
    public GameObject obstaclePrefab;

    public List<GameObject> obstaclesPool;
    public float moveSpeed;

    public float groundHeight;

    public static ObstacleController instance;

    public float minStartingGroundDistance;
    private float groundDistance;
    public float maxStartingGroundDistance;

    public float obstacleSpawnInterval;
    private float obstacleInterval;

    void Awake()
    {
        if (instance != null && instance != this) {
            Destroy(gameObject);
        }

        instance = this;

        groundDistance = minStartingGroundDistance;
        obstaclesPool = new List<GameObject>();

        BoxCollider boxCollider = GetComponent<BoxCollider>();
        boxCollider.center = new Vector3(minStartingGroundDistance - 3, groundHeight, 0);

        obstacleInterval = 0;
    }

    void Start () {
        for (float i = groundDistance; i < maxStartingGroundDistance; i += groundPrefab.transform.localScale.x)
        {
            GameObject ground = (GameObject)Instantiate(groundPrefab);
            ground.transform.position = new Vector3(groundDistance, groundHeight, 0);
            obstaclesPool.Add(ground);

            ground.hideFlags = HideFlags.HideInHierarchy;
            ground.transform.SetParent(this.transform);

            groundDistance += groundPrefab.transform.localScale.x;
        }
	}

    void Update()
    {
        Vector3 diffDistance = new Vector3(1, 0, 0) * Time.deltaTime * moveSpeed;

        foreach (GameObject obstacle in obstaclesPool)
        {
            Rigidbody rigidbody = obstacle.GetComponent<Rigidbody>();
            rigidbody.MovePosition(rigidbody.position - diffDistance);
        }

        obstacleInterval += Time.deltaTime;

        if (obstacleInterval > obstacleSpawnInterval)
        {
            obstacleInterval = 0;
            SpawnObstacle();
        }
    }

    public void RemoveObstacle(GameObject obstacle) {
        obstaclesPool.Remove(obstacle);
        Destroy(obstacle);
    }

    public void SpawnObstacle()
    {
        GameObject lastGround = GetLastObstacleWithTag("Ground");

        int obstacleHeight = Random.Range(1, 3); 
        int obstacleWidth = Random.Range(1, 5);

        Vector3 groundScale = groundPrefab.transform.localScale;

        for (int x = 1; x <= obstacleWidth; x++)
        {
            for (int y = 1; y <= obstacleHeight; y++)
            {
                GameObject obstacle = (GameObject)Instantiate(obstaclePrefab);
                obstacle.transform.position = new Vector3(groundDistance + groundScale.x * x, groundHeight + groundScale.y * y, 0);
                obstacle.tag = "Obstacle";
                obstaclesPool.Add(obstacle);

                // obstacle.hideFlags = HideFlags.HideInHierarchy;
                obstacle.transform.SetParent(this.transform);

                if(y != obstacleHeight)
                {
                    obstacle.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
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
            Vector3 newPosition = lastGround.transform.position;
            newPosition.x += groundPrefab.transform.localScale.x;
            newPosition.y = groundHeight;

            firstGround.transform.position = newPosition;
            obstaclesPool.Add(firstGround);
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
}
