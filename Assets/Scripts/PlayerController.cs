using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerController : MonoBehaviour {

	public float jumpHeight;
	public Camera mainCamera;
	public GameObject floor;
	public GUIText centerText;
	public GUIText scoreText;

	private Rigidbody rBody;
	private bool gameIsOver = false;
	private float score = 0;
	private float maxDistance = 0.0f;
	List<PickupObject> pickups = new List<PickupObject> ();

	void Start()
	{
		rBody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate()
	{
		float horizontalMovement = Input.GetAxis ("Horizontal") * 250f * Time.deltaTime;
		Vector3 movement = new Vector3 (horizontalMovement, 0.0f, 0.0f);
		rBody.AddForce (movement);
	}

	void Update()
	{
		if (!gameIsOver) {
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
			if (distance > maxDistance) {
				maxDistance = distance;
			}

			CalculaScore ();
			ExibeScore ();
		}
		else
		{
			rBody.transform.Rotate (Vector3.right * 180 * Time.deltaTime);
		}
	}

	private void ExibeScore()
	{
		scoreText.text = Mathf.FloorToInt (score) + " pts.";
	}

	private void CalculaScore()
	{
		int scoreDistancia = Mathf.FloorToInt(maxDistance * 100);
		int scorePickups = 0;
		foreach (var pickup in pickups) {
			scorePickups += pickup.TotalScore();
			//Debug.Log(pickup.Type.ToString() + ":" + pickup.TotalScore());
		}
		score = scoreDistancia + scorePickups;
	}

	private void GameOver()
	{
		rBody.velocity = Vector3.zero;
		centerText.text = "Game Over :(\n" + scoreText.text;
		scoreText.text = "";
		gameIsOver = true;
	}

	void OnTriggerEnter(Collider other)
	{
		if (!gameIsOver) 
		{
			string otherTag = other.gameObject.tag;
			// Vetor que aponta *DESTE* objeto (this) *PARA* o com qual colidiu (other)
			// http://docs.unity3d.com/Manual/DirectionDistanceFromOneObjectToAnother.html
			Vector3 heading = other.transform.position - this.transform.position;
			Debug.Log("Direction" + heading.ToString());
			Debug.Log("Distance" + heading.sqrMagnitude); // Distancia simples, sem raiz inversa para agilizar o processo
			bool isUnderPlayer =  heading.y < -0.1f && Mathf.Abs(heading.x) < other.bounds.size.x - 0.1f;
			if (otherTag.Equals ("Platform") && isUnderPlayer) 
			{
				Jump ();
			}
			else if(otherTag.Equals("Coin"))
			{
				CoinBehavior coinBehavior = (CoinBehavior)other.gameObject.GetComponent<CoinBehavior>();
				var coin = coinBehavior.Pegar();
				if(coin != null)
				{
					if(pickups.Select(po => po.Type).Contains(coin.Type))
					{
						var coinPickup = pickups.Find(po => po.Type == PickupType.Coin);
						coinPickup.Quantity += coin.Quantity;
					}
					else 
					{
						pickups.Add(coin);
					}
				}
			}
			else if(otherTag.Equals("Enemy")){
				GameOver();
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
		//if (Debug.isDebugBuild)	return true;

		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);		
		
		if (GeometryUtility.TestPlanesAABB(planes , GetComponent<Collider>().bounds))
			return true;
		else
			return false;
	}
}
