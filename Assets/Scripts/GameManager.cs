using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Image ModeImage;
    public Text GameModeTxt;
    public string[] GameModes;
    public Sprite[] GameModeImg;
    public bool testMode = true;

    public static int TheGameSize;
    public static GameManager Instance;

    private int GameMode;

    private void Start()
    {
        Instance = this;

        if (SceneManager.GetActiveScene().buildIndex == 0) {
            GameMode = PlayerPrefs.GetInt("GameMode", 1);
            GameModeTxt.text = GameModes[GameMode];
            ModeImage.sprite = GameModeImg[GameMode];
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().buildIndex==0)
            {
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene(0);
                AdsManager.instance.ShowAdd();
                Destroy(this.gameObject);
            }
        }
    }

    public void LeftArrowPressed()
    {
        GameMode--;
        if (GameMode < 0)
            GameMode = 4;

        PlayerPrefs.SetInt("GameMode", GameMode);
        GameModeTxt.text = GameModes[GameMode];
        ModeImage.sprite = GameModeImg[GameMode];
    }

    public void RightArrowPressed()
    {
        GameMode++;
        
        if (GameMode > 4)
            GameMode = 0;

        PlayerPrefs.SetInt("GameMode", GameMode);
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
        switch (GameMode)
        {
            case 0:
                TheGameSize = 3;
                break;

            case 1:
                TheGameSize = 4;
                break;

            case 2:
                TheGameSize = 5;
                break;

            case 3:
                TheGameSize = 6;
                break;

            case 4:
                TheGameSize = 8;
                break;
        }
    }

    public void RestartGame()
    {
        InGameManager.instance.CurrentData.Clear();
        File.Delete(Application.persistentDataPath + "/SaveGame"+TheGameSize+".dat");
        File.Delete(Application.persistentDataPath + "/PreviousStep.dat");
        PlayerPrefs.SetInt("score", 0);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
