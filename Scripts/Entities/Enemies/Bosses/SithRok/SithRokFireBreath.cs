using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SithRokFireBreath : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.transform.tag == "Player")
		{
			coll.GetComponent<Player_Details_Script>().TakeDamage(50f, ElementalResistances.FIRE);
		}
	}
}
