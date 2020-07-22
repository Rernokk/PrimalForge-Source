using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Interface Manager for levels in skills.
public class Level_Interface : MonoBehaviour
{
	private Player_Skills_Script levelRef;
	private CanvasGroup levelGroup;

	private void Start()
	{
		levelRef = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Skills_Script>();
		levelGroup = transform.parent.GetComponent<CanvasGroup>();
		PopulateInterface();
	}

	// Update is called once per frame
	private void Update()
	{
		if (RecipeCrafter.IsUsingInput)
		{
			return;
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			levelGroup.alpha = 0;
			levelGroup.interactable = false;
			levelGroup.blocksRaycasts = false;
		}

		if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.I_SKILLS]))
		{
			levelGroup.alpha = Mathf.Abs(levelGroup.alpha - 1);
			levelGroup.interactable = !levelGroup.interactable;
			levelGroup.blocksRaycasts = !levelGroup.blocksRaycasts;
			PopulateInterface();
		}
	}

	public void PopulateInterface()
	{

		//Populating Interface Data
		for (int i = 0; i < transform.childCount; i++)
		{
			Skill skillRef = levelRef.GetSkill(i);
			if (skillRef == null)
			{
				continue;
			}

			Transform skillPane = transform.GetChild(i).Find("BG");
			transform.GetChild(i).name = skillRef.skillName;

			//Truncating skill name to fit.
			string abbrName = skillRef.skillName;
			if (abbrName.Length > 8)
			{
				abbrName = abbrName.Substring(0, 8) + ".";
			}
			skillPane.Find("Name").GetComponent<Text>().text = abbrName;
			Sprite temp = Resources.Load<Sprite>("SkillIcons/" + skillRef.skillName);

			if (temp != null) {
				skillPane.Find("Image").GetComponent<Image>().sprite = temp;
			} else
			{
				skillPane.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("SkillIcons/Unassigned");
			}

			skillPane.Find("Level/Current").GetComponent<Text>().text = skillRef.currLevel.ToString();
			skillPane.Find("Level/Max").GetComponent<Text>().text = skillRef.maxLevel.ToString();
		}

		List<Transform> children = new List<Transform>();
		for (int i = transform.childCount - 1; i >= 0 ; i--)
		{
			Transform c = transform.GetChild(i);
			children.Add(c);
			c.parent = null;
		}
		children.Sort((q1, q2) => q1.name.CompareTo(q2.name));
		foreach (Transform child in children)
		{
			child.parent = transform;
		}
		//UpdateSkillDisplay();
	}

	public void UpdateSkillDisplay()
	{
		Skill_Controller[] array = GetComponentsInChildren<Skill_Controller>();
		foreach (Skill_Controller s in array)
		{
			s.UpdateUIElement();
		}
	}

	public void UpdateSkillDisplay(string skill)
	{
		transform.Find(skill).GetComponent<Skill_Controller>().UpdateUIElement();
	}
}
