using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public float yOffset;
	public GameObject player;

	private Transform transf;
	private float playerHeight;
	private float maxPlayerHeight;

	void Start () {
		transf = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		playerHeight = player.transform.position.y + yOffset;
		if (playerHeight > maxPlayerHeight)
			maxPlayerHeight = playerHeight;
		
		if(playerHeight > transf.position.y || maxPlayerHeight - playerHeight < yOffset)
			transf.position = new Vector3(
				transf.position.x,
				playerHeight,
				transf.position.z
				);
	}
}