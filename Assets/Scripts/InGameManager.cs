﻿using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InGameManager : MonoBehaviour
{
    public static InGameManager instance;
    public static int gameSize;

    public Image Plane, box, GameOver;
    public Text ScoreTxt, HightScoreTxt;
    public Grid grid;
    public Transform background, front, back;
    public float BoxSize;
    public List<previousData> CurrentData;
    public int NumbOfSpawns = 0;

    private float sensitivity=50;
    private int SpawnX, SpawnY, Highscore, score, NumberOfFreeGrids=0, NumberOfGrids,PrevScore=0,tempPreviousScore;
    private bool IncreaseBoxSize = false, twoMatchingOnY, twoMatchingOnX, allowMove = true, allowSpawn=true;
    private List<previousData> previousStep, previousStepTemp, temp;
    
    private void Start()
    {
        init();
        CreateGrid();
        SetHighScore();
        SpawnBox();
        //LoadGame();
        setScore(PlayerPrefs.GetInt("score", 0));
    }
    
    private void init()
    {
        instance = this;
        gameSize = GameManager.TheGameSize;
        NumberOfGrids = gameSize * gameSize;
        grid = new Grid();
        previousStepTemp = new List<previousData>();
        previousStep = new List<previousData>();
        CurrentData = new List<previousData>();
        temp = new List<previousData>();
    }

    private void Update()
    {

        if (allowMove)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary)
            {
                Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
                if (touchDeltaPosition.x > sensitivity)
                {
                    previousStepTemp.AddRange(StorePreviousTemp());
                    movePlateRight();
                }

                else if (touchDeltaPosition.x < -sensitivity)
                {
                    previousStepTemp.AddRange(StorePreviousTemp());
                    movePlateLeft();
                }

                else if (touchDeltaPosition.y > sensitivity)
                {
                    previousStepTemp.AddRange(StorePreviousTemp());
                    movePlateUp();
                }

                else if (touchDeltaPosition.y < -sensitivity)
                {
                    previousStepTemp.AddRange(StorePreviousTemp());
                    movePlateDown();
                }

            }
        }

        if (allowMove)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log("down");
                previousStepTemp.AddRange(StorePreviousTemp());
                movePlateDown();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Debug.Log("up");
                previousStepTemp.AddRange(StorePreviousTemp());
                movePlateUp();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log("right");
                previousStepTemp.AddRange(StorePreviousTemp());
                movePlateRight();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Debug.Log("left");
                previousStepTemp.AddRange(StorePreviousTemp());
                movePlateLeft();
            }
        }
    }

    private void LateUpdate()
    {
        if (IncreaseBoxSize)
        {
            Vector2 startSize = grid.pointsArray[SpawnX, SpawnY].box.GetComponent<RectTransform>().sizeDelta;
            startSize.x += 20;
            startSize.y += 20;
            grid.pointsArray[SpawnX, SpawnY].box.GetComponent<RectTransform>().sizeDelta = startSize;
            if (startSize.x >= BoxSize)
            {
                IncreaseBoxSize = false;
                grid.pointsArray[SpawnX, SpawnY].box.GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);
            }

        }
    }

    [System.Serializable]
    public class previousData {
        public int x;
        public int y;
        public int value;

        public previousData(int _x, int _y, int _value)
        {
            x = _x;
            y = _y;
            value = _value;
        }

    }

    public class points
    {
        public bool moving;
        public int value;
        public Vector2 pos;
        public Image box;

        public points(bool _moving, int _value, Vector2 _pos,Image _box)
        {
            moving = _moving;
            value = _value;
            pos = _pos;
            box = _box;
        }
    }
    
    public class Grid
    {
        public points[,] pointsArray;

        public Grid()
        {
            pointsArray = new points[gameSize, gameSize];

            for (int x = 0; x < gameSize; x++)
            {
                for (int y = 0; y < gameSize; y++)
                {
                    pointsArray[x, y] = new points(false, -1, new Vector2(0, 0), null);
                }
            }
        }
    }

    private void CreateGrid()
    {
        float backgroundSize = background.GetComponent<RectTransform>().sizeDelta.x;
        BoxSize = (backgroundSize + gameSize * 1.5f) / gameSize;
        Plane.GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);
        BoxSize = Plane.GetComponent<RectTransform>().sizeDelta.x;
        float PartOfBoxSize = (BoxSize * 0.1f);
        background.GetComponent<RectTransform>().sizeDelta += new Vector2(PartOfBoxSize, PartOfBoxSize);
        BoxSize = BoxSize - PartOfBoxSize;
        Plane.GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);

        float posx = BoxSize / 2 + PartOfBoxSize;
        float posy = -BoxSize / 2 - PartOfBoxSize;

        for (int x = 0; x < gameSize; x++)
        {
            for (int y = 0; y < gameSize; y++)
            {
                var plane = Instantiate(Plane, new Vector2(posx, posy), Quaternion.identity);
                plane.transform.SetParent(back, false);
                plane.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                plane.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                plane.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

                var Thebox = Instantiate(box, new Vector2(posx, posy), Quaternion.identity);
                Thebox.transform.SetParent(front, false);
                Thebox.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                Thebox.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                Thebox.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                Thebox.gameObject.SetActive(false);
                grid.pointsArray[x, y].pos = new Vector2(posx,posy);
                grid.pointsArray[x, y].box = Thebox;
                grid.pointsArray[x, y].value = -1;

                posy -= BoxSize + PartOfBoxSize - 2;
            }
            posy = -BoxSize / 2 - PartOfBoxSize;

            posx += BoxSize + PartOfBoxSize - 2;
        }
    }

    public void SpawnBox()
    {
        Debug.Log("spawn");
        if  (allowSpawn &&!IsBricksMoving())
        {
            allowSpawn = false;
            for (int i = 0; i < 1; )
            {
                int Randx = Random.Range(0, gameSize);
                int Randy = Random.Range(0, gameSize);

                if (grid.pointsArray[Randx, Randy].value == -1)
                {
                    GameObject textgObj =  grid.pointsArray[Randx, Randy].box.transform.GetChild(0).gameObject;
                    Text text = textgObj.GetComponent<Text>();
                    text.text = "2";
                    text.color = new Color(0f, 0f, 0f);
                    grid.pointsArray[Randx, Randy].value = 2;

                    grid.pointsArray[Randx, Randy].box.gameObject.GetComponent<RectTransform>().anchoredPosition = grid.pointsArray[Randx, Randy].pos;
                    grid.pointsArray[Randx, Randy].box.gameObject.SetActive(true);
                    
                    SpawnX = Randx;
                    SpawnY = Randy;
                    IncreaseBoxSize = true;
                    break;
                }
            }
            CountFreeGrids();
            checkSurrounding();

            previousStep.Clear();
            previousStep.AddRange(previousStepTemp);
            previousStepTemp.Clear();

            NumbOfSpawns++;

            if (NumbOfSpawns > 150)
            {
                AdsManager.instance.ShowAdd();
            }
            if (NumberOfFreeGrids != NumberOfGrids -1) 
                SaveGame();

        }
    }

    private bool IsBricksMoving()
    {
        int moving = 0;
        for (int x = 0; x < gameSize; x ++)
        {
            for ( int y= 0; y < gameSize; y ++)
            {
                if (grid.pointsArray[x,y].moving == false)
                {
                    moving++;
                }
            }
        }

        if (moving == NumberOfGrids)
        {
            allowMove = true;
            return false;
        }
        else
            return true;
    }

    private void CountFreeGrids()
    {
        NumberOfFreeGrids = 0;
        for (int x = 0; x < gameSize; x++)
        {
            for (int y = 0; y  < gameSize; y++)
            {
                if (grid.pointsArray[x,y].value ==-1)
                {
                    NumberOfFreeGrids++;
                }
            }
        }
    }

    private void checkSurrounding()
    {
        twoMatchingOnX = false;
        twoMatchingOnY = false;
        for (int x = 0; x < gameSize; x++)
        {
            for (int y = 0; y < gameSize; y++)
            {
                checkEachPointSurrounding(x, y);
            }
        }
        checkGameOver();
    }

    private void checkEachPointSurrounding(int x, int y)
    {
        for (int i = x - 1; i <= x + 1; i++)
        {
            if (i >= 0 && i < gameSize && i != x)
            {
                if (grid.pointsArray[i, y].value != -1 && grid.pointsArray[x, y].value != -1)
                {
                    if (grid.pointsArray[i, y].value == grid.pointsArray[x, y].value)
                    {
                        twoMatchingOnX = true;
                        break;
                    }
                }

            }
        }

        for (int i = y - 1; i <= y + 1; i++)
        {
            if (i >= 0 && i < gameSize && i != y)
            {
                if (grid.pointsArray[x, i].value != -1 && grid.pointsArray[x, y].value != -1)
                {
                    if (grid.pointsArray[x, i].value == grid.pointsArray[x, y].value)
                    {
                        twoMatchingOnY = true;
                        break;
                    }
                }
            }
        }
        
    }

    private void checkGameOver()
    {
        if ((NumberOfFreeGrids == 0 && !twoMatchingOnY) && (NumberOfFreeGrids == 0 && !twoMatchingOnX))
        {
            GameOver.gameObject.SetActive(true);
        }
    }

    private void movePlateDown()
    {
        
        for (int x = 0; x < gameSize; x++)
        {
            for (int y = gameSize - 1; y >= 0; y--)
            {
                for (int j = y -1 ; j >= 0; j--)
                {
                    if (grid.pointsArray[x, j].value == -1)
                    {
                        continue;
                    }
                    else
                    {
                        if (grid.pointsArray[x, y].value != -1)
                        {
                            if (grid.pointsArray[x, j].value == grid.pointsArray[x, y].value)
                            {
                                switchGridsValuesY(0, x, y, j, 0);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            switchGridsValuesY(-1, x, y, j, 0);
                        }
                    }

                }
            }

        }
    }

    private void movePlateUp()
    {
        for (int x = 0; x < gameSize; x++)
        {
            for (int y = 0 ; y < gameSize; y++)
            {
                for (int j = y + 1; j < gameSize; j++)
                {
                    if (grid.pointsArray[x, j].value == -1)
                    {
                        continue;
                    }
                    else
                    {
                        if (grid.pointsArray[x, y].value != -1)
                        {
                            if (grid.pointsArray[x, j].value == grid.pointsArray[x, y].value)
                            {
                                switchGridsValuesY(0, x, y, j, 1);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            switchGridsValuesY(-1, x, y, j, 1);
                        }
                    }

                }
            }
        }
    }

    private void movePlateRight()
    {
        for (int y = 0; y < gameSize; y++)
        {
            for (int x = gameSize - 1; x >=0; x--)
            {
                for (int j = x - 1; j >= 0; j--)
                {
                    if (grid.pointsArray[j, y].value == -1)
                    {
                        continue;
                    }
                    else
                    {
                        if (grid.pointsArray[x, y].value != -1)
                        {
                            if (grid.pointsArray[j, y].value == grid.pointsArray[x, y].value)
                            {
                                switchGridsValuesX(0, x, y, j, 2);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            switchGridsValuesX(-1, x, y, j, 2);
                        }
                    }

                }
            }
        }
    }

    private void movePlateLeft()
    {
        for (int y = 0; y < gameSize; y++)
        {
            for (int x = 0; x < gameSize; x++)
            {
                for (int j = x + 1; j < gameSize; j++)
                {
                    if (grid.pointsArray[j, y].value == -1)
                    {
                        continue;
                    }
                    else
                    {
                        if (grid.pointsArray[x, y].value != -1)
                        {
                            if (grid.pointsArray[j, y].value == grid.pointsArray[x, y].value)
                            {
                                switchGridsValuesX(0, x, y, j, 3);
                                break;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            switchGridsValuesX(-1, x, y, j, 3);
                        }
                    }
                }
            }
        }
    }

    private void switchGridsValuesY(int value, int x , int y , int  j, int direction)
    {
        bool increamented = false;
        Image boxTemp;
        if (value == -1)
        {
            grid.pointsArray[x, y].value += 1 + grid.pointsArray[x, j].value;
        }
        else
        {
            grid.pointsArray[x, y].value += grid.pointsArray[x, j].value;
            setScore(grid.pointsArray[x, y].value);
            increamented = true;
        }

        boxTemp = grid.pointsArray[x, y].box;
        grid.pointsArray[x, y].box = grid.pointsArray[x, j].box;
        grid.pointsArray[x, j].box = boxTemp;

        MovePlateToDestenation(grid.pointsArray[x, y].box, grid.pointsArray[x, y].pos, grid.pointsArray[x, y].value, direction, x, y,j,increamented);
        grid.pointsArray[x, j].value = -1;

    }

    private void switchGridsValuesX(int value, int x, int y, int j, int direction)
    {
        bool increamented = false;
        Image boxTemp;
        if (value == -1)
        {
            grid.pointsArray[x, y].value += 1 + grid.pointsArray[j, y].value;
        }
        else
        {
            grid.pointsArray[x, y].value += grid.pointsArray[j, y].value;
            setScore(grid.pointsArray[x, y].value);
            increamented = true;
        }

        boxTemp = grid.pointsArray[x, y].box;
        grid.pointsArray[x, y].box = grid.pointsArray[j, y].box;
        grid.pointsArray[j, y].box = boxTemp;

        MovePlateToDestenation(grid.pointsArray[x, y].box, grid.pointsArray[x, y].pos, grid.pointsArray[x, y].value, direction, x, y,j, increamented);
        grid.pointsArray[j, y].value = -1;

    }

    private void MovePlateToDestenation(Image box, Vector2 to, int value, int direction, int x, int y,int j,bool incremented)
    {
        grid.pointsArray[x, y].moving = true;

        if (PrevScore != tempPreviousScore)
        {
            PrevScore = tempPreviousScore;
        }

        Box boxScript = box.GetComponent<Box>();
        boxScript.to = to;
        boxScript.value = value;
        boxScript.x = x;
        boxScript.y = y;
        boxScript.j = j;
        boxScript.increamented = incremented;

        switch (direction)
        {
            case 0:
                boxScript.allowMoveDown = true;
                break;
            case 1:
                boxScript.allowMoveUp = true;
                break;
            case 2:
                boxScript.allowMoveRight = true;
                break;
            case 3:
                boxScript.allowMoveLeft = true;
                break;
        }
        allowSpawn = true;
        allowMove = false;
    }

    private void setScore(int value)
    {
        score += value;
        ScoreTxt.text = "" + score;
        PlayerPrefs.SetInt("score", score);
        if (score > Highscore)
        {
            Highscore = score;
            HightScoreTxt.text = ""+Highscore;
            switch (gameSize)
            {
                case 3:
                    PlayerPrefs.SetInt("highScore3", Highscore);
                    break;
                case 4:
                    PlayerPrefs.SetInt("highScore4", Highscore);
                    break;
                case 5:
                    PlayerPrefs.SetInt("highScore5", Highscore);
                    break;
                case 6:
                    PlayerPrefs.SetInt("highScore6", Highscore);
                    break;
                case 8:
                    PlayerPrefs.SetInt("highScore8", Highscore);
                    break;
            }

        }
    }

    private void SetHighScore()
    {
        GameObject textgObj = box.transform.GetChild(0).gameObject;
        Text text = textgObj.GetComponent<Text>();
        switch (gameSize)
        {
            case 3:
                text.fontSize = (int)BoxSize;
                Highscore = PlayerPrefs.GetInt("highScore3", 0);
                break;
            case 4:
                text.fontSize = (int)BoxSize;
                Highscore = PlayerPrefs.GetInt("highScore4", 0);
                break;
            case 5:
                text.fontSize = (int)BoxSize;
                Highscore = PlayerPrefs.GetInt("highScore5", 0);
                break;
            case 6:
                text.fontSize = (int)BoxSize;
                Highscore = PlayerPrefs.GetInt("highScore6", 0);
                break;
            case 8:
                text.fontSize = (int)BoxSize;
                Highscore = PlayerPrefs.GetInt("highScore8", 0);
                break;
        }

        HightScoreTxt.text = "" + Highscore;
    }
    
    public void LoadPrevious()
    {
        score = PrevScore;
        setScore(0); 

        GameOver.gameObject.SetActive(false);

        for (int x = 0; x < gameSize; x++)
        {
            for (int y = 0; y < gameSize; y++)
            {
                if (grid.pointsArray[x, y].value != -1)
                {
                    grid.pointsArray[x, y].value = -1;
                    grid.pointsArray[x, y].box.gameObject.SetActive(false);
                    grid.pointsArray[x, y].box.GetComponent<RectTransform>().sizeDelta = new Vector2(35, 35);
                    GameObject textgObj = grid.pointsArray[x, y].box.transform.GetChild(0).gameObject;
                    Text text = textgObj.GetComponent<Text>();
                    text.text = "2";
                    box.GetComponent<Image>().color = new Color(0.964f, 0.945f, 0.898f);
                }
            }
        }

        InstantiateBoxes(previousStep);
        checkSurrounding();
    }
    
    private List<previousData> StorePreviousTemp()
    {
        temp.Clear();
        for (int x = 0; x < gameSize; x++)
        {
            for (int y = 0; y < gameSize; y++)
            {
                if (grid.pointsArray[x, y].value != -1)
                {
                    previousData pre = new previousData(x, y, grid.pointsArray[x, y].value);
                    temp.Add(pre);
                }
            }
        }
        tempPreviousScore = score;
        return temp;
    }

    private void InstantiateBoxes(List<previousData> BoxList)
    {
        for (int i = 0; i < BoxList.Count; i++)
        {
            GameObject box = grid.pointsArray[BoxList[i].x, BoxList[i].y].box.gameObject;

            box.SetActive(true);
            box.GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);
            GameObject textgObj =box.transform.GetChild(0).gameObject;
            Text text = textgObj.GetComponent<Text>();
            text.text = "" + BoxList[i].value;

            Color boxImageColor = new Color(237, 224, 200);
            switch (BoxList[i].value)
            {
                case 2:
                    boxImageColor = new Color(0.964f, 0.945f, 0.898f);
                    break;
                case 4:
                    boxImageColor = new Color(1f, 0.964f, 0.874f);
                    break;
                case 8:
                    boxImageColor = new Color(0.949f, 0.694f, 0.474f);
                    text.color = new Color(1f, 1f, 1f);
                    break;
                case 16:
                    boxImageColor = new Color(0.960f, 0.584f, 0.388f);
                    text.color = new Color(1f, 1f, 1f);
                    break;
                case 32:
                    boxImageColor = new Color(0.964f, 0.486f, 0.372f);
                    text.color = new Color(1f, 1f, 1f);
                    break;
                case 64:
                    boxImageColor = new Color(0.964f, 0.368f, 0.231f);
                    text.color = new Color(1f, 1f, 1f);
                    break;
                case 128:
                    boxImageColor = new Color(0.929f, 0.811f, 0.447f);
                    text.color = new Color(1f, 1f, 1f);
                    break;
                case 256:
                    boxImageColor = new Color(0.929f, 0.8f, 0.380f);
                    text.color = new Color(1f, 1f, 1f);
                    break;
                case 512:
                    boxImageColor = new Color(0.929f, 0.784f, 0.313f);
                    text.color = new Color(1f, 1f, 1f);
                    break;
                case 1024:
                    boxImageColor = new Color(0.929f, 0.772f, 0.247f);
                    text.color = new Color(1f, 1f, 1f);
                    break;
                case 2048:
                    boxImageColor = new Color(0.929f, 0.760f, 0.180f);
                    text.color = new Color(1f, 1f, 1f);
                    break;
                default:
                    boxImageColor = new Color(0.933f, 0.894f, 0.854f);
                    break;
            }
            box.GetComponent<Image>().color = boxImageColor;
            grid.pointsArray[BoxList[i].x, BoxList[i].y].value = BoxList[i].value;
        }
    }

    private void SaveGame()
    {
        CurrentData.Clear();
        CurrentData.AddRange(StorePreviousTemp());

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file3 = File.Create(Application.persistentDataPath + "/SaveGame"+gameSize+".dat");
        bf.Serialize(file3, CurrentData);
        file3.Close();
    }

    private void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveGame" + gameSize + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveGame"+gameSize+".dat", FileMode.Open);
            List<previousData> savedGames = (List<previousData>)bf.Deserialize(file);
            file.Close();

            if (savedGames.Count > 0)
                InstantiateBoxes(savedGames);
            else
                SpawnBox();
        }
        else
        {
            SpawnBox();
        }
    }

}
