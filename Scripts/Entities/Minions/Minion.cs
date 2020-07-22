using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minion : AttackableEntity
{
  #region Internal Variables
  [SerializeField] protected Material healthMaterial;

  protected bool isFlaggedDead = false;
  protected Rigidbody2D rgd2d;

  //Current Target
  [SerializeField] protected AttackableEntity targetEntity;

  //Pathfinding Variables
  [SerializeField] protected List<Node> currentPath;
  protected GameObject player;
  protected bool wanderIsPaused = false;
  protected AIManager graphManager;
  [SerializeField] protected bool inCombat = false;
  protected bool findPath = true;
  protected float distanceLimiter = 3f;
  protected float checkTimer = 0f;

  //Internal Attack Timer
  protected float attackTimer = 0f;
  #endregion

  #region Variables
  //Attributes
  [Header("Resistances")]
  [SerializeField]
  float physicalResist;
  [SerializeField] float fireResist;
  [SerializeField] float iceResist;
  [SerializeField] float poisonResist;
  [SerializeField] float chaosResist;
  [SerializeField] float shockResist;

  [Header("Combat Attributes")]
  //Combat Attributes
  [SerializeField]
  protected float attackPower = 1f;
  [SerializeField] protected float defensePower = 1f;
  [SerializeField] protected float attackMultiplier = 1.0f;
  [SerializeField] protected float defenseMultiplier = 1.0f;


  [Header("Misc. Attributes")]
  //Movement Speed
  [SerializeField]
  protected float CharacterSpeed;

  //Dark Arcane Magic Buffs
  [Header("Dark Arcane Buff")]
  public bool empowered = false;

  public float baseAttackRange = 1f;
  public float empoweredAttackRange = 4f;
	protected float currentAttackRange = 1f;

	//Attacks Per Second
	public float baseAttackSpeed = 1f;
  public float empoweredAttackSpeed = 1.25f;
  protected float currentAttackSpeed = 1f;

	public float empoweredRegenRate;

  [HideInInspector]
  public float baseRegenRate;

  #endregion

  #region Properties
  public float AttackSpeed
  {
    get
    {
      return currentAttackSpeed;
    }

    set
    {
      currentAttackSpeed = value;
    }
  }
  public float DefenseMultiplier
  {
    get
    {
      return defenseMultiplier;
    }

    set
    {
      defenseMultiplier = value;
    }
  }
  public float AttackMultiplier
  {
    get
    {
      return attackMultiplier;
    }

    set
    {
      attackMultiplier = value;
    }
  }
  public float DefensePower
  {
    get
    {
      return defensePower;
    }

    set
    {
      defensePower = value;
    }
  }
  public float AttackPower
  {
    get
    {
      return attackPower;
    }

    set
    {
      attackPower = value;
    }
  }
  #endregion

  #region Methods
  protected void Awake()
  {
    currentHealth = maxHealth;
    healthMaterial = new Material(healthMaterial);
    transform.Find("Minion_Health_Bar/HealthBar").GetComponent<Image>().material = healthMaterial;
    transform.tag = "Minion";
    currentPath = new List<Node>();

    elementalResists = new Dictionary<ElementalResistances, float>();
    elementalResists.Add(ElementalResistances.PHYSICAL, physicalResist);
    elementalResists.Add(ElementalResistances.FIRE, fireResist);
    elementalResists.Add(ElementalResistances.ICE, iceResist);
    elementalResists.Add(ElementalResistances.POISON, poisonResist);
    elementalResists.Add(ElementalResistances.VOID, chaosResist);
    elementalResists.Add(ElementalResistances.SHOCK, shockResist);
    rgd2d = gameObject.GetComponent<Rigidbody2D>();
    graphManager = GameObject.Find("GameManager").GetComponent<AIManager>();

    currentAttackRange = baseAttackRange;
    currentAttackSpeed = baseAttackSpeed;
    baseRegenRate = regenRate;
  }
  protected override void Start()
  {
    base.Start();
    distanceLimiter = Random.Range(2f, 2.5f);
    checkTimer = Random.Range(0f, 3f);
  }
  protected override void Update()
  {
    base.Update();
    UpdateHealthBar();
    if(currentHealth <= 0){
      Die();
    }

    if (!empowered && DarkArcaneMagic.isActive)
    {
      print("Empowering " + transform.name);
      empowered = true;
      Empower();
    }
    else if (empowered && !DarkArcaneMagic.isActive)
    {
      print("DeEmpowering " + transform.name);
      empowered = false;
      DeEmpower();
    }
    if (targetEntity != null)
    {
      Debug.DrawLine(transform.position, targetEntity.transform.position, Color.green);
    }

    if (attackTimer > 0)
    {
      attackTimer -= Time.deltaTime;
      if (attackTimer < 0)
      {
        attackTimer = 0;
      }
    }
    if (targetEntity == null && !inCombat)
    {
      targetEntity = Player_Accessor_Script.DetailsScript;
    }

    //Not moving, move out.
    if (rgd2d.velocity.magnitude == 0)
    {
      //Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, .25f, 2048);
      //for (int i = 0; i < nearby.Length; i++)
      //{
      //  if (nearby[i].GetComponent<Rigidbody2D>().velocity.magnitude == 0)
      //  {
      //    nearby[i].transform.position += (nearby[i].transform.position - transform.position).normalized * .015f;
      //    transform.position += (transform.position - nearby[i].transform.position).normalized * .015f;
      //  }
      //}
    }

    if (targetEntity == null)
    {
      rgd2d.velocity = Vector2.zero;
      targetEntity = Player_Accessor_Script.DetailsScript;
      if (inCombat)
      {
        inCombat = !inCombat;
      }
    }
    if (checkTimer < 5f)
    {
      checkTimer += Time.deltaTime;
    }

    if (checkTimer > 5f && targetEntity == null || targetEntity == Player_Accessor_Script.DetailsScript)
    {
      checkTimer = 0;
      CheckForNearbyTargets();
    }

    if (inCombat)
    {
      if (targetEntity != null && CanSeeTarget())
      {
        if (Vector2.Distance(transform.position, targetEntity.transform.position) > currentAttackRange)
        {
          rgd2d.velocity = (targetEntity.transform.position - transform.position).normalized * CharacterSpeed;
        }
        else
        {
          rgd2d.velocity = Vector2.zero;
          if (attackTimer <= 0)
          {
            AttackTarget();
          }
        }
      }
    }
    else
    {
      //Pathing towards player
      if (CanSeePlayer() && Vector2.Distance(transform.position, Player_Accessor_Script.DetailsScript.transform.position) < 4)
      {
        if (Vector2.Distance(transform.position, Player_Accessor_Script.DetailsScript.transform.position) > distanceLimiter)
        {
          rgd2d.velocity = (Player_Accessor_Script.DetailsScript.transform.position - transform.position).normalized * Player_Accessor_Script.ControllerScript.PlayerSpeedMultiplier * Player_Accessor_Script.ControllerScript.PlayerSpeed;
          currentPath.Clear();
        }
        else
        {
          rgd2d.velocity = Vector2.zero;
          currentPath.Clear();
        }
      }
      else
      {
				if (currentPath == null)
				{
					currentPath = new List<Node>();
				}

				if (graphManager == null)
				{
					graphManager = AIManager.instance;
				}
				if (currentPath.Count == 0)
				{
					currentPath = graphManager.Dijkstra(transform.position, Player_Accessor_Script.DetailsScript.transform.position);
				}
				else
				{
					Vector3 currTar = new Vector3(currentPath[0].x, currentPath[0].y);
					rgd2d.velocity = (currTar - transform.position).normalized * CharacterSpeed;
					if (Vector3.Distance(transform.position, currTar) < .5f)
					{
						currentPath.RemoveAt(0);
					}
				}
      }
    }
  }
  private void LateUpdate()
  {
    if (isFlaggedDead)
    {
      Destroy(gameObject);
    }
  }
  protected virtual void Die()
  {
    isFlaggedDead = true;
  }
  private void UpdateHealthBar()
  {
    healthMaterial.SetFloat("_Value", HealthPercent);
  }
  public override void TakeDamage(float damageAmount, ElementalResistances type)
  {
    base.TakeDamage(damageAmount, type);
    UpdateHealthBar();
    if (CurrentHealth <= 0)
    {
      Die();
    }
  }
  public void SacrificeMinion()
  {
    Die();
  }
  protected bool CanSeePlayer()
  {
    if (Physics2D.RaycastAll(transform.position, (Player_Accessor_Script.DetailsScript.transform.position - transform.position).normalized, Vector3.Distance(transform.position, Player_Accessor_Script.DetailsScript.transform.position), LayerMask.GetMask("Wall")).Length == 0)
    {
      return true;
    }
    return false;
  }
  protected bool CanSeeTarget()
  {
    if (Physics2D.RaycastAll(transform.position, (targetEntity.transform.position - transform.position).normalized, Vector3.Distance(transform.position, targetEntity.transform.position), 8192).Length == 0)
    {
      return true;
    }
    return false;
  }
  protected void CheckForNearbyTargets()
  {
    Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 12f, 256);
    if (colls.Length > 0)
    {
      List<Collider2D> tempColList = new List<Collider2D>();
      for (int i = 0; i < colls.Length; i++)
      {
        tempColList.Add(colls[i]);
      }
      //tempColList.Sort((q1, q2) => Vector3.Distance(q1.transform.position, transform.position).CompareTo(Vector3.Distance(q2.transform.position, transform.position)));
      for (int i = 0; i < tempColList.Count; i++)
      {
        if (Physics2D.RaycastAll(transform.position, tempColList[i].transform.position - transform.position, Vector2.Distance(transform.position, tempColList[i].transform.position), 8192).Length == 0)
        {
          targetEntity = tempColList[i].GetComponent<AttackableEntity>();
          break;
        }
      }
      if (targetEntity != null)
      {
        inCombat = true;
      }
    }
  }
  protected virtual void AttackTarget()
  {
    print("Attacking!");
    targetEntity.TakeDamage(AttackPower, ElementalResistances.PHYSICAL);
    attackTimer = 1.0f / AttackSpeed;
    if (targetEntity.CurrentHealth <= 0)
    {
      targetEntity = null;
      inCombat = false;
    }
  }
  protected virtual void Empower()
  {

  }
  protected virtual void DeEmpower()
  {

  }
  #endregion
}
