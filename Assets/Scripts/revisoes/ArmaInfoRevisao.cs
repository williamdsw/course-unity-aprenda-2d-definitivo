using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaInfoRevisao : MonoBehaviour 
{
	private float minDamage;
	private float maxDamage;
	private int damageType;

	// Properties
	
	public float MinDamage 
	{
		get { return this.minDamage; }
		set { minDamage = value; }
	}
	
	public float MaxDamage 
	{
		get { return this.maxDamage; }
		set { maxDamage = value; }
	}

	public int DamageType 
	{
		get { return this.damageType; }
		set { damageType = value; }
	}
}