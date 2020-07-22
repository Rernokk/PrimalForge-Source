using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MetalNode : ResourceNode
{
  
	// Use this for initialization
	protected override void Start()
  {
    base.Start();
    transform.name = reagentName.ToString().ToUpper()[0] + reagentName.ToString().ToLower().Substring(1) + "_Vein";
    requiredToolType = ToolType.PICKAXE;
    AssignToHolder("MetalNode_Parent");
    skillExperience = Skills.Mining;
		audioSource.clip = AudioManager.instance.FetchSoundClip("Mining00").clip;
  }
}
