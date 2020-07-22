using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour {
  Ability_Select_Interface intf;
	CanvasGroup canvGroup;

  private void Start()
  {
    intf = GetComponentInParent<Ability_Select_Interface>();
		canvGroup = GetComponent<CanvasGroup>();
  }

  public void TriggerSkillSwap(){
    intf.ReplaceSkill(transform.name);
  }

	public void DragSkill()
	{
		if (canvGroup.interactable)
		{
			intf.ghostIcon.ActivateTrack();
			intf.ghostIcon.GetComponent<Image>().sprite = AbilityLibrary.abilityDictionary[transform.name].skillIcon;
		}
	}
}
