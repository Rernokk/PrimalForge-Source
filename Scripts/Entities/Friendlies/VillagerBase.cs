using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerBase : Townsfolk {
	public override void Interact()
	{
		chatDialog.Clear();
		chatDialog.Add(Dialog_Interface.GenericLines[Random.Range(0, Dialog_Interface.GenericLines.Length)]);
		Player_Accessor_Script.DialogInterface.StartDialog(this);
	}
}
