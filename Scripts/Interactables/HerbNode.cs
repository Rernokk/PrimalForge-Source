using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbNode : ResourceNode
{
  void Start()
  {
    base.Start();
    transform.name = reagentName.ToString().ToUpper()[0] + reagentName.ToString().ToLower().Substring(1) + "_";
    requiredToolType = ToolType.SICKLE;
    AssignToHolder("HerbNode_Parent");
    skillExperience = Skills.Gathering;
		audioSource.clip = AudioManager.instance.FetchSoundClip("Gathering00").clip;
	}
}
