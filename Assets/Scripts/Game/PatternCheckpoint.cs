using System.Collections.Generic;
using UnityEngine;

public class PatternCheckpoint : MonoBehaviour
{
    private PatternController patternController;

    void Awake()
    {
        patternController = GetComponentInParent<PatternController>();
    }

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

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.tag == "Player" 
            && patternController.isRecordingPlayer 
            && !patternController.hasPlayerCollided
            && this.gameObject == patternController.endCollider.gameObject)
        {
            Destroy(this.GetComponent<BoxCollider2D>());
            patternController.isRecordingPlayer = false;
            PatternManager.instance.patternsInfo[this.transform.parent.name].AddAttempt(true);

            List<string> tags = TagsManager.instance.PlayerStateToTags(patternController.playerStates.ToArray());
            foreach(string tag in tags)
            {
                TagsManager.instance.tagsInfo[tag].AddAttempt(true);
            }
        }
    }
}
