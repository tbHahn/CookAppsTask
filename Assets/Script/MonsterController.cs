using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>정찰 Enum</summary>
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
    #region Values

    [SerializeField] int targetRange;   //적 감지 범위
    [SerializeField] int gold = 10;     //골드 보상
    [SerializeField] int exp = 10;      //경험치 보상


    List<GameObject> PlayerCharacters;  //플레이어의 캐릭터를 찾기 위한 리스트
    float shortDistance;                //가까운 적을 찾기위한 최소거리
    GameObject Target;                  //찾아낸 적

    PatrolMode patrolMode;
    Vector3 targetDir;                  //타겟방향
    Animator anim;                      //애니메이터

    bool isPatrol;                      //정찰중인지 확인하는 bool
    bool isDelay;                       //공격의 딜레이를 확인하는 bool
    bool isDead;                        //사망여부를 확인하는 bool
    bool isReward;                      //보상을 주었는지를 확인하는 bool
    bool isBoss;

    float attackDelayTime;              //공격 대기 시간
    float patrolDelay = 5;              //정찰 대기 시간(초기값은 임의설정)
    float chaseRange = 10;              //추적 사거리

    [HideInInspector] public float NowHp;       //현재체력
    

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        //초기 캐릭터 리스트 생성
        PlayerCharacters = new List<GameObject>(GameObject.FindGameObjectsWithTag("Player"));

        shortDistance = 15f;
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //몬스터 기초설정
        StatusSetting();
        //몬스터 체력바 설정
        GameManager.GetInstance.GetHpInfo().AddMonsterHPBar(this.gameObject);
        NowHp = MaxHp;
        //정찰할경우의 정찰모드 설정
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

        //적이 없을경우 계속 추적시도
        if (Target == null)
            FoundEnemy();

        //추적 실패시 계속 정찰
        if(isPatrol)
            PatrolMap();


        if (isDelay)
        {
            attackDelayTime += Time.deltaTime;
            if (attackDelayTime >= AttackDelay)
                isDelay = false;
        }
        else if (!isDelay)
        {
            //적 발견시
            if (Target != null && targetDir.sqrMagnitude > AttackRange)
            {
                targetDir = Target.transform.position - transform.parent.position;

                if(targetDir.sqrMagnitude > Mathf.Pow(chaseRange,2))
                {
                    //일정거리 벗어날경우 추적실패 -> 정찰돌입
                    isPatrol = true;
                    return;
                }

                transform.parent.Translate(targetDir.normalized * 1 * Time.deltaTime);
                isPatrol = false;

                anim.SetBool("FindEnemy", true);
            }
            //적이 공격 사거리에 들어온 경우
            if (Target != null && AttackRange >= targetDir.sqrMagnitude)
            {
                isPatrol = false;
                patrolDelay = 0;
                anim.SetBool("AttackEnemy", true);
            }
        }

    }

    #endregion

    #region Method

    #region Public Method
    /// <summary>사망했는지 확인하는 함수</summary>
    public bool GetCharacterDead() => isDead;

    public int GetGold() => gold;

    public int GetExp() => exp;

    /// <summary>피해받을때</summary>
    /// <param name="Dmg">받는 데미지</param>
    public void SetDamange(float Dmg)
    {
        NowHp -= Dmg;

        if (NowHp <= 0)
            isDead = true;
    }

    /// <summary>기사의 공격을 받을시 스턴</summary>
    public void SetStun()
    {
        isDelay = true;
        attackDelayTime = 0;
        anim.SetBool("AttackEnemy", false);
        anim.SetBool("FindEnemy", false);
        anim.StopPlayback();
    }

    public void SettingBoss()
    {
        MaxHp *= 3;
        AttackDamage *= 3;
        AttackRange *= 3;
        AttackDelay *= 3;
        gold *= 3;
        exp *= 3;
        isBoss = true;
    }

    public bool GetBossStatus => isBoss;

    #endregion

    #region Private Method

    /// <summary>스테이지별 몬스터 세팅</summary>
    private void StatusSetting()
    {
        int stage = GameManager.GetInstance.GetStageInfo().GetStage();

        if(stage > 1)
        {
            AttackDamage += (AttackDamage * (0.1f * stage));
            MaxHp += (MaxHp * (0.1f * stage));
        }
    }


    /// <summary>적을 찾는 함수</summary>
    private void FoundEnemy()
    {
        Target = null;

        foreach(GameObject targetObj in  PlayerCharacters)
        {
            if (targetObj.GetComponent<CharacterController>().GetCharacterDead())
                continue; //죽은 캐릭터는 넘어가기

            float findDistance = Vector3.Distance(transform.parent.position, targetObj.transform.position);

            //가장 가까운 적을 찾기위한 비교문
            if(findDistance <= shortDistance + 0.1f)
            {
                Target = targetObj;
                shortDistance = findDistance;
            }
        }
        //타겟을 못정할시 정찰모드
        if(Target == null)
        {
            isPatrol = true;
            return;
        }

        targetDir = Target.transform.position - transform.parent.position;
        isPatrol = false;//정찰종료

        //타겟의 위치에 따라 스프라이트 반전
        if (isBoss)
        {
            if (targetDir.x < 0)
                transform.parent.localScale = new Vector3(-1.5f , transform.parent.localScale.y, transform.parent.localScale.z);
            else if (targetDir.x >= 0)
                transform.parent.localScale = Vector3.one * 1.5f;
        }
        else
        {
            if (targetDir.x < 0)
                transform.parent.localScale = new Vector3(-1, 1, 1);
            else if (targetDir.x >= 0)
                transform.parent.localScale = Vector3.one;
        }

        Debug.Log(Target.name);
    }

    private void AttackPlayer()
    {
        Target.GetComponent<CharacterController>().SetDamange(AttackDamage);
    }

    private void AttackEnd()
    {
        isDelay = true;
        attackDelayTime = 0;
        anim.SetBool("AttackEnemy", false);
        anim.SetBool("FindEnemy", false);
        anim.StopPlayback();

        //적 사망시 애니메이션 종료
        if (Target.GetComponent<CharacterController>().GetCharacterDead())
        {
            Target = null;
            anim.SetBool("AttackEnemy", false);
            anim.SetBool("FindEnemy", false);
            anim.StopPlayback();
        }
    }

    /// <summary>몬스터 사망시 삭제처리</summary>
    private void DeleteMonster()
    {
        GameManager.GetInstance.GetHpInfo().DeadMonster(this.gameObject);
        int idx = GameManager.GetInstance._list_Monsters.FindIndex(x => x.gameObject.transform == this.transform.parent);
        GameManager.GetInstance._list_Monsters.RemoveAt(idx);
        Destroy(this.transform.parent.gameObject);
    }

    /// <summary>주변 적 정찰</summary>
    private void PatrolMap()
    {
        //3초마다 지정된 정찰모드로 변경
        if (patrolDelay > 3)
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
        patrolDelay += Time.deltaTime;
    }
    #endregion

    #endregion
}
