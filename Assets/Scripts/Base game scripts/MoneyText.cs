using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyText : MonoBehaviour
{
    public Text money;
    private GameSession gameSession;


    void Start()
    {
        money = GetComponent<Text>();
        gameSession = FindObjectOfType<GameSession>();
    }


    void Update()
    {
        money.text = "Money: " + gameSession.PlayerMoney.ToString();
    }
}
