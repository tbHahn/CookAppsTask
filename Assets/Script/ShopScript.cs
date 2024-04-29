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
    private void Awake()
    {
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
}
