using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PatrolMode
{
    moveLeft,
    moveRight,
    moveUp,
    moveDown,
    Standing
}

public class MonsterController : CharacterManager
{
    [SerializeField] int targetRange; //적 감지 범위

    List<GameObject> PlayerCharacters;
    float shortDistance;
    GameObject Target;

    Vector3 targetDir;

    Animator anim;

    bool isPatrol;
    bool isDelay;
    bool isDead;
    bool isReward;
    float attackDelayTime;
    float spwanDelayTime;

    PatrolMode patrolMode;
    float patrolDelay = 5;

    float chaseRange = 10;

    [HideInInspector] public float NowHp;
    [HideInInspector] public int gold = 10;

    private void Awake()
    {
        PlayerCharacters = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));
        shortDistance = 15f;//Vector3.Distance(transform.parent.position, PlayerCharacters[0].transform.position);

        anim = GetComponent<Animator>();

        GameManager.GetInstance.GetHpInfo().AddMonsterHPBar(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        NowHp = MaxHp;
        var randPatrol = System.Enum.GetValues(typeof(PatrolMode));
        patrolMode = (PatrolMode)randPatrol.GetValue(Random.Range(0, randPatrol.Length));
        FoundEnemy();
        if(!isPatrol)
            targetDir = Target.transform.position - transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            if(!isReward)
            {
                GameManager.GetInstance.Reward(this.gameObject);
                isReward = true;
            }
            anim.SetBool("isDead", true);
            return;
        }

        
        if (Target == null)
        {
            FoundEnemy();

        }

        if(isPatrol)
        {
            if(patrolDelay > 3)
            {
                Debug.Log("PatrolMode");
                anim.SetBool("FindEnemy", true);
                patrolDelay = 0;

                if (patrolMode == PatrolMode.Standing)
                    patrolMode = PatrolMode.moveLeft;
                else if (patrolMode == PatrolMode.moveLeft)
                    patrolMode = PatrolMode.moveDown;
                else if (patrolMode == PatrolMode.moveDown)
                    patrolMode = PatrolMode.moveRight;
                else if (patrolMode == PatrolMode.moveRight)
                    patrolMode = PatrolMode.moveUp;
                else if (patrolMode == PatrolMode.moveUp)
                    patrolMode = PatrolMode.Standing;
            }

            switch (patrolMode)
            {
                case PatrolMode.moveLeft:
                    transform.parent.Translate(Vector3.left * 1 * Time.deltaTime);
                    transform.parent.localScale = new Vector3(-1, 1, 1);
                    break;
                case PatrolMode.moveRight:
                    transform.parent.Translate(Vector3.right * 1 * Time.deltaTime);
                    transform.parent.localScale = Vector3.one;
                    break;
                case PatrolMode.moveUp:
                    transform.parent.Translate(Vector3.up * 1 * Time.deltaTime);
                    break;
                case PatrolMode.moveDown:
                    transform.parent.Translate(Vector3.down * 1 * Time.deltaTime);
                    break;
                case PatrolMode.Standing:
                    anim.SetBool("FindEnemy", false);
                    break;
            }

            //if (patrolRand % 2 == 1)
            //{
            //    transform.parent.Translate(Vector3.left * 1 * Time.deltaTime);
            //    transform.parent.localScale = new Vector3(-1, 1, 1);
            //}
            //else
            //{
            //    transform.parent.Translate(Vector3.right * 1 * Time.deltaTime);
            //    transform.parent.localScale = Vector3.one;
            //}

            patrolDelay += Time.deltaTime;
        }

        if (isDelay)
            attackDelayTime += Time.deltaTime;
        else if (!isDelay)
        {
            if (Target != null && targetDir.sqrMagnitude > AttackRange)
            {
                targetDir = Target.transform.position - transform.parent.position;

                if(targetDir.sqrMagnitude > Mathf.Pow(chaseRange,2))
                {
                    isPatrol = true;
                    //anim.SetBool("FindEnemy", false);
                    return;
                }

                transform.parent.Translate(targetDir.normalized * 1 * Time.deltaTime);
                isPatrol = false;

                anim.SetBool("FindEnemy", true);
            }

            if (Target != null && AttackRange >= targetDir.sqrMagnitude)
            {
                isPatrol = false;
                patrolDelay = 0;
                anim.SetBool("AttackEnemy", true);
            }
        }

        if (attackDelayTime >= AttackDelay)
            isDelay = false;

    }

    void FoundEnemy()
    {
        Target = null;

        foreach(GameObject targetObj in  PlayerCharacters)
        {
            if (targetObj.GetComponent<CharacterController>().GetCharacterDead())
                continue;

            float findDistance = Vector3.Distance(transform.parent.position, targetObj.transform.position);

            if(findDistance <= shortDistance + 0.1f)
            {
                Target = targetObj;
                shortDistance = findDistance;
            }
        }

        if(Target == null)
        {
            isPatrol = true;
            return;
        }

        targetDir = Target.transform.position - transform.parent.position;
        isPatrol = false;

        //if (Target.transform.position.x > transform.parent.position.x)
        //    transform.parent.localScale = new Vector3(transform.parent.localScale.x * -1, 1, 1);

        if (targetDir.x < 0)
            transform.parent.localScale = new Vector3(-1, 1, 1);
        else if (targetDir.x >= 0)
            transform.parent.localScale = Vector3.one;


        Debug.Log(Target.name);
    }

    public void AttackPlayer()
    {
        Target.GetComponent<CharacterController>().SetDamange(AttackDamage);
    }

    public void AttackEnd()
    {
        isDelay = true;
        attackDelayTime = 0;
        anim.SetBool("AttackEnemy", false);
        anim.SetBool("FindEnemy", false);
        anim.StopPlayback();

        if (Target.GetComponent<CharacterController>().GetCharacterDead())
        {
            Target = null;
            anim.SetBool("AttackEnemy", false);
            anim.SetBool("FindEnemy", false);
            anim.StopPlayback();
        }
    }

    public void SetDamange(float Dmg)
    {
        NowHp -= Dmg;

        if (NowHp <= 0)
            isDead = true;
    }

    public void DeleteMonster()
    {
        GameManager.GetInstance.GetHpInfo().DeadMonster(this.gameObject);
        int idx = GameManager.GetInstance._list_Monsters.FindIndex(x => x.gameObject.transform == this.transform.parent);
        GameManager.GetInstance._list_Monsters.RemoveAt(idx);
        Destroy(this.transform.parent.gameObject);
    }

    public bool GetCharacterDead() => isDead;

    public void SetStun()
    {
        isDelay = true;
        attackDelayTime = 0;
        anim.SetBool("AttackEnemy", false);
        anim.SetBool("FindEnemy", false);
        anim.StopPlayback();
    }
}
