using UnityEngine;

public class PatternCheckpoint : MonoBehaviour
{
    private PatternController patternController;

    void Awake()
    {
        patternController = GetComponentInParent<PatternController>();
    }

	// When the player enters the green (start) checkpoint
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.transform.tag == "Player" 
            && !patternController.isRecordingPlayer 
            && this.gameObject == patternController.startCollider.gameObject)
        {
            Destroy(this.GetComponent<BoxCollider2D>());
            patternController.isRecordingPlayer = true;
        }
    }

	// When the player exits the red (end) checkpoint
    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "Player" 
            && patternController.isRecordingPlayer 
            && !patternController.hasPlayerCollided
            && this.gameObject == patternController.endCollider.gameObject)
        {
            Destroy(this.GetComponent<BoxCollider2D>());
            patternController.isRecordingPlayer = false;

            patternController.AddAttempt(true);
        }
    }
}
