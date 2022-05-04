using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    public int playerMoney = 0;
    public int bankMoney = 0;
    public int lifeCost = 1000;
    public int Level2Cost = 10000;
    public int Level3Cost = 100000;
    private bool Level2Bought = false;
    private bool Level3Bought = false;

    // Start is called before the first frame update
    void Awake()
    {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if(numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            bankMoney = 0;
            ResetGameSession();
        }
    }

    void TakeLife()
    {
        playerLives--;
        playerMoney = 0;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    void ResetGameSession()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }


    public void TransferToBank()
    {
        bankMoney += playerMoney;
        playerMoney = 0;
    }

    public void BuyLife()
    {
        if (bankMoney >= lifeCost)
        {
            bankMoney -= lifeCost;
            playerLives++;
        }
    }


    public void BuyLevel2()
    {
        if(bankMoney >= Level2Cost)
        {
            bankMoney -= Level2Cost;
            Level2Bought = true;
        }
    }

    public void BuyLevel3()
    {
        if (bankMoney >= Level3Cost)
        {
            bankMoney -= Level3Cost;
            Level3Bought = true;
        }
    }

    public void GoToLevel1()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToLevel2()
    {
        if(Level2Bought)
        {
            SceneManager.LoadScene(2);
        }
    }

    public void GoToLevel3()
    {
        if (Level3Bought)
        {
            SceneManager.LoadScene(3);
        }
    }

    public void updateMoney(int amount)
    {
        playerMoney += amount;
    }

    public int PlayerLives { 
        get
        {
            return playerLives;
        }
    }

    public int PlayerMoney {
        get
        {
            return playerMoney;
        }
    }
}
