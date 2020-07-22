using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SithRokTrigger : MonoBehaviour
{
	[SerializeField]
	bool EntryField = false;

	[SerializeField]
	SithRok bossRef;
	private void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.tag == "Player")
		{
			if (EntryField)
			{
				Camera.main.GetComponent<Animator>().Play("PanOut");
				GetComponent<Collider2D>().enabled = false;
				transform.parent.Find("FightExit").GetComponent<Collider2D>().enabled = true;
				transform.root.GetComponent<Collider2D>().enabled = true;
				bossRef.PlayFightStartAnimation();
			} else
			{
				Camera.main.GetComponent<Animator>().Play("PanIn");
				GetComponent<Collider2D>().enabled = false;
				transform.parent.Find("FightTrigger").GetComponent<Collider2D>().enabled = true;
			}
		}
	}
}
