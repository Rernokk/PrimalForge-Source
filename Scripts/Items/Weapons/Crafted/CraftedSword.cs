using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CraftedSword : CraftedWeapon
{
	public CraftedSword(CraftedSword template) : base(template)
	{
		RequiredProfession = Skills.Blacksmithing;
		ItemName = template.baseName;
	}

}
