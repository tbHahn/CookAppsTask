using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSceneScript : MonoBehaviour
{
    public void RestartBtn()
    {
        SceneManager.LoadScene("GameScene");
    }
}
