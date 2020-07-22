using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeep : Townsfolk {
	protected override void Start(){
		base.Start();
		villagerType = VillagerType.SHOPKEEP;
		if (villagerName == "")
			villagerName = "Shopkeep";

		UpdateDisplayInfo();
	}

	public override void Interact()
	{
		Player_Accessor_Script.DialogInterface.StartDialog(this);
	}
}
