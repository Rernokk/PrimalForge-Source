using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Reputation_Script : MonoBehaviour
{
	bool isInteracting = false;

	public bool IsInteracting
	{
		get
		{
			return isInteracting;
		}

		set
		{
			isInteracting = value;
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.I_INTERACT]))
		{
			if (!isInteracting)
			{
				Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, 1.5f, LayerMask.NameToLayer("Friendlies"));
				for (int i = 0; i < nearby.Length; i++)
				{
					if (nearby[i].GetComponent<Townsfolk>())
					{
						nearby[i].GetComponent<Townsfolk>().Interact();
						break;
					}
				}
			}
			else
			{
				Player_Accessor_Script.DialogInterface.ProgressDialog();
			}
		}
	}
}
