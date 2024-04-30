using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>직업을 정하는 Enum</summary>
public enum Job
{
    Knight,
    Theif,
    Archer,
    Priest
}

public class CharacterController : CharacterManager
{
    #region Values

    [SerializeField] float SkillRange;      //스킬 사거리
    [SerializeField] float SkillDelay;      //스킬 재사용 시간
    [SerializeField] Job _job;              //직업
    [SerializeField] float targetRange;     //추적 사거리(플레이어의 경우 추적 사거리를 자유롭게 변경 가능)

    [HideInInspector] public GameObject Target = null;                  //적 오브젝트

    List<GameObject> _list_Enemy = new List<GameObject>();              //적들의 정보를 담아두는 리스트
    List<GameObject> _list_ThiefTarget = new List<GameObject>();        //도적의 스킬을 위한 리스트
    List<GameObject> _list_PriestTarget = new List<GameObject>();       //성직자의 스킬을 위한 리스트

    bool isDead;                    //캐릭터 사망정보를 확인하는 bool
    bool isDelay;                   //공격의 딜레이를 확인하는 bool
    bool isSkillOn;                 //스킬을 사용할 차례인지 확인하는 bool

    float RespawnTime;              //부활 대기시간
    float attackDelayTime;          //공격 대기시간
    
    float shortDistance;            //적과의 가까운 거리
    Vector3 targetDir;              //타겟의 방향

    Animator anim;

    [HideInInspector] public float NowHp;       
    [HideInInspector] public int level = 1;

    #endregion

    #region MonoBehaviour

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        GameManager.GetInstance.GetLevelInfo().AddLevelBar(gameObject);     //경험치바 추가

        NowHp = MaxHp;
        //성직자 스킬을 위한 리스트 생성
        if(_job == Job.Priest)
            _list_PriestTarget = GameManager.GetInstance._Players.ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.GetInstance.GetStageClear())
        {
            anim.SetBool("isClear", true);
            return;
        }

        if (isDead)
        {
            anim.SetBool("isDead", true);
            RespawnTime += Time.deltaTime;

            //부활 대기시간 지날시 부활
            if (RespawnTime >= RespawnDelay)
            {
                Target = null;
                isDead = false;
                anim.SetBool("isDead", false);
                RespawnTime = 0;
                attackDelayTime = 0;
                isDelay = false;
                isSkillOn = false;
                NowHp = MaxHp;
            }
            return;
        }
        else if (Target == null)
            FindTarget();

        if (isDelay)
        {
            attackDelayTime += Time.deltaTime;

            if (isSkillOn)
            {
                //스킬 사용전 스킬 대기시간 만큼 대기
                if (attackDelayTime >= SkillDelay)
                    isDelay = false;
            }
            else
            {
                //스킬 사용후 공격 대기시간 만큼 대기
                if (attackDelayTime >= AttackDelay)
                    isDelay = false;
            }
        }
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
    }

    #endregion

    #region Method

    #region Public Method

    public bool GetCharacterDead() => isDead;

    /// <summary>공격 받는 함수(성직자의 스킬의 경우 아군 체력 채우는 용도)</summary>
    /// <param name="Dmg"></param>
    public void SetDamange(float Dmg)
    {
        NowHp -= Dmg;

        if (NowHp <= 0)
            isDead = true;
        else if (NowHp >= MaxHp)
            NowHp = MaxHp;
    }

    /// <summary>다음 스테이지 넘어갈시 초기화</summary>
    public void NextStage()
    {
        anim.SetBool("isClear", false);
        anim.StopPlayback();
        RespawnTime = 0;
        attackDelayTime = 0;
        isDelay = false;
        isSkillOn = false;
    }

    #endregion

    #region Private Method

    private void FindTarget()
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


    private void AttackEnemy()
    {
        if(isSkillOn)
        {
            //각 직업에 맞게 스킬 사용
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
            //스킬 사용후 일반공격을 하기위해 bool변경
            isSkillOn = false;
        }
        else
        {
            //일반 공격후 스킬 사용 가능하게 대기
            Target.GetComponent<MonsterController>().SetDamange(AttackDamage);
            isSkillOn = true;
        }
    }

    private void AttackEnd()
    {
        isDelay = true;
        attackDelayTime = 0;
        anim.SetBool("isAttack", false);
        anim.SetBool("isFind", false);
        if(_job == Job.Priest)
            anim.SetBool("isSkill", false);
        anim.StopPlayback();

        if (Target != null && Target.GetComponent<MonsterController>().GetCharacterDead())
        {
            Target = null;
            anim.SetBool("isAttack", false);
            anim.SetBool("isFind", false);
            anim.StopPlayback();
        }
    }


    private void ThiefSkillTarget()
    {
        foreach (GameObject targetObj in _list_Enemy)
        {
            float findDistance = Vector3.Distance(transform.position, targetObj.transform.position);

            if (findDistance <= SkillRange)
                _list_ThiefTarget.Add(targetObj.transform.GetChild(0).gameObject);
        }
    }

    private void PriestSkillTarget()
    {
        GameObject temp = _list_PriestTarget[3];

        for(int i = 2; i >= 0; i--)
        {
            float findDistance = Vector3.Distance(transform.position, _list_PriestTarget[i].transform.position);

            if (findDistance > SkillRange)
                continue;

            if (_list_PriestTarget[i].GetComponent<CharacterController>().isDead == false &&
                (temp.GetComponent<CharacterController>().MaxHp - temp.GetComponent<CharacterController>().NowHp) < 
                (_list_PriestTarget[i].GetComponent<CharacterController>().MaxHp - _list_PriestTarget[i].GetComponent<CharacterController>().NowHp))
                temp = _list_PriestTarget[i];
        }

        temp.GetComponent<CharacterController>().SetDamange(AttackDamage * 2.5f * -1);
    }

    #endregion

    #endregion
}
