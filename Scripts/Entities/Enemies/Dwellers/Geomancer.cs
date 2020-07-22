using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geomancer : EnemyObject
{
	[SerializeField]
	GameObject thrallObject;

	[SerializeField]
	GeomancerThrall thrallReference;

	#region Properties
	public GeomancerThrall ThrallReference
	{
		get
		{
			return thrallReference;
		}

		set
		{
			thrallReference = value;
		}
	}
	#endregion

	protected override void Start()
	{
		base.Start();
		GameObject tempRef = Instantiate(thrallObject, transform.position, Quaternion.identity);
		thrallReference = tempRef.GetComponent<GeomancerThrall>();
		thrallReference.MasterReference = this;
	}

	protected override void Update()
	{
		base.Update();
		if (TargetEntity == null)
		{
			TargetEntity = GameObject.FindGameObjectWithTag("Player").GetComponent<AttackableEntity>();
		}

		if (ThrallReference == null)
		{
			GameObject tempRef = Instantiate(thrallObject, transform.position, Quaternion.identity);
			thrallReference = tempRef.GetComponent<GeomancerThrall>();
			thrallReference.MasterReference = this;
		}
	}
}
