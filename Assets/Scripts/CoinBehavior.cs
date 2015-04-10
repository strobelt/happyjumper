using UnityEngine;
using System.Collections;

public class CoinBehavior : MonoBehaviour {

	public int pontos;

	private bool shouldRotate = true, pego = false;
	private Transform transf;

	void Start()
	{
		transf = GetComponent<Transform> ();
	}

	// Update is called once per frame
	void Update () 
	{
		if (shouldRotate)
			transf.Rotate (Vector3.up * -72 * Time.deltaTime);
		else {
			transf.Rotate (Vector3.up * 720 * Time.deltaTime);
			transf.Translate(Vector2.up * 5 * Time.deltaTime);
			Destroy (this.gameObject, 0.5f);
		}
	}

	public PickupObject Pegar()
	{
		if (!pego) {
			shouldRotate = false;
			pego = true;
			return new PickupObject(){
				Quantity = 1,
				Score = pontos,
				Type = PickupType.Coin
			};
		}
		else
		{
			return null;
		}
	}
}
