using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesText : MonoBehaviour
{
    public Text livesText;
    private GameSession gameSession;
    

    void Start()
    {
        livesText = GetComponent<Text>();
        gameSession = FindObjectOfType<GameSession>();
    }

   
    void Update()
    {
        livesText.text = "Lives: " + gameSession.PlayerLives.ToString();
    }
}
