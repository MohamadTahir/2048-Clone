using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
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
    public GameSaveStatus gameSaveStatus;
    public Transform background;
    public float BoxSize;
    public bool allowMove;
    public List<previousData> CurrentData;

    private float sensitivity;
    private Vector3 directionVector;
    private int SpawnX, SpawnY, Highscore, score, NumberOfFreeGrids=0, NumberOfGrids,PrevScore=0,tempPreviousScore;
    private bool IncreaseBoxSize , twoMatchingOnY, twoMatchingOnX, AllowSpawn;
    private List<previousData> previousStep, previousStepTemp, temp;
    
    private void Start()
    {
        init();
        CreateGrid();
        SetHighScore();
        LoadGame();
    }
    
    private void init()
    {
        instance = this;
        gameSize = GameManager.TheGameSize;
        NumberOfGrids = gameSize * gameSize;
        grid = new Grid();
        gameSaveStatus = new GameSaveStatus();
        previousStepTemp = new List<previousData>();
        previousStep = new List<previousData>();
        CurrentData = new List<previousData>();
        temp = new List<previousData>();
        IncreaseBoxSize = false;
        AllowSpawn = true;
        allowMove = true;
        sensitivity = 50f;
    }

    private void Update()
    {

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary)
        {
            if (allowMove)
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
                previousStepTemp.AddRange(StorePreviousTemp());
                movePlateDown();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                previousStepTemp.AddRange(StorePreviousTemp());
                movePlateUp();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                previousStepTemp.AddRange(StorePreviousTemp());
                movePlateRight();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
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

    [System.Serializable]
    public class GameSaveStatus
    {
        public List<previousData> grid3;
        public List<previousData> grid4;
        public List<previousData> grid5;
        public List<previousData> grid6;
        public List<previousData> grid8;

        public GameSaveStatus()
        {
            grid3 = new List<previousData>();
            grid4 = new List<previousData>();
            grid5 = new List<previousData>();
            grid6 = new List<previousData>();
            grid8 = new List<previousData>();
        }

    }

    public class points
    {
        public int index;
        public int value;
        public Vector2 pos;
        public Image box;

        public points(int _index, int _value, Vector2 _pos,Image _box)
        {
            index = _index;
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
            int index = 0;
            pointsArray = new points[gameSize, gameSize];

            for (int x = 0; x < gameSize; x++)
            {
                for (int y = 0; y < gameSize; y++)
                {
                    index++;
                    pointsArray[x, y] = new points(index, -1, new Vector2(0, 0), null);
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
                var box = Instantiate(Plane, new Vector2(posx, posy), Quaternion.identity);
                box.transform.SetParent(background, false);
                box.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                box.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                box.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                grid.pointsArray[x, y].pos = new Vector2(posx,posy);
                posy -= BoxSize + PartOfBoxSize - 2;
            }
            posy = -BoxSize / 2 - PartOfBoxSize;

            posx += BoxSize + PartOfBoxSize - 2;
        }
    }

    public void SpawnBox()
    {
        if (AllowSpawn)
        {
            AllowSpawn = false;
            for (int i = 0; i < NumberOfGrids; )
            {
                int Randx = Random.Range(0, gameSize);
                int Randy = Random.Range(0, gameSize);

                if (grid.pointsArray[Randx, Randy].value == -1)
                {

                    var Thebox = Instantiate(box, grid.pointsArray[Randx, Randy].pos, Quaternion.identity);
                    Thebox.transform.SetParent(background.transform, false);
                    Thebox.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                    Thebox.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                    Thebox.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                    Thebox.GetComponent<Image>().color = new Color(0.964f, 0.945f, 0.898f);

                    GameObject textgObj = Thebox.transform.GetChild(0).gameObject;
                    Text text = textgObj.GetComponent<Text>();
                    text.text = "2";

                    SpawnX = Randx;
                    SpawnY = Randy;
                    grid.pointsArray[Randx, Randy].value = 2;
                    grid.pointsArray[Randx, Randy].box = Thebox;
                    IncreaseBoxSize = true;
                    break;
                }
            }
            previousStep.Clear();
            previousStep.AddRange(previousStepTemp);
            previousStepTemp.Clear();
            SaveGame();

            CountFreeGrids();
            checkSurrounding();
        }
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
        if (value == -1)
        {
            grid.pointsArray[x, y].value += 1 + grid.pointsArray[x, j].value;
            grid.pointsArray[x, y].box = grid.pointsArray[x, j].box;
            
            MovePlateToDestenation(grid.pointsArray[x, y].box, null, grid.pointsArray[x, y].pos, grid.pointsArray[x, y].value, direction);
        }
        else
        {
            grid.pointsArray[x, y].value += grid.pointsArray[x, j].value;
            setScore(grid.pointsArray[x, y].value);
            MovePlateToDestenation(grid.pointsArray[x, j].box, grid.pointsArray[x, y].box.gameObject, grid.pointsArray[x, y].pos, grid.pointsArray[x, y].value, direction);
            
            grid.pointsArray[x, y].box = grid.pointsArray[x, j].box;
        }
        grid.pointsArray[x, j].value = -1;
        AllowSpawn = true;

    }

    private void switchGridsValuesX(int value, int x, int y, int j, int direction)
    {
        if (value == -1)
        {
            grid.pointsArray[x, y].value += 1 + grid.pointsArray[j, y].value;
            grid.pointsArray[x, y].box = grid.pointsArray[j, y].box;

            MovePlateToDestenation(grid.pointsArray[x, y].box, null, grid.pointsArray[x, y].pos, grid.pointsArray[x, y].value, direction);
        }
        else
        {
            grid.pointsArray[x, y].value += grid.pointsArray[j, y].value;
            setScore(grid.pointsArray[x, y].value);
            MovePlateToDestenation(grid.pointsArray[j, y].box, grid.pointsArray[x, y].box.gameObject, grid.pointsArray[x, y].pos, grid.pointsArray[x, y].value, direction);
            
            grid.pointsArray[x, y].box = grid.pointsArray[j, y].box;
        }
        grid.pointsArray[j, y].value = -1;
        AllowSpawn = true;

    }

    private void MovePlateToDestenation(Image box, GameObject jBox, Vector3 to, int value, int direction)
    {
        if (tempPreviousScore != score)
            PrevScore = tempPreviousScore;

        Box boxScript = box.GetComponent<Box>();
        boxScript.box = jBox;
        boxScript.to = to;
        boxScript.value = value;
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
        allowMove = false;
    }

    private void setScore(int value)
    {
        score += value;
        ScoreTxt.text = "" + score;
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
                    Destroy(grid.pointsArray[x, y].box.gameObject);
                    grid.pointsArray[x, y].box = null;
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
            var Thebox = Instantiate(box, grid.pointsArray[BoxList[i].x, BoxList[i].y].pos, Quaternion.identity);
            Thebox.transform.SetParent(background.transform, false);
            Thebox.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            Thebox.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            Thebox.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
            Thebox.GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);
            GameObject textgObj = Thebox.transform.GetChild(0).gameObject;
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
            Thebox.GetComponent<Image>().color = boxImageColor;

            grid.pointsArray[BoxList[i].x, BoxList[i].y].box = Thebox;
            grid.pointsArray[BoxList[i].x, BoxList[i].y].value = BoxList[i].value;
        }
    }

    private void SaveGame()
    {
        CurrentData.Clear();
        CurrentData.AddRange(StorePreviousTemp());
        
        if (File.Exists(Application.persistentDataPath + "/SaveGame.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveGame.dat", FileMode.Open);
            GameSaveStatus savedGames = (GameSaveStatus)bf.Deserialize(file);
            file.Close();

            FileUtil.DeleteFileOrDirectory(Application.persistentDataPath + "/SaveGame.dat");

            switch (gameSize)
            {
                case 3:
                    savedGames.grid3.Clear();
                    savedGames.grid3.AddRange(CurrentData);
                    break;

                case 4:
                    savedGames.grid8.Clear();
                    savedGames.grid8.AddRange(CurrentData);
                    break;

                case 5:
                    savedGames.grid8.Clear();
                    savedGames.grid8.AddRange(CurrentData);
                    break;

                case 6:
                    savedGames.grid8.Clear();
                    savedGames.grid8.AddRange(CurrentData);
                    break;

                case 8:
                    savedGames.grid8.Clear();
                    savedGames.grid8.AddRange(CurrentData);
                    break;
            }
            
            FileStream saveFile = File.Create(Application.persistentDataPath + "/SaveGame.dat");
            bf.Serialize(saveFile, savedGames);
            saveFile.Close();
        }
        else
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/SaveGame.dat");

            switch (gameSize)
            {
                case 3:
                    gameSaveStatus.grid3.AddRange(CurrentData);
                    break;
                case 4:
                    gameSaveStatus.grid4.AddRange(CurrentData);
                    break;
                case 5:
                    gameSaveStatus.grid5.AddRange(CurrentData);
                    break;
                case 6:
                    gameSaveStatus.grid6.AddRange(CurrentData);
                    break;
                case 8:
                    gameSaveStatus.grid8.AddRange(CurrentData);
                    break;
            }

            bf.Serialize(file, gameSaveStatus);
            file.Close();
        }
    }

    private void LoadGame()
    {
        if (File.Exists(Application.persistentDataPath + "/SaveGame.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/SaveGame.dat", FileMode.Open);
            GameSaveStatus savedGames = (GameSaveStatus)bf.Deserialize(file);
            file.Close();

            switch (gameSize)
            {
                case 3:

                    if (savedGames.grid3.Count > 0)
                        InstantiateBoxes(savedGames.grid3);
                    else
                        SpawnBox();
                    break;

                case 4:
                    
                    if (savedGames.grid4.Count > 0)
                        InstantiateBoxes(savedGames.grid4);
                    else
                        SpawnBox();
                    break;

                case 5:

                    if (savedGames.grid5.Count > 0)
                        InstantiateBoxes(savedGames.grid5);
                    else
                        SpawnBox();
                    break;
                    
                case 6:

                    if (savedGames.grid6.Count > 0)
                        InstantiateBoxes(savedGames.grid6);
                    else
                        SpawnBox();
                    break;

                case 8:

                    if (savedGames.grid8.Count > 0)
                        InstantiateBoxes(savedGames.grid8);
                    else
                        SpawnBox();
                    break;
            }

        }
        else
        {
            SpawnBox();
        }
    }

}
