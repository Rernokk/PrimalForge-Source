using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Helmet : Armor
{
	public Helmet () : base ()
	{
		type = ArmorType.HELMET;
	}
}
