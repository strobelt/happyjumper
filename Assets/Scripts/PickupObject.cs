using UnityEngine;
using System;

public class PickupObject
{
	public PickupType Type { get; set; }
	public int Quantity { get; set; }
	public float Score { get; set; }

	public int TotalScore()
	{
		return Quantity * Mathf.FloorToInt (Score);
	}
}

public enum PickupType
{
	Coin = 1
}
