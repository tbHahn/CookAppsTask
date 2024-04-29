using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelScript : MonoBehaviour
{
    [SerializeField] Slider _expBar;
    [SerializeField] RectTransform _levelGroup;

    List<GameObject> _list_Players = new List<GameObject>();
    List<Slider> _list_PlayersEXPBars = new List<Slider>();

    private void Update()
    {
        for(int i = 0; i < _list_PlayersEXPBars.Count; i++)
            _list_PlayersEXPBars[i].transform.position = Camera.main.WorldToScreenPoint(_list_Players[i].transform.position + new Vector3(0, 2.8f, 0));
        
    }

    public void AddLevelBar(GameObject player)
    {
        Slider sld = Instantiate(_expBar, _levelGroup);

        sld.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "LV" + player.GetComponent<CharacterController>().level.ToString();

        _list_Players.Add(player);
        _list_PlayersEXPBars.Add(sld);
    }

    public void GetExpPoint(GameObject monster)
    {
        for(int i = 0; i < _list_Players.Count; i++)
        {
            if(_list_Players[i].GetComponent<CharacterController>().Target == monster)
            {
                _list_PlayersEXPBars[i].value += 10;
                if (_list_PlayersEXPBars[i].value >= _expBar.maxValue)
                    LevelUp(_list_Players[i], i);
            }
        }
    }

    void LevelUp(GameObject player, int idx)
    {
        player.GetComponent<CharacterController>().level++;
        _list_PlayersEXPBars[idx].value = 0;
        _list_PlayersEXPBars[idx].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "LV" + player.GetComponent<CharacterController>().level.ToString();
        player.GetComponent<CharacterController>().NowHp = player.GetComponent<CharacterController>().MaxHp;
    }
}
