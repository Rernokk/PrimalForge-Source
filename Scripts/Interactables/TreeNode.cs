using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNode : ResourceNode {

	protected override void Start () {
    base.Start();
    transform.name = reagentName.ToString().ToUpper()[0] + reagentName.ToString().ToLower().Substring(1) + "_Tree";
    requiredToolType = ToolType.HATCHET;
    AssignToHolder("TreeNode_Parent");
    skillExperience = Skills.Lumbering;
		audioSource.clip = AudioManager.instance.FetchSoundClip("Lumbering00").clip;
	}
}
