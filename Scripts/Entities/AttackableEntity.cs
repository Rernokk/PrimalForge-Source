using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackableEntity : MonoBehaviour
{
	[SerializeField] protected float maxHealth, currentHealth;
	[SerializeField] protected float shieldAmount = 0f;
	public float healMultiplier = 1.0f, resistanceMultiplier = 1.0f;

	//Resistances
	[SerializeField] protected Dictionary<ElementalResistances, float> elementalResists;

	protected bool isBlocking = false;

	//Stun Variables
	protected bool isStunned = false;
	protected float stunDuration;

	//Bleed Variables
	protected bool isBleeding = false;
	protected float bleedAmount = 0;
	protected float bleedDuration = 0;

	//Regeneration (Amount per 5)
	[SerializeField] protected float regenRate = 1.0f;

	//Damage Text Variable
	private GameObject damageTextPrefab;
	protected float damageColorTimer = 0f;
	private Dictionary<Transform, Color> spriteRenderColorReferences = new Dictionary<Transform, Color>();

	#region Properties
	public float MaxHealth
	{
		get
		{
			return maxHealth;
		}

		set
		{
			maxHealth = value;
		}
	}
	public float CurrentHealth
	{
		get
		{
			return currentHealth;
		}

		set
		{
			currentHealth = value;
		}
	}
	public float HealthPercent
	{
		get
		{
			return currentHealth / MaxHealth;
		}
	}
	public float HealMultiplier
	{
		get
		{
			return healMultiplier;
		}
		set
		{
			healMultiplier = value;
		}
	}
	public float ResistanceMultiplier
	{
		get
		{
			return resistanceMultiplier;
		}
		set
		{
			resistanceMultiplier = value;
		}
	}
	public bool IsStunned
	{
		get
		{
			return isStunned;
		}

		set
		{
			isStunned = value;
		}
	}
	public float StunDurationRemaining
	{
		get
		{
			return stunDuration;
		}

		set
		{
			stunDuration = value;
		}
	}
	public bool IsBleeding
	{
		get
		{
			return isBleeding;
		}
		set
		{
			isBleeding = value;
		}
	}
	public float BleedAmount
	{
		get
		{
			return bleedAmount;
		}

		set
		{
			bleedAmount = value;
		}
	}
	public float BleedDuration
	{
		get
		{
			return bleedDuration;
		}

		set
		{
			bleedDuration = value;
		}
	}
	public bool IsBlocking
	{
		get
		{
			return isBlocking;
		}
		set
		{
			isBlocking = value;
		}
	}

	public Dictionary<ElementalResistances, float> ElementalResists
	{
		get
		{
			return elementalResists;
		}
	}
	#endregion

	public virtual void TakeDamage(float damageAmount, ElementalResistances type)
	{
		float unmitigated = damageAmount;

		//Mitigation Calculations
		float resistValue = 0;
		if (type != ElementalResistances.TRUE)
		{
			resistValue = elementalResists[type];
		}

		if (resistValue == 0)
		{
			resistValue++;
		}
		resistValue *= resistanceMultiplier;

		if (type == ElementalResistances.PHYSICAL)
		{
			int mitigated = Mathf.RoundToInt(damageAmount / (Mathf.Log10(resistValue + 5)) + 1);
			if (mitigated < 0)
			{
				mitigated = 0;
			}

			if (mitigated > shieldAmount)
			{
				mitigated -= (int)shieldAmount;
				shieldAmount = 0;
			}
			else
			{
				shieldAmount -= mitigated;
				mitigated = 0;
			}
			CurrentHealth -= mitigated;
			GameObject textRef = Instantiate(damageTextPrefab, transform.position + (Vector3)(Random.insideUnitCircle * Random.Range(0f, 1f)), Quaternion.identity);
			textRef.GetComponentInChildren<Text>().text = mitigated.ToString();
			textRef.GetComponentInChildren<Text>().color = new Color(.25f, .25f, 0f);
			Destroy(textRef, 1f);

			foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>())
			{
				spr.color = Color.red;
			}
			damageColorTimer = .25f;
		}
		else if (type == ElementalResistances.TRUE)
		{
			if (damageAmount > shieldAmount)
			{
				damageAmount -= (int)shieldAmount;
				shieldAmount = 0;
			}
			else
			{
				shieldAmount -= damageAmount;
				damageAmount = 0;
			}
			CurrentHealth -= damageAmount;
			GameObject textRef = Instantiate(damageTextPrefab, transform.position + (Vector3)(Random.insideUnitCircle * Random.Range(0f, 3f)), Quaternion.identity);
			textRef.GetComponentInChildren<Text>().text = damageAmount.ToString();
			Destroy(textRef, 1f);
			//foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>())
			//{
			//	spr.color = Color.red;
			//}
			//damageColorTimer = .25f;
		}
		else
		{
			int mitigated = Mathf.RoundToInt(damageAmount / (Mathf.Pow(resistValue / 10, 0.5f)));
			if (mitigated < 0)
			{
				mitigated = 0;
			}

			if (mitigated > shieldAmount)
			{
				mitigated -= (int)shieldAmount;
				shieldAmount = 0;
			}
			else
			{
				shieldAmount -= mitigated;
				mitigated = 0;
			}
			CurrentHealth -= mitigated;

			GameObject textRef = Instantiate(damageTextPrefab, transform.position + (Vector3)(Random.insideUnitCircle * Random.Range(0f, 3f)), Quaternion.identity);
			textRef.GetComponentInChildren<Text>().text = mitigated.ToString();
			switch (type)
			{
				case (ElementalResistances.FIRE):
					textRef.GetComponentInChildren<Text>().color = Color.red;
					break;
				case (ElementalResistances.ICE):
					textRef.GetComponentInChildren<Text>().color = Color.cyan;
					break;
				case (ElementalResistances.POISON):
					textRef.GetComponentInChildren<Text>().color = new Color(0f, .5f, 0f);
					break;
				case (ElementalResistances.SHOCK):
					textRef.GetComponentInChildren<Text>().color = Color.yellow;
					break;
				case (ElementalResistances.VOID):
					textRef.GetComponentInChildren<Text>().color = new Color(.3f, .2f, .6f);
					break;
			}
			Destroy(textRef, 1f);

			//foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>())
			//{
			//	if (!spriteRenderColorReferences.ContainsKey(spr.transform)) { 
			//		spriteRenderColorReferences.Add(spr.transform, spr.color);
			//	} else if ((spriteRenderColorReferences.ContainsKey(spr.transform) && spr.color != spriteRenderColorReferences[spr.transform])&& spr.color != Color.red)
			//	{
			//		spriteRenderColorReferences[spr.transform] = spr.color;
			//	}
			//	spr.color = Color.red;
			//}
			//damageColorTimer = .25f;
		}
	}
	protected virtual void Start()
	{
		elementalResists = new Dictionary<ElementalResistances, float>
		{
			{ ElementalResistances.PHYSICAL, 12 },
			{ ElementalResistances.FIRE, 12 },
			{ ElementalResistances.ICE, 12 },
			{ ElementalResistances.SHOCK, 12 },
			{ ElementalResistances.EXPLOSIVE, 12},
			{ ElementalResistances.POISON, 12 },
			{ ElementalResistances.VOID, 12 },
			{ ElementalResistances.ARCANE, 12 },
			{ ElementalResistances.SPECTRAL, 12},
		};
		damageTextPrefab = Resources.Load<GameObject>("InterfaceElements/DamageFlareText");
	}
	protected virtual void Update()
	{
		if (Player_Accessor_Script.EquipmentScript != null && Vector2.Distance(transform.position, Player_Accessor_Script.EquipmentScript.transform.position) > 30)
		{
			return;
		}

		if (shieldAmount > 0)
		{
			shieldAmount -= Time.deltaTime * .125f * shieldAmount;
			if (shieldAmount < 0)
			{
				shieldAmount = 0;
			}
		}
		Regenerate();

		//if (damageColorTimer > 0)
		//{
		//	damageColorTimer -= Time.deltaTime;
		//	if (damageColorTimer < 0)
		//	{
		//		damageColorTimer = 0;
		//		foreach (SpriteRenderer spr in GetComponentsInChildren<SpriteRenderer>())
		//		{
		//			spr.color = spriteRenderColorReferences[spr.transform];
		//		}
		//	}
		//}
	}
	public void AddShield(float shieldAmount)
	{
		this.shieldAmount = shieldAmount * healMultiplier;
		if (shieldAmount < 0)
		{
			shieldAmount = 0;
		}
	}
	public virtual void HealSelf(float healingAmount)
	{
		currentHealth += healingAmount * healMultiplier;
		if (currentHealth > maxHealth)
		{
			currentHealth = maxHealth;
		}
	}

	public virtual void Knockback(Vector3 direction, float magnitude)
	{
		GetComponent<Rigidbody2D>().velocity = direction * magnitude;
	}

	protected virtual void Regenerate()
	{
		if (currentHealth < maxHealth || regenRate < 0)
		{
			HealSelf(regenRate * Time.deltaTime * .2f);
		}
	}
}
