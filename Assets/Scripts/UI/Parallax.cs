using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float scrollSpeed;
    public float tileSizeX;

    private Vector3 startPosition, startParentPosition;

    void Start ()
    {
        startPosition = transform.localPosition;
		startParentPosition = transform.parent.localPosition;
    }

    void Update ()
    {
        float newPosition = Mathf.Repeat(Time.time * scrollSpeed, tileSizeX);
        transform.localPosition = startPosition + Vector3.right * newPosition;

		transform.localPosition = new Vector3(transform.localPosition.x, -transform.parent.localPosition.y + startPosition.y, transform.localPosition.z);
    }
}