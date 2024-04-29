using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// NowHp = GetChild(2)
/// MaxHp = GetChild(3)
/// </summary>

public class HpBarScript : MonoBehaviour
{
    #region Values

    [SerializeField] Slider[] _HpBars;                              //캐릭터들의 Hp바배열
    [SerializeField] Slider _MonsterBar;                            //몬스터들에게 붙이는 Hp바
    [SerializeField] RectTransform _HpBarGroup;                     //Hp바들을 모을 RectTransform

    List<GameObject> _list_Players = new List<GameObject>();        //플레이어 캐릭터들을 받을 리스트
    List<GameObject> _list_Monsters = new List<GameObject>();       //몬스터들을 관리할 리스트
    List<Slider> _list_MonsterHPBars = new List<Slider>();          //몬스터들의 Hp바를 관리할 리스트
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        //플레이어리스트 받아오기
        _list_Players = GameManager.GetInstance._Players.ToList();

        //플레이어Hp설정
        for (int i = 0; i < _list_Players.Count; i++)
        {
            _HpBars[i].maxValue = _list_Players[i].GetComponent<CharacterController>().MaxHp;
            _HpBars[i].value = _HpBars[i].maxValue;
            _HpBars[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = _HpBars[i].maxValue.ToString();
        }
    }
    private void Update()
    {
        //플레이어와 몬스터들의 Hp바가 각각 알맞은 위치로 추적하도록 설정

        for(int i = 0; i < _list_Players.Count; i++)
        {
            _HpBars[i].transform.position = Camera.main.WorldToScreenPoint(_list_Players[i].transform.position + new Vector3(0, 2.2f, 0));
            _HpBars[i].value = _list_Players[i].GetComponent<CharacterController>().NowHp;
            _HpBars[i].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _HpBars[i].value.ToString();
        }

        for(int j = 0; j < _list_Monsters.Count; j++)
        {
            _list_MonsterHPBars[j].transform.position = Camera.main.WorldToScreenPoint(_list_Monsters[j].transform.position + new Vector3(0, 1.4f, 0));
            _list_MonsterHPBars[j].value = _list_Monsters[j].transform.GetChild(0).GetComponent<MonsterController>().NowHp;
            _list_MonsterHPBars[j].transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _list_MonsterHPBars[j].value.ToString();
        }

    }

    #endregion

    #region Method

    /// <summary>몬스터 생성시 Hp바 추가</summary>
    /// <param name="monster">추가된 몬스터</param>
    public void AddMonsterHPBar(GameObject monster)
    {
        Slider hpSlider = Instantiate(_MonsterBar, _HpBarGroup);

        hpSlider.maxValue = monster.GetComponent<MonsterController>().MaxHp;
        hpSlider.value = hpSlider.maxValue;
        hpSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = hpSlider.maxValue.ToString();

        _list_Monsters.Add(monster.transform.parent.gameObject);
        _list_MonsterHPBars.Add(hpSlider);
    }

    /// <summary>몬스터 사망시 Hp바 제거</summary>
    /// <param name="monster">사망한 몬스터</param>
    public void DeadMonster(GameObject monster)
    {
        int idx = _list_Monsters.FindIndex(x => x.transform.GetChild(0).gameObject == monster);
        Slider deleteItem = _list_MonsterHPBars[idx];
        _list_MonsterHPBars.RemoveAt(idx);
        _list_Monsters.RemoveAt(idx);

        Destroy(deleteItem.gameObject);
    }

    /// <summary>캐릭터 최대 체력 증가</summary>
    public void MaxHpIncrease()
    {
        for(int i = 0; i < _list_Players.Count; i++)
        {
            _HpBars[i].maxValue = _list_Players[i].GetComponent<CharacterController>().MaxHp;
            _HpBars[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = _HpBars[i].maxValue.ToString();
        }
    }
    #endregion
}
