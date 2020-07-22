using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Animator_Script : MonoBehaviour
{
	private bool isCasting = false;
	public bool IsCastingAnimation
	{
		get
		{
			return isCasting;
		}

		set
		{
			isCasting = value;
		}
	}

	public void StartCastingAnimation()
	{
		isCasting = true;
	}

	public void EndCastingAnimation()
	{
		isCasting = false;
	}
}
