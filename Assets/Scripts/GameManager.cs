using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image ModeImage;
    public Text GameModeTxt;
    public string [] GameModes;
    public Sprite[] GameModeImg;

    public static GameManager Instance;

    private int currentText = 1;


    private void Start()
    {
        Instance = this;
    }

    public void LeftArrowPressed()
    {
        currentText--;
        if (currentText<0)
            currentText = 4;

        GameModeTxt.text = GameModes[currentText];
        ModeImage.sprite = GameModeImg[currentText];
    }

    public void RightArrowPressed()
    {
        currentText++;
        if (currentText > 4)
            currentText = 0;

        GameModeTxt.text = GameModes[currentText];
        ModeImage.sprite = GameModeImg[currentText];
    }
}
