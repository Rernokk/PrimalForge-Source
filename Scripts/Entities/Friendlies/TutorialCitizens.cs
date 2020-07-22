using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCitizens : Townsfolk
{
	protected override void Start()
	{
		base.Start();
		PlayerPrefs.SetString("PlayerName", "Rernokk");
		for (int i = 0; i < chatDialog.Count; i++)
		{
			chatDialog[i] = chatDialog[i].Replace("~", PlayerPrefs.GetString("PlayerName"));
		}
	}
}