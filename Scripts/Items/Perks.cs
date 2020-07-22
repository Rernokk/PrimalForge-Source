using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PerkSlot
{
	Helmet, Chestplate, Platelegs,
	Staff,
	Sword,
	RecurveBow,
	SacrificialKnife,
	Censer,
	Vessel,
	Unassigned
}

public enum LegendaryPerk
{
	//Bow
	Fragmentation_Shot,

	//Sword
	Steady_Rage, Heavy_Handed,

	//Sacrificial Knife
	Oranthuls_Grasp, Violent_Kings_Skullcrown,

	//Censer
	Etherias_Gift
}
public class Perks : MonoBehaviour
{
	public static Dictionary<PerkSlot, Dictionary<LegendaryPerk, string>> perkDescriptions;
	
	private void Awake()
	{
		perkDescriptions = new Dictionary<PerkSlot, Dictionary<LegendaryPerk, string>>();
		for (int i = 0; i < System.Enum.GetValues(typeof(PerkSlot)).Length - 1; i++)
		{
			perkDescriptions.Add((PerkSlot)i, new Dictionary<LegendaryPerk, string>());
		}
		perkDescriptions[PerkSlot.RecurveBow].Add(LegendaryPerk.Fragmentation_Shot, "Shots fired from Spray detonate after a short delay, spreading new projectiles.");
		perkDescriptions[PerkSlot.RecurveBow].Add(LegendaryPerk.Steady_Rage, "Enrage lasts longer, but increases damage by less.");
		perkDescriptions[PerkSlot.Sword].Add(LegendaryPerk.Steady_Rage, "Enrage lasts longer, but increases damage by less.");
		perkDescriptions[PerkSlot.Sword].Add(LegendaryPerk.Heavy_Handed, "Cleave has increased range and damage.");

		perkDescriptions[PerkSlot.SacrificialKnife].Add(LegendaryPerk.Oranthuls_Grasp, "Command Undead now can summon up to three more minions, and you may have one additioanl Flesh Construct. Dark Arcane Magic also increases decay rate of minion health.");
		perkDescriptions[PerkSlot.SacrificialKnife].Add(LegendaryPerk.Violent_Kings_Skullcrown, "Command Undead empowered by Dark Arcane Magic will now use slower projectiles that detonate on impact, dealing damage to surrounding foes.");

		perkDescriptions[PerkSlot.Censer].Add(LegendaryPerk.Etherias_Gift, "Your lifeweaving spells now replicate healing effects and resistance boosts to your minions, while reducing their effectiveness.");
	}

	public static PerkSlot ConvertTypeToSlot(WeaponType type)
	{
		foreach (PerkSlot perkEnum in System.Enum.GetValues(typeof(PerkSlot)))
		{
			if (type.ToString().ToLower() == perkEnum.ToString().ToLower())
			{
				return perkEnum;
			}
		}
		return PerkSlot.Unassigned;
	}

	public static LegendaryPerk ConvertTextToPerk(string text)
	{
		for (int i = 0; i < System.Enum.GetValues(typeof(LegendaryPerk)).Length; i++)
		{
			if (text == ((LegendaryPerk)i).ToString())
			{
				return (LegendaryPerk)(i);
			}
		}
		return LegendaryPerk.Fragmentation_Shot;
	}
}
