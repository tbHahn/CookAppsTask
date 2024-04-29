using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSceneScript : MonoBehaviour
{
    public void RestartBtn()
    {
        SceneManager.LoadScene("GameScene");//원래의 게임 씬으로 이동
    }
}
