using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Pickaxe : Tool
{

  // Use this for initialization
  public Pickaxe () : base()
  {
    toolType = ToolType.PICKAXE;
    ItemName = "Pickaxe";
		requiredSkill = Skills.Mining;
	}
}
