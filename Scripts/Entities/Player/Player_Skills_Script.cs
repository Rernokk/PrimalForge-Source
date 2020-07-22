using Facepunch.Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

////Crafting Skills
//public Skill alchemy, infusion, divination, bowcrafting, gemMastery, blacksmithing, carpentry, cooking, etching;

////Combat Skills
//public Skill marksmanship, swordsmanship, guard;

////Magic Skills
//public Skill conjuration, fleshweaving, elementalism, witchcraft, occultism;

////Gathering Skills
//public Skill fishing, mining, lumbering, forestry, soulConjuration;

////Utility Skills
//public Skill champion, hunting, trading, thieving;

public enum Skills
{
	Swordsmanship,
	Marksmanship,
	Alchemy,
	Infusion,
	Divination,
	Bowcrafting,
	GemMastery,
	Blacksmithing,
	Carpentry,
	Cooking,
	Etching,
	Conjuration,
	Fleshweaving,
	Elementalism,
	Witchcraft,
	Occultism,
	Fishing,
	Mining,
	Lumbering,
	Gathering,
	SoulConjuration,
	Champion,
	Hunting,
	Trading,
	Thieving,
	Necromancy,
	Lifeweaving,
	Weaving,
	Leatherworking,
	None
}
public class Player_Skills_Script : MonoBehaviour
{
	private Level_Interface levelInterface;
	private bool expChanged = false;
	private List<float> ExpPerLevelReq = new List<float>();
	public CharacterSkillDetails SkillDetails { get; set; }

	//Prebuilding Exp Requirements:
	public float CalcExpReq(int lv)
	{
		if (lv == 0)
		{
			return 300;
		}

		return Mathf.Ceil(300 + Mathf.Pow(lv, 3) / 6 + (CalcExpReq(lv - 1) * .05f));
	}

	private void Awake()
	{
		SkillDetails = new CharacterSkillDetails();
		if (!File.Exists("SkillSet.dat"))
		{
			SkillDetails.ResetSkills();
			SaveSkills();
		}
		for (int i = 0; i <= 120; i++)
		{
			ExpPerLevelReq.Add(CalcExpReq(i));
		}
		LoadSkills();
	}

	private void Start()
	{
		levelInterface = GameObject.Find("Interface/Levels/Window").GetComponent<Level_Interface>();
		//levelInterface.UpdateSkillDisplay();
	}

	private void Update()
	{
		//TODO: Debug, Adding Experience
		if (Input.GetKeyDown(KeybindManager.Keybinds[KeybindFunction.D_ADDEXP]))
		{
			foreach (Skills skill in SkillDetails.skillList.Keys)
			{
				AddExperience(skill, 10000);
			}
			//AddExperience(Skills.Alchemy, (int)GetSkill(Skills.Alchemy).reqExp);
		}
	}

	public void AddExperience(Skills skill, int amount)
	{
		//print("Adding " + amount + " to " + skill + " req is : " + ExpPerLevelReq[GetSkill(skill).maxLevel + 1]);
		if (GetSkill(skill).maxLevel < 120)
		{
			SkillDetails.AddExperience(skill, amount, ExpPerLevelReq[GetSkill(skill).maxLevel + 1]);
			expChanged = true;
		}
	}

	public void LoadSkills()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream stream = new FileStream("SkillSet.dat", FileMode.Open);
		SkillDetails = (CharacterSkillDetails)bf.Deserialize(stream);
		stream.Close();
	}

	public void SaveSkills()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream stream = new FileStream("SkillSet.dat", FileMode.Create);
		bf.Serialize(stream, SkillDetails);
		stream.Close();
	}

	public static Skills ConvertStringToSkill(string str)
	{
		string strRef = "";
		foreach (char character in str)
		{
			if (character != ' ')
			{
				strRef += character;
			}
		}

		foreach (Skills s in Player_Accessor_Script.SkillsScript.SkillDetails.skillList.Keys)
		{
			if (s.ToString() == strRef)
			{
				return s;
			}
		}
		return Skills.None;
	}

	public Skill GetSkill(int num)
	{
		if (SkillDetails == null || num >= SkillDetails.dictionaryKeys.Count)
		{
			return null;
		}

		if (SkillDetails.skillList.ContainsKey(SkillDetails.dictionaryKeys[num]))
		{
			return SkillDetails.skillList[SkillDetails.dictionaryKeys[num]];
		}
		else
		{
			return null;
		}
	}

	public Skill GetSkill(string str)
	{
		if (SkillDetails == null)
		{
			return null;
		}
		Skills ret = ConvertStringToSkill(str);
		if (ret != Skills.None)
		{
			return SkillDetails.skillList[ret];
		}
		return null;
	}

	public Skill GetSkill(Skills name)
	{
		if (SkillDetails == null)
		{
			return null;
		}
		return SkillDetails.skillList[name];
	}

	private void LateUpdate()
	{
		if (expChanged)
		{
			levelInterface.PopulateInterface();
			SaveSkills();
			expChanged = false;
		}
	}
}

[Serializable]
public class CharacterSkillDetails
{
	////Crafting Skills
	//public Skill alchemy, infusion, divination, bowcrafting, gemMastery, blacksmithing, carpentry, cooking, etching;

	////Combat Skills
	//public Skill marksmanship, swordsmanship, guard;

	////Magic Skills
	//public Skill conjuration, fleshweaving, elementalism, witchcraft, occultism;

	////Gathering Skills
	//public Skill fishing, mining, lumbering, forestry, soulConjuration;

	////Utility Skills
	//public Skill champion, hunting, trading, thieving;

	public Dictionary<Skills, Skill> skillList;
	public List<Skills> dictionaryKeys;

	public CharacterSkillDetails()
	{
		skillList = new Dictionary<Skills, Skill>();
		dictionaryKeys = new List<Skills>();
		ResetSkills();
	}

	public void AddExperience(Skills skill, int amount, float reqExp)
	{
		int startLv = skillList[skill].currLevel;
		skillList[skill].AddExp(amount, reqExp);
		if (skillList[skill].currLevel != startLv)
		{
			int lowest = 120;
			foreach (Skill s in skillList.Values)
			{
				if (s.currLevel < lowest)
				{
					lowest = s.currLevel;
				}
			}

			for (int i = 1; i <= 12; i++)
			{
				if (i * 10 <= lowest)
				{
					if (Client.Instance.Achievements.Find("ADVENTURER_LEVEL_" + i * 10).State == false)
					{
						Client.Instance.Achievements.Find("ADVENTURER_LEVEL_" + i * 10).Trigger();
					}
				}
			}
		}
	}

	public void ResetSkills()
	{
		dictionaryKeys = new List<Skills>();
		skillList = new Dictionary<Skills, Skill>
		{
			{ Skills.Alchemy, new Skill("Alchemy") },
			{ Skills.Infusion, new Skill("Infusion") },
			{ Skills.Divination, new Skill("Divination") },
			{ Skills.Bowcrafting, new Skill("Bowcrafting") },
			{ Skills.GemMastery, new Skill("Gem Mastery") },
			{ Skills.Blacksmithing, new Skill("Blacksmithing") },
			{ Skills.Carpentry, new Skill("Carpentry") },
			{ Skills.Cooking, new Skill("Cooking") },
			{ Skills.Etching, new Skill("Etching") },
			{ Skills.Weaving, new Skill("Weaving") },
			{ Skills.Leatherworking, new Skill("Leatherworking") },

			{ Skills.Fishing, new Skill("Fishing") },
			{ Skills.Mining, new Skill("Mining") },
			{ Skills.Lumbering, new Skill("Lumbering") },
			{ Skills.Gathering, new Skill("Gathering") },
			{ Skills.SoulConjuration, new Skill("Soul Conjuration") },

			{ Skills.Marksmanship, new Skill("Marksmanship") },
			{ Skills.Swordsmanship, new Skill("Swordsmanship") },
			//{ Skills.Endurance, new Skill("Endurance") },
			{ Skills.Conjuration, new Skill("Conjuration") },
			{ Skills.Necromancy, new Skill("Necromancy") },
			{ Skills.Elementalism, new Skill("Elementalism") },
			{ Skills.Witchcraft, new Skill("Witchcraft") },
			{ Skills.Lifeweaving, new Skill("Lifeweaving") },

			//{ Skills.Champion, new Skill("Champion") },
			//{ Skills.Hunting, new Skill("Hunting") },
			//{ Skills.Trading, new Skill("Trading") },
			//{ Skills.Thieving, new Skill("Thieving") }
		};

		foreach (Skills s in skillList.Keys)
		{
			dictionaryKeys.Add(s);
		}
	}

}

[Serializable]
public class Skill
{
	public string skillName;
	public int maxLevel;
	public int currLevel;
	public float currExp;
	public float reqExp;

	public Skill(string str)
	{
		Reset(str);
	}

	public void Reset(string name)
	{
		skillName = name;
		maxLevel = 1;
		currLevel = 1;
		currExp = 0;
	}

	public void AddExp(float amount, float ReqExp)
	{
		currExp += amount;
		reqExp = ReqExp;
		CheckIfLeveled();
	}

	private void CheckIfLeveled()
	{
		if (maxLevel > 120)
		{
			maxLevel = 120;
			return;
		}

		while (reqExp <= currExp)
		{
			currExp -= reqExp;
			maxLevel += 1;
			currLevel += 1;
			if (maxLevel > 120)
			{
				currLevel = 120;
				maxLevel = 120;
				break;
			}
		}
	}

	public override string ToString()
	{
		return skillName + "\nMax: " + maxLevel + "\nCurrent: " + currLevel + "\nExperience: " + currExp + "\nRequired: " + reqExp;
	}
}