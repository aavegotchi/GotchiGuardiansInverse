using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOver_UI : MonoBehaviour
{
    #region Fields
    [SerializeField] private string loadSceneName = "GotchiTowerDefense";
    [SerializeField] private GameObject managers;
    #endregion

    #region Unity Functions
    void OnEnable()
    {
        Time.timeScale = 0;
    }

    void OnDisable()
    {
        Time.timeScale = 1;
    }
    #endregion

    #region Public Functions
    public void Restart()
    {
        Destroy(managers);
        SceneManager.LoadScene(loadSceneName);
    }
    #endregion
}