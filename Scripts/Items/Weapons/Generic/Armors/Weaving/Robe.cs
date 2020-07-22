using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Robe : Armor
{
	public Robe() : base()
	{
		type = ArmorType.ROBE;
	}
}
