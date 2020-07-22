using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Hat : Armor
{
	public Hat() : base()
	{
		type = ArmorType.HAT;
	}
}
