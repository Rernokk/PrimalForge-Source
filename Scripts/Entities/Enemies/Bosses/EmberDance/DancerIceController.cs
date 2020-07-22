using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancerIceController : MonoBehaviour
{
	private DancerController rootReference;
	private bool elementalActive, breathingFrost = false;
	private Animator animCtrller;

	[SerializeField]
	private float frostBreathMaxTurningRate = 3f, maxBreathDuration = 5f;
	private float breathTimer = 0f;

	private Transform frostBreathAnchor;

	[SerializeField]
	GameObject iceBoulderPrefab;

	[SerializeField]
	Transform iceBoulderSpawn;


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

	private void Awake()
	{
		animCtrller = GetComponent<Animator>();
		frostBreathAnchor = transform.Find("FrostbreathAnchor");
	}

	private void Update()
	{
		if (breathTimer > 0f)
		{
			breathTimer -= Time.deltaTime;
			if (breathTimer <= 0)
			{
				breathTimer = 0;
				breathingFrost = false;
			}
		}
	}

	private void LateUpdate()
	{
		if (breathingFrost || true)
		{
			Debug.DrawRay(frostBreathAnchor.position, frostBreathAnchor.forward * 8f, Color.cyan);
			Vector3 start = frostBreathAnchor.forward;
			frostBreathAnchor.LookAt(Player_Accessor_Script.EquipmentScript.transform.position, -Vector3.forward);
			Vector3 target = frostBreathAnchor.forward;
			Vector3 newDir = Vector3.RotateTowards(start, target, Time.deltaTime, 0f);
			frostBreathAnchor.rotation = Quaternion.LookRotation(newDir);
		}
	}

	public void ActivateAttack()
	{
		if (!elementalActive)
		{
			return;
		}
		if (!breathingFrost)
		{
			if (Random.Range(0f, 1f) > 1.5f)
			{
				PlayBoulderAnimation();
			}
			else
			{
				FrostBreathAttack();
			}
		}
	}

	public void PlayBoulderAnimation()
	{
		animCtrller.Play("BoulderAnimation");
		rootReference.SetAttackTimer(6f);
	}

	public void SpitIceBoulder()
	{
		GameObject iceBoulderRef = Instantiate(iceBoulderPrefab, iceBoulderSpawn.position, iceBoulderSpawn.rotation);
		iceBoulderRef.GetComponent<Rigidbody2D>().velocity = -iceBoulderRef.transform.up * 3f;
		Destroy(iceBoulderRef, 8f);
	}

	public void FrostBreathAttack()
	{
		breathTimer = maxBreathDuration;
		breathingFrost = true;
	}
}
