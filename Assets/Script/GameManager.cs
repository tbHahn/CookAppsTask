using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _MonsterPrefab;
    [SerializeField] Transform _MonsterGroup;
    [SerializeField] int MonsterCount;
    [SerializeField] HpBarScript _hpBarScript;
    [SerializeField] LevelScript _expBarScript;
    [SerializeField] ShopScript _shopScript;

    [HideInInspector] public bool SpwanCall;
    [HideInInspector] public List<GameObject> _list_Monsters = new List<GameObject>();



    public GameObject[] _Players;
    

    static GameManager Instance;

    float spwanTime;
    float delayTime;


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

    private void Start()
    {
        MonsterSpwan();
        delayTime = _list_Monsters[0].transform.GetChild(0).GetComponent<MonsterController>().RespawnDelay;
    }

    private void Update()
    {
        if(_Players[0].GetComponent<CharacterController>().GetCharacterDead() &&
            _Players[1].GetComponent<CharacterController>().GetCharacterDead() &&
            _Players[2].GetComponent<CharacterController>().GetCharacterDead() &&
            _Players[3].GetComponent<CharacterController>().GetCharacterDead())
        {
            SceneManager.LoadScene("GameOverScene");
        }

        if (_list_Monsters.Count < MonsterCount)
        {
            if (spwanTime < delayTime)
            {
                spwanTime += Time.deltaTime;
            }
            else
            {
                MonsterSpwan();
                spwanTime = 0;
            }
        }
    }

    void MonsterSpwan()
    {
        float rangeX = Random.Range(-10f, 10f);
        float rangeY = Random.Range(-3f, 3f);
        GameObject go = Instantiate(_MonsterPrefab, _MonsterGroup);
        go.transform.position = new Vector3(rangeX, rangeY, 0);
        _list_Monsters.Add(go);
    }

    public HpBarScript GetHpInfo() => _hpBarScript;

    public LevelScript GetLevelInfo() => _expBarScript;

    public void Reward(GameObject monster)
    {
        GameObject obj = _list_Monsters.Find(x => x.transform.GetChild(0).gameObject == monster);

        if (obj == null)
            return;

        _expBarScript.GetExpPoint(obj.transform.GetChild(0).gameObject);
        _shopScript.EarnGold(monster.GetComponent<MonsterController>().gold);
    }
}
