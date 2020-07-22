using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Hatchet : Tool
{

  public Hatchet() : base()
  {
    ItemName = "Hatchet";
    toolType = ToolType.HATCHET;
		requiredSkill = Skills.Lumbering;
  }
}
