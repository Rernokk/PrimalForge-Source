using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum AttributeType { ATTACKPOWER, DEFENSEPOWER, ATTACKMULT, DEFENSEMULT };

public class EnemyObject : AttackableEntity
{
  [SerializeField] protected Material healthMaterial;
  [SerializeField] protected int mobLevel;

  protected bool isFlaggedDead = false;
  protected Rigidbody2D rgd2d;

  //Attributes
  [Header("Resistances")]
  [SerializeField] float physicalResist;
  [SerializeField] float fireResist;
  [SerializeField] float iceResist;
  [SerializeField] float poisonResist;
  [SerializeField] float chaosResist;
  [SerializeField] float shockResist;

  [Header("Character General Attributes")]
  [SerializeField]
  protected float ActiveRange = 10f;
  [SerializeField] protected float FalloffRange = 20f;

  //Combat Attributes
  [SerializeField] protected float AttackPower = 1f;
  [SerializeField] protected float DefensePower = 1f;
  [SerializeField] protected float AttackMultiplier = 1.0f;
  [SerializeField] protected float DefenseMultiplier = 1.0f;
	protected float bleedTick = 0f;

  //Attacks Per Second
  [SerializeField] protected float AttackSpeed = 1f;

  [SerializeField]
  protected float CharacterSpeed;

  [SerializeField]
  protected bool isAggressive, isChasing;

  //Current Target
  protected AttackableEntity targetEntity;

  //Pathfinding Variables
  protected List<Node> currentPath;
  protected GameObject player;
  protected bool wanderIsPaused = false;
  protected AIManager graphManager;

  //Internal Attack Timer
  protected float attackTimer = 0f;

  //Insantiy
  [SerializeField]
  protected bool isInsane;
  protected float insanityTimer;

  //Links
  HashSet<EnemyObject> replicatedTargets;

	protected Slider HealthBar;

  #region Properties
  public AttackableEntity TargetEntity
  {
    get
    {
      return targetEntity;
    }
    set
    {
      targetEntity = value;
    }
  }
  public HashSet<EnemyObject> LinkedTargets
  {
    get
    {
      return replicatedTargets;
    }

    set
    {
      replicatedTargets = value;
    }
  }
  public bool IsInsane
  {
    get
    {
      return isInsane;
    }

    set
    {
      isInsane = value;
    }
  }
  #endregion

  protected void Awake()
  {
    currentHealth = maxHealth;
    //healthMaterial = new Material(healthMaterial);
    healthMaterial = new Material(Resources.Load<Material>("Materials/Enemy_Health_Bar_Base"));
		//transform.Find("Health_Bar/HealthBar").GetComponent<Image>().material = healthMaterial;
		Transform healthBarRef = transform.Find("Health_Bar/Slider");
		if (healthBarRef != null)
		{
			HealthBar = healthBarRef.GetComponent<Slider>();
		}
    transform.tag = "Enemy";
    currentPath = new List<Node>();

    elementalResists = new Dictionary<ElementalResistances, float>();
    elementalResists.Add(ElementalResistances.PHYSICAL, physicalResist);
    elementalResists.Add(ElementalResistances.FIRE, fireResist);
    elementalResists.Add(ElementalResistances.ICE, iceResist);
    elementalResists.Add(ElementalResistances.POISON, poisonResist);
    elementalResists.Add(ElementalResistances.VOID, chaosResist);
    elementalResists.Add(ElementalResistances.SHOCK, shockResist);
    rgd2d = gameObject.GetComponent<Rigidbody2D>();
		GameObject sceneManagerRefTemp = GameObject.FindGameObjectWithTag("SceneManager");
		if (sceneManagerRefTemp != null)
		{
			graphManager = sceneManagerRefTemp.GetComponent<AIManager>();
		}
    replicatedTargets = new HashSet<EnemyObject>();
  }

  private void UpdateHealthBar()
  {
		//healthMaterial.SetFloat("_Value", HealthPercent);
		HealthBar.value = HealthPercent;
  }

  private void Die()
  {
    foreach (EnemyObject tagged in replicatedTargets)
    {
      tagged.replicatedTargets.Remove(this);
    }
    isFlaggedDead = true;
    ActionOnDeath();
  }
  protected virtual void LateUpdate()
  {
    if (isFlaggedDead)
    {
			for (int i = 0; i < 4; i++)
			{
				if (Player_Accessor_Script.EquipmentScript.FetchItemSlot(i) != null && Player_Accessor_Script.EquipmentScript.FetchItemSlot(i).SkillSet[0] != null)
				{
					Player_Accessor_Script.SkillsScript.AddExperience(Player_Accessor_Script.EquipmentScript.FetchItemSlot(i).SkillSet[0].SkillRequired, Mathf.RoundToInt(CalculateExpWorth() * 1f));
				}
			}
      Destroy(gameObject);
    }
  }
  float CalculateExpWorth()
  {
		return 100 * Mathf.Pow(1.02f, mobLevel - 1);
  }
  protected IEnumerator WanderPathfinding()
  {
    if (currentPath == null)
    {
      currentPath = new List<Node>();
    }
    wanderIsPaused = true;
    yield return new WaitForSeconds(Random.Range(.2f, 3f));
    wanderIsPaused = false;
    int val = 0;
    Vector3 tarPos = transform.position + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * Random.Range(3f, 6f));
    bool notHitWall = true;
    RaycastHit2D[] info = Physics2D.RaycastAll(transform.position, tarPos - transform.position, (tarPos - transform.position).magnitude);
    //Debug.DrawRay(transform.position, tarPos - transform.position, Color.red, .25f);
    foreach (RaycastHit2D hit in info)
    {
      if (hit.transform.tag == "Wall")
      {
        notHitWall = false;
      }
    }

    if (notHitWall)
    {
      currentPath = graphManager.Dijkstra(transform.position, tarPos);
      if (currentPath.Count > 0)
      {
        currentPath.RemoveAt(0);
      }
    }
  }
  protected override void Start()
  {
    base.Start();
    player = GameObject.FindGameObjectWithTag("Player");
    elementalResists[ElementalResistances.PHYSICAL] = physicalResist;
    elementalResists[ElementalResistances.VOID] = chaosResist;
    elementalResists[ElementalResistances.SHOCK] = shockResist;
    elementalResists[ElementalResistances.POISON] = poisonResist;
    elementalResists[ElementalResistances.FIRE] = fireResist;
    elementalResists[ElementalResistances.ICE] = iceResist;
  }
  protected override void Update()
  {
    base.Update();
    UpdateHealthBar();
    if (isInsane){
      insanityTimer -= Time.deltaTime;
      if (insanityTimer < 0){
        insanityTimer = 0;
        isInsane = false;
      }
    }
    foreach (EnemyObject obj in replicatedTargets)
    {
      Debug.DrawLine(transform.position, obj.transform.position, Color.yellow, .1f);
    }
    if (attackTimer > 0)
    {
      attackTimer -= Time.deltaTime;
      if (attackTimer < 0)
      {
        attackTimer = 0;
      }
    }
  }
  public void SetPath(List<Node> path)
  {
    currentPath = path;
  }
  protected bool CheckIfNotStunned()
  {
    if (isStunned)
    {
      stunDuration -= Time.deltaTime;
      if (stunDuration <= 0)
      {
        IsStunned = false;
      }
      return false;
    }
    return true;
  }
  protected void CheckForBleed()
  {
    if (isBleeding)
    {
			if (bleedTick <= 0)
			{
				TakeDamage(bleedAmount, ElementalResistances.POISON);
				bleedTick += 1f;
			}
      bleedDuration -= Time.deltaTime;
			bleedTick -= Time.deltaTime;
      if (bleedDuration < 0)
      {
        isBleeding = false;
        bleedDuration = 0;
        bleedAmount = 0;
      }
    }
  }
  protected void UpdateWandering()
  {
    if (currentPath == null)
    {
      currentPath = new List<Node>();
    }
    if (currentPath.Count <= 0 && !wanderIsPaused)
    {
      StartCoroutine(WanderPathfinding());
    }
    else if (currentPath.Count > 0)
    {
      for (int i = 0; i < currentPath.Count - 1; i++)
      {
        //Debug.DrawLine(new Vector2(currentPath[i].x, currentPath[i].y), new Vector2(currentPath[i + 1].x, currentPath[i + 1].y), Color.red, 1f);
      }
      transform.position += (new Vector3(currentPath[0].x, currentPath[0].y) - transform.position).normalized * Time.deltaTime * CharacterSpeed;
      if (Vector3.Distance(transform.position, new Vector3(currentPath[0].x, currentPath[0].y)) < .5f)
      {
        currentPath.RemoveAt(0);
      }
    }
  }
  public void ApplyBleed(float damage, float duration)
  {
    if (isBleeding)
    {
      if (BleedAmount < damage)
      {
        BleedAmount = damage;
        bleedDuration = duration;
      }
      else
      {
        BleedAmount += damage * .25f;
        bleedDuration += duration * .25f;
      }
    }
    else
    {
      BleedAmount = damage;
      bleedDuration = duration;
      isBleeding = true;
    }
  }
  public override void TakeDamage(float damageAmount, ElementalResistances type)
  {
    base.TakeDamage(damageAmount, type);
		if (HealthBar.transform.parent.GetComponent<Animator>() != null)
		{
			HealthBar.transform.parent.GetComponent<Animator>().Play("DamageFlash");
		}
    UpdateHealthBar();
		foreach (EnemyObject obj in replicatedTargets)
    {
      obj.TakeReplicateDamage(damageAmount, type);
    }
    if (CurrentHealth <= 0)
    {
      Die();
    }
  }
  protected void TakeReplicateDamage(float damageAmount, ElementalResistances type)
  {
    base.TakeDamage(damageAmount * .5f, type);
    UpdateHealthBar();
    if (CurrentHealth <= 0)
    {
      Die();
    }
  }
  protected virtual void AttackTarget()
  {
    targetEntity.TakeDamage(AttackPower, ElementalResistances.PHYSICAL);
    attackTimer = 1.0f / AttackSpeed;
    if (targetEntity.CurrentHealth <= 0)
    {
      targetEntity = null;
    }
  }
  public virtual void DampenAttributes(float dur, float amnt, AttributeType type)
  {
    if (type == AttributeType.ATTACKMULT)
    {
      StartCoroutine(DampenAttackRoutine(dur, amnt));
    }
    else if (type == AttributeType.DEFENSEMULT)
    {
      StartCoroutine(DampenDefenseRoutine(dur, amnt));
    }
  }
  protected IEnumerator DampenAttackRoutine(float dur, float amnt)
  {
    AttackMultiplier -= amnt;
    yield return new WaitForSeconds(dur);
    AttackMultiplier += amnt;
  }
  protected IEnumerator DampenDefenseRoutine(float dur, float amnt)
  {
    DefenseMultiplier -= amnt;
    yield return new WaitForSeconds(dur);
    DefenseMultiplier += amnt;
  }
  public void SetInsane(){
    isInsane = true;
    insanityTimer = 8f;
  }
  protected virtual void ActionOnDeath(){
    SpawnCorpse(transform.position);
  }
  protected virtual void SpawnCorpse(Vector3 pos){
    GameObject obj = Instantiate(Resources.Load<GameObject>("Prefabs/Corpse"), pos, Quaternion.identity);
    Destroy(obj, 60f);
  }
}
