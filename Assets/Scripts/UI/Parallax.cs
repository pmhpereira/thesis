using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public float scrollSpeed;
    public float tileSizeX;

	private SpriteRenderer[] renderers;
	private int currentIndex = 0;
	private float currentDistance;

	private PlayerController player;

    void Awake ()
    {
		renderers = transform.GetComponentsInChildren<SpriteRenderer>();
		player = FindObjectOfType<PlayerController>();
    }

    void Update ()
    {
		transform.position += Vector3.right * scrollSpeed * Time.deltaTime;
		currentDistance += Mathf.Abs(scrollSpeed - player.moveSpeed) * Time.deltaTime;

		if(currentDistance > tileSizeX)
		{
			currentDistance -= tileSizeX;

			renderers[currentIndex].transform.localPosition += Vector3.right * tileSizeX * renderers.Length;
			currentIndex = (currentIndex + 1) % renderers.Length;
		}
    }
}