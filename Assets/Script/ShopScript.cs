using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopScript : MonoBehaviour
{
    [SerializeField] Button _shopBtn;
    [SerializeField] RectTransform _shopPage;
    [SerializeField] TextMeshProUGUI _tmpTotalGold;

    [SerializeField] TextMeshProUGUI _tmpHp;
    [SerializeField] TextMeshProUGUI _tmpHpGold;

    [SerializeField] TextMeshProUGUI _tmpAttack;
    [SerializeField] TextMeshProUGUI _tmpAttackGold;

    bool isShopOpen;

    Vector2 DestPos;

    [HideInInspector] public int totalGold;

    int HpIncreseFigure;
    int HpIncreseGold;
    int AttackIncreseFigure;
    int AttackIncreseGold;

    int defaultHp;
    int defaultHpGold;
    int defaultAttack;
    int defaultAttackGold;

    private void Awake()
    {
        totalGold = 0;

        HpIncreseFigure = 10;
        HpIncreseGold = 5;
        AttackIncreseFigure = 5;
        AttackIncreseGold = 10;

        defaultHp = HpIncreseFigure;
        defaultHpGold = HpIncreseGold;
        defaultAttack = AttackIncreseFigure;
        defaultAttackGold = AttackIncreseGold;

        //_tmpTotalGold.text = totalGold.ToString();
        //_tmpAttack.text = AttackIncreseFigure.ToString();
        //_tmpAttackGold.text = AttackIncreseGold.ToString();
        //_tmpHp.text = HpIncreseFigure.ToString();
        //_tmpHpGold.text = HpIncreseGold.ToString();
        SetText();

        DestPos = new Vector2(-850,0);
    }

    public void OnBtn_Shop()
    {
        if(isShopOpen)
        {
            StartCoroutine(CloseShop());
        }
        else
        {
            StartCoroutine(OpenShop());
        }
    }

    IEnumerator OpenShop()
    {
        while(_shopPage.anchoredPosition.x > -844)
        {
            _shopPage.anchoredPosition = Vector2.Lerp(_shopPage.anchoredPosition, DestPos, 0.1f);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        _shopPage.anchoredPosition = DestPos;
        isShopOpen = true;
        StopCoroutine(OpenShop());
    }

    IEnumerator CloseShop()
    {
        while (_shopPage.anchoredPosition.x < 0)
        {
            _shopPage.anchoredPosition = Vector2.Lerp(_shopPage.anchoredPosition, new Vector2(3,0), 0.1f);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        _shopPage.anchoredPosition = Vector2.zero;
        isShopOpen = false;
        StopCoroutine(CloseShop());
    }

    public void OnBtn_IncreaseHp()
    {
        if (defaultHpGold > totalGold)
            return;

        foreach(GameObject character in GameManager.GetInstance._Players)
            character.GetComponent<CharacterController>().MaxHp += defaultHp;

        GameManager.GetInstance.GetHpInfo().MaxHpIncrease();

        totalGold -= defaultHpGold;
        defaultHpGold += HpIncreseGold;
        defaultHp += HpIncreseFigure;
        SetHpText();
    }

    public void OnBtn_IncreaseAttack()
    {
        if (defaultAttackGold > totalGold)
            return;

        foreach (GameObject character in GameManager.GetInstance._Players)
            character.GetComponent<CharacterController>().AttackDamage += defaultAttack;

        totalGold -= defaultAttackGold;
        defaultAttackGold += AttackIncreseGold;
        defaultAttack += AttackIncreseFigure;
        SetATKText();
    }

    void SetText()
    {
        SetHpText();
        SetATKText();
    }

    void SetHpText()
    {
        _tmpTotalGold.text = totalGold.ToString();
        _tmpHp.text = defaultHp.ToString();
        _tmpHpGold.text = defaultHpGold.ToString();
    }

    void SetATKText()
    {
        _tmpTotalGold.text = totalGold.ToString();
        _tmpAttack.text = defaultAttack.ToString();
        _tmpAttackGold.text = defaultAttackGold.ToString();
    }

    public void EarnGold(int gold)
    {
        totalGold += gold;
        _tmpTotalGold.text = totalGold.ToString();
    }
}
