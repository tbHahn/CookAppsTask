using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfomationScript : MonoBehaviour
{
    #region Values
    [SerializeField] GameObject _infoPrefab;            //정보보기 버튼 프리팹
    [SerializeField] RectTransform _infoTransform;      //정보버튼 생성할 위치
    [SerializeField] Sprite[] _infoThumnail;             //캐릭터 초상화

    List<GameObject> _list_InfoBtn = new List<GameObject>();    //버튼관리 리스트
    bool isOpen;    //정보창확인용 bool
    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        SettingBtn();
    }

    #endregion

    #region Method

    #region Public
    public void OnBtn_Infomation(int index)
    {
        if(isOpen)
        {
            _list_InfoBtn[index].transform.GetChild(0).gameObject.SetActive(false);
            isOpen = false;
        }
        else
        {
            SettingDetail(index);
            _list_InfoBtn[index].transform.GetChild(0).gameObject.SetActive(true);
            isOpen = true;
        }
    }

    #endregion

    #region Private

    /// <summary>버튼 생성</summary>
    private void SettingBtn()
    {
        for (int i = 0; i < GameManager.GetInstance._Players.Length; i++)
        {
            int idx = i;
            GameObject go = Instantiate(_infoPrefab, _infoTransform);
            go.GetComponent<Image>().sprite = _infoThumnail[idx];
            go.GetComponent<Button>().onClick.AddListener(() => OnBtn_Infomation(idx));
            _list_InfoBtn.Add(go);
        }
    }

    /// <summary>디테일 설정</summary>
    /// <param name="index">캐릭터 인덱스</param>
    private void SettingDetail(int index)
    {
        _list_InfoBtn[index].transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text =
            "MaxHp = " + GameManager.GetInstance._Players[index].GetComponent<CharacterController>().MaxHp.ToString();
        _list_InfoBtn[index].transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text =
            "Attack = " + GameManager.GetInstance._Players[index].GetComponent<CharacterController>().AttackDamage.ToString();
    }

    #endregion

    #endregion
}
