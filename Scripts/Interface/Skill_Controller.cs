using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Skill_Controller : MonoBehaviour {

	[SerializeField]
	CanvasGroup mouseOverDisplay;
	bool intfUpdated = false;
	
	public void ToggleMouseoverDisplay(){
		mouseOverDisplay.alpha = Mathf.Abs(mouseOverDisplay.alpha - 1);
	}

	void Update(){
		if (!intfUpdated && transform.name.IndexOf("Anchor") == -1){
			UpdateUIElement();
			intfUpdated = true;
		}
		if (transform.name.IndexOf("Anchor") != -1)
		{
			Destroy(gameObject);
		}
	}

	public void UpdateUIElement()
	{
		if (transform.name.IndexOf("Anchor") != -1)
		{
			print("Failed to rename");
			Destroy(gameObject);
			return;
		}
		transform.Find("MouseoverWindow/Name").GetComponent<Text>().text = transform.name;
		Skill skillRef = Player_Accessor_Script.SkillsScript.GetSkill(transform.name);
		transform.Find("MouseoverWindow/Level").GetComponent<Text>().text = skillRef.currLevel + "/" + skillRef.maxLevel;
		transform.Find("MouseoverWindow/Progress").GetComponent<Slider>().value = skillRef.currExp / (skillRef.reqExp+1);
		transform.Find("MouseoverWindow/Exp").GetComponent<Text>().text = ((int)skillRef.currExp + "/" + (int)skillRef.reqExp + " (" + ((int)((skillRef.currExp / (skillRef.reqExp + 1)) * 1000)/10.0f) + "%)");
	}
}
