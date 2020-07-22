using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FishNode : ResourceNode
{

  void Start()
  {
    base.Start();
    transform.name = reagentName.ToString().ToUpper()[0] + reagentName.ToString().ToLower().Substring(1) + "_School";
    requiredToolType = ToolType.FISHING_ROD;
    AssignToHolder("FishNode_Parent");
    skillExperience = Skills.Fishing;
		audioSource.clip = AudioManager.instance.FetchSoundClip("Fishing00").clip;
	}
}
