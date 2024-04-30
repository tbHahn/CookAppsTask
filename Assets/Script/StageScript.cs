using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StageScript : MonoBehaviour
{
    #region Valuse

    [SerializeField] GameObject _stageClear;        //스테이지 클리어시 나오는 문구 오브젝트
    [SerializeField] TextMeshProUGUI _stageInfo;    //현재 스테이지가 몇스테인지 알려주기 위한 tmp

    bool isClear;       //클리어유무
    float showTime;     
    int stage;

    #endregion

    #region MonoBehaviour

    private void Awake()
    {
        stage = 1;
        _stageInfo.text = stage.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(isClear)
            showTime += Time.deltaTime;
        
        //5초후 자동으로 다음 스테이지 진입
        if(showTime > 5)
        {
            isClear = false;
            showTime = 0;
            _stageClear.SetActive(false);
            _stageInfo.text = stage.ToString();
            GameManager.GetInstance.NextStage();
        }
    }

    #endregion

    #region Method

    public int GetStage() => stage;

    public void StageClear()
    {
        _stageClear.SetActive(true);
        isClear = true;
        stage++;
    }
    #endregion
}
