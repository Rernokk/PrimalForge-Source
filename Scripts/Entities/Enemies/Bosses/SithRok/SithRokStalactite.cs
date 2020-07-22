using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SithRokStalactite : MonoBehaviour
{
	public void DestroyStalactite()
	{
		if (Vector3.Distance(Player_Accessor_Script.DetailsScript.transform.position, transform.position) < 2f)
		{
			Player_Accessor_Script.DetailsScript.TakeDamage(10f, ElementalResistances.PHYSICAL);
			Camera.main.GetComponent<Camera_Controller_Script>().TriggerShake(.5f, 2f);
		}
		Destroy(gameObject);
	}
}
