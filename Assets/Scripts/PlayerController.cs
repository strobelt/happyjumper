using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float jumpHeight;
	public Camera mainCamera;
	public GUIText centerText;

	private Rigidbody rBody;

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
		rBody.position = new Vector3 (
			Mathf.Clamp (rBody.position.x, -4.2f, 4.2f), 
			rBody.position.y, 
			rBody.position.z
			);

		if (!CanIBeSeen ()) 
		{
			centerText.text = "Perdeu :(";
		}
	}

	void OnTriggerEnter(Collider other)
	{
		bool isUnderPlayer = other.gameObject.transform.position.y < rBody.position.y;
		if (other.gameObject.tag.Equals("Platform") && isUnderPlayer) 
		{
			Jump();
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
