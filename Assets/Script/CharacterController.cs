using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Job
{
    Knight,
    Theif,
    Archer,
    Priest
}

public class CharacterController : CharacterManager
{
    [SerializeField] float SkillRange;
    [SerializeField] float SkillDelay;
    [SerializeField] Job _job;
    [SerializeField] float targetRange;

    [HideInInspector] public GameObject Target = null;

    List<GameObject> _list_Enemy = new List<GameObject>();

    List<GameObject> _list_ThiefTarget = new List<GameObject>();
    List<GameObject> _list_PriestTarget = new List<GameObject>();

    bool isDead;        //캐릭터 사망정보
    bool isDelay;

    bool isSkillOn;

    float RespawnTime;
    float attackDelayTime;
    
    float shortDistance;
    Vector3 targetDir;

    Animator anim;

    [HideInInspector] public float NowHp;
    [HideInInspector] public int level = 1;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();

        GameManager.GetInstance.GetLevelInfo().AddLevelBar(gameObject);

        NowHp = MaxHp;

        if(_job == Job.Priest)
            _list_PriestTarget = GameManager.GetInstance._Players.ToList();
        

        //targetRange = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            anim.SetBool("isDead", true);
            RespawnTime += Time.deltaTime;

            if (RespawnTime >= RespawnDelay)
            {
                isDead = false;
                anim.SetBool("isDead", false);
                RespawnTime = 0;
                attackDelayTime = 0;
                isDelay = false;
                isSkillOn = false;
                NowHp = MaxHp;
            }
        }
        else if (Target == null)
            FindTarget();

        if (isDelay)
            attackDelayTime += Time.deltaTime;
        else if (!isDelay)
        {
            if (Target != null && targetDir.sqrMagnitude > AttackRange)
            {
                anim.SetBool("isFind", true);

                targetDir = Target.transform.position - transform.position;
                transform.Translate(targetDir.normalized * 1 * Time.deltaTime);
            }

            if (Target != null && AttackRange >= targetDir.sqrMagnitude)
            {
                anim.SetBool("isAttack", true);
            }
        }

        if (isSkillOn)
        {
            if (attackDelayTime >= SkillDelay)
                isDelay = false;
        }
        else
        {
            if (attackDelayTime >= AttackDelay)
                isDelay = false;
        }
    }

    public bool GetCharacterDead() => isDead;

    public void SetDamange(float Dmg)
    {
        NowHp -= Dmg;

        if (NowHp <= 0)
            isDead = true;
        else if (NowHp >= MaxHp)
            NowHp = MaxHp;
    }

    void FindTarget()
    {
        if (GameManager.GetInstance._list_Monsters.Count <= 0)
            return;
        else
            _list_Enemy = GameManager.GetInstance._list_Monsters;

        if (Target == null)
        {
            foreach (GameObject targetObj in _list_Enemy)
            {
                float findDistance = Vector3.Distance(transform.position, targetObj.transform.position);

                if (findDistance <= shortDistance + targetRange)
                {
                    Target = targetObj.transform.GetChild(0).gameObject;
                    shortDistance = findDistance;
                }
            }

            if (Target == null)
                return;

            targetDir = Target.transform.parent.position - transform.position;

            if (targetDir.x >= 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (targetDir.x < 0)
                transform.localScale = Vector3.one;
        }
    }


    public void AttackEnemy()
    {
        if(isSkillOn)
        {
            switch(_job)
            {
                case Job.Knight:
                    Target.GetComponent<MonsterController>().SetDamange(AttackDamage);
                    if (!Target.GetComponent<MonsterController>().GetCharacterDead())
                        Target.GetComponent<MonsterController>().SetStun();
                    break;
                case Job.Theif:
                    ThiefSkillTarget();
                    for(int i = 0; i < _list_ThiefTarget.Count; i++)
                        _list_ThiefTarget[i].GetComponent<MonsterController>().SetDamange(AttackDamage);
                    _list_ThiefTarget.Clear();
                    break;
                case Job.Archer:
                    Target.GetComponent<MonsterController>().SetDamange(AttackDamage * 2.5f);
                    break;
                case Job.Priest:
                    anim.SetBool("isSkill", true);
                    break;
            }

            isSkillOn = false;
        }
        else
        {
            Target.GetComponent<MonsterController>().SetDamange(AttackDamage);
            isSkillOn = true;
        }
    }

    public void AttackEnd()
    {
        isDelay = true;
        attackDelayTime = 0;
        anim.SetBool("isAttack", false);
        anim.SetBool("isFind", false);
        if(_job == Job.Priest)
            anim.SetBool("isSkill", false);
        anim.StopPlayback();

        if (Target.GetComponent<MonsterController>().GetCharacterDead())
        {
            Target = null;
            anim.SetBool("isAttack", false);
            anim.SetBool("isFind", false);
            anim.StopPlayback();
        }
    }


    void ThiefSkillTarget()
    {
        foreach (GameObject targetObj in _list_Enemy)
        {
            float findDistance = Vector3.Distance(transform.position, targetObj.transform.position);

            if (findDistance <= SkillRange)
                _list_ThiefTarget.Add(targetObj.transform.GetChild(0).gameObject);
        }
    }

    void PriestSkillTarget()
    {
        GameObject temp = _list_PriestTarget[3];

        for(int i = 2; i >= 0; i--)
        {
            if(_list_PriestTarget[i].GetComponent<CharacterController>().isDead == false && 
                temp.GetComponent<CharacterController>().NowHp > _list_PriestTarget[i].GetComponent<CharacterController>().NowHp)
                temp = _list_PriestTarget[i];
        }

        temp.GetComponent<CharacterController>().SetDamange(AttackDamage * 2.5f * -1);
    }
}
