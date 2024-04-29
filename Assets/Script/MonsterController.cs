using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : CharacterManager
{
    [SerializeField] int targetRange; //적 감지 범위

    List<GameObject> PlayerCharacters;
    float shortDistance;
    GameObject Target;

    Vector3 targetDir;

    Animator anim;

    bool GameOver;
    bool isDelay;
    bool isDead;
    float attackDelayTime;
    float spwanDelayTime;

    float chaseRange = 10;

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
        FoundEnemy();
        targetDir = Target.transform.position - transform.parent.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            anim.SetBool("isDead", true);
            return;
        }

        if (GameOver)
        {
            anim.SetBool("AttackEnemy", false);
            anim.SetBool("FindEnemy", false);
        }
        else if (Target == null)
            FoundEnemy();

        if (isDelay)
            attackDelayTime += Time.deltaTime;
        else if (!isDelay)
        {
            if (Target != null && targetDir.sqrMagnitude > AttackRange)
            {
                targetDir = Target.transform.position - transform.parent.position;

                if(targetDir.sqrMagnitude > Mathf.Pow(chaseRange,2))
                {
                    Target = null;
                    anim.SetBool("FindEnemy", false);
                    return;
                }

                transform.parent.Translate(targetDir.normalized * 1 * Time.deltaTime);

                anim.SetBool("FindEnemy", true);
            }

            if (Target != null && AttackRange >= targetDir.sqrMagnitude)
            {
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
            Debug.Log("GameOver");
            return;
        }

        targetDir = Target.transform.position - transform.parent.position;

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
        MaxHp -= Dmg;

        if (MaxHp <= 0)
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
