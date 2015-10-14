using UnityEngine;

public class PatternCheckpoint : MonoBehaviour
{
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            PatternManager.instance.patternsInfo[this.transform.parent.name].AddAttempt(true);
            Destroy(this.GetComponent<BoxCollider2D>());
        }
    }
}
