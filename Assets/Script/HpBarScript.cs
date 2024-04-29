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
    [SerializeField] Slider[] _HpBars;

    [SerializeField] Slider _MonsterBar;

    [SerializeField] RectTransform _HpBarGroup;

    List<GameObject> _list_Players = new List<GameObject>();

    List<GameObject> _list_Monsters = new List<GameObject>();
    List<Slider> _list_MonsterHPBars = new List<Slider>();

    private void Awake()
    {
        _list_Players = GameManager.GetInstance._Players.ToList();

        for (int i = 0; i < _list_Players.Count; i++)
        {
            _HpBars[i].maxValue = _list_Players[i].GetComponent<CharacterController>().MaxHp;
            _HpBars[i].value = _HpBars[i].maxValue;
            _HpBars[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = _HpBars[i].maxValue.ToString();
        }
    }

    private void Update()
    {
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


    public void AddMonsterHPBar(GameObject monster)
    {
        Slider hpSlider = Instantiate(_MonsterBar, _HpBarGroup);

        hpSlider.maxValue = monster.GetComponent<MonsterController>().MaxHp;
        hpSlider.value = hpSlider.maxValue;
        hpSlider.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = hpSlider.maxValue.ToString();

        _list_Monsters.Add(monster.transform.parent.gameObject);
        _list_MonsterHPBars.Add(hpSlider);
    }

    public void DeadMonster(GameObject monster)
    {
        int idx = _list_Monsters.FindIndex(x => x.transform.GetChild(0).gameObject == monster);
        Slider deleteItem = _list_MonsterHPBars[idx];
        _list_MonsterHPBars.RemoveAt(idx);
        _list_Monsters.RemoveAt(idx);

        Destroy(deleteItem.gameObject);
    }

    public void MaxHpIncrease()
    {
        for(int i = 0; i < _list_Players.Count; i++)
        {
            _HpBars[i].maxValue = _list_Players[i].GetComponent<CharacterController>().MaxHp;
            _HpBars[i].transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = _HpBars[i].maxValue.ToString();
        }

    }
}
