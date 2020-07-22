using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerFireController : MonoBehaviour
{
	private DancerController rootReference;
	private bool elementalActive = false;
	private Animator animCtrller;

	[SerializeField]
	GameObject fireWallPrefab;

	#region Properties
	public DancerController DancerRootReference
	{
		get
		{
			return rootReference;
		}

		set
		{
			rootReference = value;
		}
	}

	public bool ElementalActive
	{
		get
		{
			return elementalActive;
		}

		set
		{
			if (!elementalActive && value)
			{
				GetComponent<Animator>().Play("Activate");
			}
			else if (!value && elementalActive)
			{
				GetComponent<Animator>().Play("Deactivate");
			}
			elementalActive = value;
		}
	}
	#endregion

	public void Awake()
	{
		animCtrller = GetComponent<Animator>();
	}

	public void ActivateAttack()
	{
		if (!elementalActive)
		{
			return;
		}
		FireWall();
	}

	private void FireWall()
	{
		rootReference.SetAttackTimer(2f);
		GameObject fireRef = Instantiate(fireWallPrefab, transform.position + new Vector3(transform.name.IndexOf("L_") != -1 ? 1 : -1, 0) * .5f, Quaternion.identity);
		fireRef.GetComponent<DancerFlameWallController>().WaveDirection = transform.name.IndexOf("L_") != -1 ? 1 : -1;
		fireRef.transform.Find("Top_Wall/FireParticleBase").GetComponent<ParticleSystem>().Play();
		fireRef.transform.Find("Mid_Wall/FireParticleBase").GetComponent<ParticleSystem>().Play();
		fireRef.transform.Find("Bottom_Wall/FireParticleBase").GetComponent<ParticleSystem>().Play();
		Destroy(fireRef, 8f);
	}
}
