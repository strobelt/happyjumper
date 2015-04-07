using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float jumpHeight;
	public Camera mainCamera;
	public GameObject floor;
	public GUIText centerText;
	public GUIText scoreText;

	private Rigidbody rBody;
	private float maxDistance = 0.0f;
	private bool gameIsOver = false;

	void Start()
	{
		rBody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate()
	{
		float horizontalMovement = Input.GetAxis ("Horizontal") * 150f * Time.deltaTime;
		Vector3 movement = new Vector3 (horizontalMovement, 0.0f, 0.0f);
		rBody.AddForce (movement);
	}

	void Update()
	{
		if (!gameIsOver) 
		{
			// Move o jogador na horizontal
			rBody.position = new Vector3 (
			Mathf.Clamp (rBody.position.x, -4.2f, 4.2f), 
			rBody.position.y, 
			rBody.position.z
			);

			// Verifica se o jogador saiu de vista
			if (!CanIBeSeen ()) {
				GameOver ();
			}

			// Atualiza pontuaçao
			float distance = Vector3.Distance (GetComponent<Transform> ().position, floor.transform.position);
			if (distance > maxDistance)
				maxDistance = distance;
			scoreText.text = Mathf.FloorToInt (maxDistance * 100) + " pts.";
		}	
	}

	private void GameOver()
	{
		centerText.text = "Game Over :(\n" + scoreText.text;
		scoreText.text = "";
		gameIsOver = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if (!gameIsOver) 
		{
			bool isUnderPlayer = other.gameObject.transform.position.y < rBody.position.y;
			if (other.gameObject.tag.Equals ("Platform") && isUnderPlayer) {
				Jump ();
			}
		}
	}

	void Jump()
	{
		Vector3 movement = new Vector3 (0.0f, jumpHeight, 0.0f);
		rBody.velocity = Vector3.zero;
		rBody.angularVelocity = Vector3.zero;
		rBody.AddForce (movement);
	}

	private bool CanIBeSeen() {
		
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);		
		
		if (GeometryUtility.TestPlanesAABB(planes , GetComponent<Collider>().bounds))
			return true;
		else
			return false;
	}
}
