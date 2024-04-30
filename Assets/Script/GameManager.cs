using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Values
    [Header("기본 정보")]
    [SerializeField] GameObject _MonsterPrefab;         //몬스터 프리팹
    [SerializeField] Transform _MonsterGroup;           //몬스터들 모아놓을 그룹
    [SerializeField] int MaxMonsterCount;                  //최대 몬스터 수
    [SerializeField] HpBarScript _hpBarScript;          //HP관련 스크립트
    [SerializeField] LevelScript _expBarScript;         //경험치 관련 스크립트
    [SerializeField] ShopScript _shopScript;            //상점 관련 스크립트    

    [Header("스테이지 관련")]
    [SerializeField] StageScript _stageScript;          //스테이지 관련 스크립트
    [SerializeField] int _bossCondition;                //보스 등장 조건

    [HideInInspector] public List<GameObject> _list_Monsters = new List<GameObject>();//몬스터를 관리할 리스트

    public GameObject[] _Players;       //캐릭터들을 관리할 배열
    float spwanTime;
    float delayTime;

    int  deathMonsterCount;             //지금까지 처치된 몬스터의 수

    bool isBoss;        //보스 등장 조건 충족
    bool isStageClear;  //스테이지 클리어를 확인하는 bool
    #endregion

    #region Singleton
    static GameManager Instance;

    public static GameManager GetInstance
    {
        get
        {
            GameObject go = GameObject.Find("GameManager");

            if (go == null)
            {
                go = new GameObject() { name = "GameManager" };
                Instance = go.AddComponent<GameManager>();
                DontDestroyOnLoad(go);
            }

            Instance = go.GetComponent<GameManager>();

            return Instance;
        }
    }
    #endregion

    #region MonoBehaviour

    private void Start()
    {
        MonsterSpwan();
        
        delayTime = _list_Monsters[0].transform.GetChild(0).GetComponent<MonsterController>().RespawnDelay;//초기 재생성 대기 시간 설정
    }

    private void Update()
    {
        if(_Players[0].GetComponent<CharacterController>().GetCharacterDead() &&
            _Players[1].GetComponent<CharacterController>().GetCharacterDead() &&
            _Players[2].GetComponent<CharacterController>().GetCharacterDead() &&
            _Players[3].GetComponent<CharacterController>().GetCharacterDead())
        {
            SceneManager.LoadScene("GameOverScene"); // 모든 캐릭터 사망시 게임 오버씬으로 전환
        }

        if (isStageClear)
            return;

        if(isBoss && _list_Monsters.Count == 0)
        {
            isStageClear = true;
            _stageScript.StageClear();
            return;
        }
        else if (isBoss)
            return;

        if(deathMonsterCount >= _bossCondition)
        {
            isBoss = true;
            BossAppear();
        }


        //몬스터의 수가 최대제한 아래일시 몬스터 생성
        if (_list_Monsters.Count < MaxMonsterCount)
        {
            if (spwanTime < delayTime)
                spwanTime += Time.deltaTime;
            else
            {
                MonsterSpwan();
                spwanTime = 0;//생성시 대기시간 초기화
            }
        }
    }

    #endregion

    #region Method

    #region Public Method

    public HpBarScript GetHpInfo() => _hpBarScript;

    public LevelScript GetLevelInfo() => _expBarScript;

    public StageScript GetStageInfo() => _stageScript;

    public bool GetStageClear() => isStageClear;

    /// <summary>보상을 주는 함수</summary>
    /// <param name="monster">어떠한 몬스터가 사망했는지 알려주는 파라미터</param>
    public void Reward(GameObject monster)
    {
        GameObject obj = _list_Monsters.Find(x => x.transform.GetChild(0).gameObject == monster);

        if (obj == null)
            return;

        _expBarScript.GetExpPoint(obj.transform.GetChild(0).gameObject);        //경험치
        _shopScript.EarnGold(monster.GetComponent<MonsterController>().gold);   //골드

        deathMonsterCount++;
    }

    /// <summary>스테이지 넘어갈시 기준값 변경 및 초기화</summary>
    public void NextStage()
    {
        for (int i = 0; i < _Players.Length; i++)
            _Players[i].GetComponent<CharacterController>().NextStage();

        MaxMonsterCount++;
        spwanTime = 5;
        deathMonsterCount = 0;
        _bossCondition += 2;
        isBoss = false;
        isStageClear = false;
    }

    #endregion

    #region Private Method
    /// <summary>몬스터 생성하는 함수</summary>
    private void MonsterSpwan()
    {
        float rangeX = Random.Range(-10f, 10f);
        float rangeY = Random.Range(-3f, 3f);
        GameObject go = Instantiate(_MonsterPrefab, _MonsterGroup);
        go.transform.position = new Vector3(rangeX, rangeY, 0);
        _list_Monsters.Add(go);
    }

    /// <summary>조건 만족시 보스 등장</summary>
    private void BossAppear()
    {
        GameObject go = Instantiate(_MonsterPrefab, _MonsterGroup);
        go.transform.position = new Vector3(5, -2, 0);//보스 위치 임의 지정
        go.transform.GetChild(0).GetComponent<MonsterController>().SettingBoss();
        _list_Monsters.Add(go);
    }

    #endregion

    #endregion
}
