using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelScript : MonoBehaviour
{
    #region Valuse

    [SerializeField] Slider _expBar;                //경험치 바
    [SerializeField] RectTransform _levelGroup;     //경험치 바들을 모아놓을 그룹

    List<GameObject> _list_Players = new List<GameObject>();    //캐릭터들을 관리할 리스트
    List<Slider> _list_PlayersEXPBars = new List<Slider>();     //캐릭터들의 경험치 바를 관리할 리스트

    #endregion

    #region MonoBehaviour

    private void Update()
    {
        //경험치바들이 알맞은 캐릭터들을 추적

        for(int i = 0; i < _list_PlayersEXPBars.Count; i++)
            _list_PlayersEXPBars[i].transform.position = Camera.main.WorldToScreenPoint(_list_Players[i].transform.position + new Vector3(0, 2.8f, 0));
    }

    #endregion

    #region Method

    #region Public Method

    /// <summary>경험치 바 추가</summary>
    /// <param name="player">추가될 캐릭터</param>
    public void AddLevelBar(GameObject player)
    {
        Slider sld = Instantiate(_expBar, _levelGroup);

        sld.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "LV" + player.GetComponent<CharacterController>().level.ToString();

        _list_Players.Add(player);
        _list_PlayersEXPBars.Add(sld);
    }

    /// <summary>경험치 획득</summary>
    /// <param name="monster">경험치를 주는 몬스터</param>
    public void GetExpPoint(GameObject monster)
    {
        for(int i = 0; i < _list_Players.Count; i++)
        {
            if(_list_Players[i].GetComponent<CharacterController>().Target == monster)
            {
                _list_PlayersEXPBars[i].value += monster.GetComponent<MonsterController>().GetExp();
                if (_list_PlayersEXPBars[i].value >= _expBar.maxValue)
                    LevelUp(_list_Players[i], i);
            }
        }
    }

    #endregion

    #region Private Method

    /// <summary>레벨업시 사용되는 함수</summary>
    /// <param name="player">레벨업을 한 캐릭터</param>
    /// <param name="idx">레벨업을 한 캐릭터의 인덱스</param>
    private void LevelUp(GameObject player, int idx)
    {
        player.GetComponent<CharacterController>().level++;
        _list_PlayersEXPBars[idx].value = 0;
        _list_PlayersEXPBars[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "LV" + player.GetComponent<CharacterController>().level.ToString();
        player.GetComponent<CharacterController>().NowHp = player.GetComponent<CharacterController>().MaxHp;
    }
    #endregion

    #endregion
}
