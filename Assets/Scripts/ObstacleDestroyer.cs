using UnityEngine;
using System.Collections;

public class ObstacleDestroyer : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Ground")
        {
            ObstacleController.instance.RepositionGround();
        }
        else if(other.tag == "Ceiling")
        {
            ObstacleController.instance.RepositionCeiling();
        }
        else if (other.tag == "Obstacle" && other.transform.parent.tag != "Ground")
        {
            ObstacleController.instance.RemoveObstacle(other.gameObject);
        }
    }
}
