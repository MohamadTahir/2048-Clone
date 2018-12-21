using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public Image ModeImage;
    public Text GameModeTxt;
    public string [] GameModes;
    public Sprite[] GameModeImg;
    public int GameSize = 4;

    public static GameManager Instance;


    private int GameMode = 1;


    private void Start()
    {
        Instance = this;
    }

    public void LeftArrowPressed()
    {
        GameMode--;
        if (GameMode < 0)
            GameMode = 4;

        GameModeTxt.text = GameModes[GameMode];
        ModeImage.sprite = GameModeImg[GameMode];
    }

    public void RightArrowPressed()
    {
        GameMode++;
        if (GameMode > 4)
            GameMode = 0;

        GameModeTxt.text = GameModes[GameMode];
        ModeImage.sprite = GameModeImg[GameMode];
    }

    public void StartGame()
    {
        DontDestroyOnLoad(gameObject);
        SetGameSize();
        SceneManager.LoadScene(1);

        
    }
    private void SetGameSize()
    {
        if (GameMode == 0)
            GameSize = 3;

        if (GameMode == 1)
            GameSize = 4;

        if (GameMode == 2)
            GameSize = 5;  

        if (GameMode == 3)
            GameSize = 6;

        if (GameMode == 4)
            GameSize = 8;
    }
}
