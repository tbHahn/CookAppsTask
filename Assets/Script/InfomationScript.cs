using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InfomationScript : MonoBehaviour
{
    #region Values
    [SerializeField] GameObject _infoPrefab;            //�������� ��ư ������
    [SerializeField] RectTransform _infoTransform;      //������ư ������ ��ġ
    [SerializeField] Sprite[] _infoThumnail;             //ĳ���� �ʻ�ȭ

    List<GameObject> _list_InfoBtn = new List<GameObject>();    //��ư���� ����Ʈ
    bool isOpen;    //����âȮ�ο� bool
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

    /// <summary>��ư ����</summary>
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

    /// <summary>������ ����</summary>
    /// <param name="index">ĳ���� �ε���</param>
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
