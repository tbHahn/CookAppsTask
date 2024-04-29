using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _MonsterPrefab;
    [SerializeField] Transform _MonsterGroup;
    [SerializeField] int MonsterCount;
    [SerializeField] HpBarScript _hpBarScript;


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
}
