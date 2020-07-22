using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Chestplate : Armor
{
	public Chestplate() : base()
	{
		type = ArmorType.CHESTPLATE;
	}
}