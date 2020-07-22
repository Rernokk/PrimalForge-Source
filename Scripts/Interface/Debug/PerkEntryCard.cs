using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkEntryCard : MonoBehaviour
{
	public void PushChangeToRecipe()
	{
		GetComponentInParent<RecipeCrafter>().TogglePerk(transform);
	}
}
