using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Values
    [SerializeField] GameObject _MonsterPrefab;         //몬스터 프리팹
    [SerializeField] Transform _MonsterGroup;           //몬스터들 모아놓을 그룹
    [SerializeField] int MonsterCount;                  //최대 몬스터 수
    [SerializeField] HpBarScript _hpBarScript;          //HP관련 스크립트
    [SerializeField] LevelScript _expBarScript;         //경험치 관련 스크립트
    [SerializeField] ShopScript _shopScript;            //상점 관련 스크립트    

    [HideInInspector] public List<GameObject> _list_Monsters = new List<GameObject>();//몬스터를 관리할 리스트

    public GameObject[] _Players;       //캐릭터들을 관리할 배열
    float spwanTime;
    float delayTime;
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

        //몬스터의 수가 최대제한 아래일시 몬스터 생성
        if (_list_Monsters.Count < MonsterCount)
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


    /// <summary>보상을 주는 함수</summary>
    /// <param name="monster">어떠한 몬스터가 사망했는지 알려주는 파라미터</param>
    public void Reward(GameObject monster)
    {
        GameObject obj = _list_Monsters.Find(x => x.transform.GetChild(0).gameObject == monster);

        if (obj == null)
            return;

        _expBarScript.GetExpPoint(obj.transform.GetChild(0).gameObject);        //경험치
        _shopScript.EarnGold(monster.GetComponent<MonsterController>().gold);   //골드
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

    #endregion

    #endregion
}
