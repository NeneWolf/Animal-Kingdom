using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    int enemyCount;
    GameObject[] enemies;

    [SerializeField] Text enemyCountText;
    [SerializeField] GameObject playerCanvas;
    [SerializeField] GameObject endGamePanel;

    public bool isGameOver;

    ScenesManager scenesManager;

    private void Awake()
    {
        scenesManager = FindAnyObjectByType<ScenesManager>();
    }


    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemies.Length;
    }

    void Update()
    {
        enemyCountText.text = "Enemies: " + enemyCount;
    }

    public void EnemyDied()
    {
        enemyCount--;

        if (enemyCount <= 0)
        {
            isGameOver = true;
            playerCanvas.SetActive(false);
            endGamePanel.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void RestartGame()
    {
        scenesManager.ChangeSceneByID(1);
    }

    public void MainMenu()
    {
        scenesManager.ChangeSceneByID(0);
    }

    public void QuitGame()
    {
        scenesManager.QuitGame();
    }
}
