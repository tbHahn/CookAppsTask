using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopScript : MonoBehaviour
{
    #region Valuse

    [Header("상점 기본")]
    [SerializeField] Button _shopBtn;
    [SerializeField] RectTransform _shopPage;
    [SerializeField] TextMeshProUGUI _tmpTotalGold;
    [Header("Hp관련")]
    [SerializeField] TextMeshProUGUI _tmpHp;
    [SerializeField] TextMeshProUGUI _tmpHpGold;
    [Header("공격력관련")]
    [SerializeField] TextMeshProUGUI _tmpAttack;
    [SerializeField] TextMeshProUGUI _tmpAttackGold;

    [HideInInspector] public int totalGold; //현재 가지고 있는 총 골드

    bool isShopOpen;        //상점창이 열렸는지 확인하는 bool
    Vector2 DestPos;        //상점창의 최종 위치

    int HpIncreseFigure;            //Hp증가치
    int HpIncreseGold;              //Hp구입 골드 증가치
    int AttackIncreseFigure;        //공격력 증가치
    int AttackIncreseGold;          //공격력구입 골드 증가치

    int defaultHp;                  //상점 기본 Hp
    int defaultHpGold;              //상점 기본 Hp구입 골드
    int defaultAttack;              //상점 기본 공격력
    int defaultAttackGold;          //상점 기본 공격력구입 골드

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        //초기화

        totalGold = 0;

        HpIncreseFigure = 10;
        HpIncreseGold = 5;
        AttackIncreseFigure = 5;
        AttackIncreseGold = 10;

        defaultHp = HpIncreseFigure;
        defaultHpGold = HpIncreseGold;
        defaultAttack = AttackIncreseFigure;
        defaultAttackGold = AttackIncreseGold;
        SetText();

        DestPos = new Vector2(-850,0);      //상점창 최종위치 임의 지정
    }
    #endregion

    #region Method

    #region PublicMethod

    /// <summary>상점버튼</summary>
    public void OnBtn_Shop()
    {
        if(isShopOpen)
            StartCoroutine(CloseShop());
        else
            StartCoroutine(OpenShop());
    }

    /// <summary>Hp구매 버튼</summary>
    public void OnBtn_IncreaseHp()
    {
        if (defaultHpGold > totalGold)
            return;

        foreach(GameObject character in GameManager.GetInstance._Players)
            character.GetComponent<CharacterController>().MaxHp += HpIncreseFigure;

        GameManager.GetInstance.GetHpInfo().MaxHpIncrease();

        totalGold -= defaultHpGold;
        defaultHpGold += HpIncreseGold;
        //defaultHp += HpIncreseFigure;
        SetHpText();
    }

    /// <summary>공격력 구매 버튼</summary>
    public void OnBtn_IncreaseAttack()
    {
        if (defaultAttackGold > totalGold)
            return;

        foreach (GameObject character in GameManager.GetInstance._Players)
            character.GetComponent<CharacterController>().AttackDamage += AttackIncreseFigure;

        totalGold -= defaultAttackGold;
        defaultAttackGold += AttackIncreseGold;
        //defaultAttack += AttackIncreseFigure;
        SetATKText();
    }

    /// <summary>골드를 얻는 함수</summary>
    /// <param name="gold">골드 액수</param>
    public void EarnGold(int gold)
    {
        totalGold += gold;
        _tmpTotalGold.text = totalGold.ToString();
    }

    #endregion

    #region Private Method

    /// <summary>값 변경시 플레이어에게 보여주기 위한 Text설정</summary>
    private void SetText()
    {
        SetHpText();
        SetATKText();
    }

    private void SetHpText()
    {
        _tmpTotalGold.text = totalGold.ToString();
        _tmpHp.text = defaultHp.ToString();
        _tmpHpGold.text = defaultHpGold.ToString();
    }

    private void SetATKText()
    {
        _tmpTotalGold.text = totalGold.ToString();
        _tmpAttack.text = defaultAttack.ToString();
        _tmpAttackGold.text = defaultAttackGold.ToString();
    }
    #endregion

    #region Coroutine

    /// <summary>상점이 열리는 코루틴</summary>
    private IEnumerator OpenShop()
    {
        while (_shopPage.anchoredPosition.x > -844)
        {
            _shopPage.anchoredPosition = Vector2.Lerp(_shopPage.anchoredPosition, DestPos, 0.1f);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        _shopPage.anchoredPosition = DestPos;
        isShopOpen = true;
        StopCoroutine(OpenShop());
    }

    /// <summary>상점이 닫히는 코루틴</summary>
    private IEnumerator CloseShop()
    {
        while (_shopPage.anchoredPosition.x < 0)
        {
            _shopPage.anchoredPosition = Vector2.Lerp(_shopPage.anchoredPosition, new Vector2(3, 0), 0.1f);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        _shopPage.anchoredPosition = Vector2.zero;
        isShopOpen = false;
        StopCoroutine(CloseShop());
    }

    #endregion

    #endregion
}
