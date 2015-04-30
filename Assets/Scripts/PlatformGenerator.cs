using UnityEngine;
using System.Collections;

public class PlatformGenerator : MonoBehaviour {

	public GameObject platformPrefab;
	public GameObject coinPrefab;
	public GameObject enemyPrefab;
	public Camera mainCamera;

	public float maxPlatformXSpacementFromLast = 5f;
	public float platformMaxYSpacement = 1f;
	public float platformMinYSpacement = 0.8f;
	public float startingHeight = 2;
	public float coinOffset = 0.7f;
	public float coinChance = 0.6f;
	public float enemyOffset = 0.7f;
	public float enemyChance = 0.8f;
	public int minimumPlatformsBetweenEnemies = 6;
	public float enemyMovementSpace = 2f;

	private GameObject lastPlatform;
	private bool lastHadCoin = false;
	private bool lastHadEnemy = false;
	private int platformsBetweenEnemies = 0;

	private float minPlatformXPosition = -4f, maxPlatformXPosition = 4f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (CanIBeSeen()) 
		{
			if(this.gameObject.transform.position.y >= startingHeight)
				spawnPlatformAndCoin();

			this.gameObject.transform.position = new Vector3(
				this.gameObject.transform.position.x,
				this.gameObject.transform.position.y + platformMaxYSpacement,
				this.gameObject.transform.position.z
				);
		}
	}

	private void spawnPlatformAndCoin()
	{
		float x, y;
		bool hasCoin = Random.value >= coinChance;

		#region Platform and coin spawning
		if (lastPlatform == null) {
			x = Random.value * (maxPlatformXPosition * 2 - minPlatformXPosition);
		} else {
			x = lastPlatform.transform.position.x + 
				(Random.value * (2 * maxPlatformXSpacementFromLast) - maxPlatformXSpacementFromLast);
			x = Mathf.Clamp (x, minPlatformXPosition, maxPlatformXPosition);
		}

		y = this.gameObject.transform.position.y + 
			(Random.value * (platformMaxYSpacement - platformMinYSpacement) - platformMinYSpacement);

		if (lastPlatform != null){

			if(lastHadCoin && lastHadEnemy && 
			   (y - lastPlatform.transform.position.y) < (Mathf.Max(coinOffset, enemyOffset) + 0.1f))
			{
				y += lastPlatform.transform.position.y + Mathf.Max(coinOffset, enemyOffset) + 0.1f;
			}
			else if(lastHadCoin && (y - lastPlatform.transform.position.y) < (coinOffset + 0.1f)) 
			{
				y += lastPlatform.transform.position.y + coinOffset + 0.1f;
			}
			else if(lastHadEnemy && (y - lastPlatform.transform.position.y) < (enemyOffset + 0.1f)) 
			{
				y += lastPlatform.transform.position.y + enemyOffset + 0.2f;
			}
			
			if((lastHadCoin || lastHadEnemy) && (y - lastPlatform.transform.position.y) < platformMinYSpacement)
			{
				y = lastPlatform.transform.position.y + platformMinYSpacement;
			}

		}


		lastPlatform = (GameObject)Instantiate (platformPrefab, new Vector3 (x, y, 0), Quaternion.identity);

		if (hasCoin){
			Instantiate (coinPrefab, new Vector3 (
				lastPlatform.transform.position.x,
				lastPlatform.transform.position.y + coinOffset,
				lastPlatform.transform.position.z), Quaternion.identity);
			lastHadCoin = true;
		}
		else
		{
			lastHadCoin = false;
		}
		#endregion

		#region Enemy Spawning
		bool hasEnemy = platformsBetweenEnemies >= minimumPlatformsBetweenEnemies && Random.value >= enemyChance;

		if(hasEnemy){

			float enemyXPosition;

			if(lastPlatform.transform.position.x >= maxPlatformXPosition / 2)
			{
				enemyXPosition = lastPlatform.transform.position.x - enemyMovementSpace;
			}
			else
			{
				enemyXPosition = lastPlatform.transform.position.x + enemyMovementSpace;
			}

			Instantiate(enemyPrefab, new Vector3(
				enemyXPosition,
				lastPlatform.transform.position.y,
				lastPlatform.transform.position.z), Quaternion.identity);

			platformsBetweenEnemies = 0;
			lastHadEnemy = true;
		}
		else 
		{
			platformsBetweenEnemies++;
			lastHadEnemy = false;
		}

		#endregion
	}
	
	private bool CanIBeSeen() {
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(mainCamera);		

		if (GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds))
			return true;
		else
			return false;
	}
}
