using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InGameManager : MonoBehaviour
{
    public static int GameSize;

    public Image Plane;
    public Image Box;
    public Transform background;
    public static InGameManager instance;

    private int NumberOfGrids;
    private int NumberOfFreeGrids;
    public Grid grid;

    private int SpawnX;
    private int SpawnY;
    public float BoxSize;
    private bool IncreaseBoxSize;

    private void Start()
    {
        instance = this;
        GameSize = GameManager.Instance.GameSize;
        NumberOfGrids = GameSize * GameSize;
        NumberOfFreeGrids = NumberOfGrids;
        IncreaseBoxSize = false;
        grid = new Grid();


        GameObject textgObj = Box.transform.GetChild(0).gameObject;
        Text text = textgObj.GetComponent<Text>();
        switch (GameSize)
        {
            case 3:
                text.fontSize = 60;
                break;
            case 4:
                text.fontSize = 50;
                break;
            case 5:
                text.fontSize = 40;
                break;
            case 6:
                text.fontSize = 35;
                break;
            case 8:
                text.fontSize = 25;
                break;
        }

        CreateGrid();
        SpawnBox();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            movePlateDown();

            SpawnBox();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            movePlateUp();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            movePlateRight();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movePlateLeft();
        }
    }

    private void LateUpdate()
    {
        if (IncreaseBoxSize)
        {
            Vector2 startSize = grid.pointsArray[SpawnX, SpawnY].box.GetComponent<RectTransform>().sizeDelta;
            startSize.x += 10;
            startSize.y += 10;
            grid.pointsArray[SpawnX, SpawnY].box.GetComponent<RectTransform>().sizeDelta = startSize;
            if (startSize.x >= BoxSize)
            {
                IncreaseBoxSize = false;
                grid.pointsArray[SpawnX, SpawnY].box.GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);
            }

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
            pointsArray = new points[GameSize, GameSize];

            for (int x = 0; x < GameSize; x++)
            {
                for (int y = 0; y < GameSize; y++)
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
        BoxSize = (backgroundSize + GameSize * 1.5f) / GameSize;
        Plane.GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);
        BoxSize = Plane.GetComponent<RectTransform>().sizeDelta.x;
        float PartOfBoxSize = (BoxSize * 0.1f);
        background.GetComponent<RectTransform>().sizeDelta += new Vector2(PartOfBoxSize, PartOfBoxSize);
        BoxSize = BoxSize - PartOfBoxSize;
        Plane.GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);

        float posx = BoxSize / 2 + PartOfBoxSize;
        float posy = -BoxSize / 2 - PartOfBoxSize;

        for (int x = 0; x < GameSize; x++)
        {
            for (int y = 0; y < GameSize; y++)
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
        for (int i =0 ; i < NumberOfGrids; i++)
        {
            int Randx = Random.Range(0, GameSize);
            int Randy = Random.Range(0, GameSize);

            if (grid.pointsArray[Randx,Randy].value == -1)
            {
                
                var box = Instantiate(Box,grid.pointsArray[Randx,Randy].pos, Quaternion.identity);
                box.transform.SetParent(background.transform, false);
                box.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                box.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                box.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                box.GetComponent<Image>().color = new Color(255,255,255);

                GameObject textgObj = box.transform.GetChild(0).gameObject;
                Text text = textgObj.GetComponent<Text>();
                text.text = "2";

                SpawnX = Randx;
                SpawnY = Randy;
                grid.pointsArray[Randx, Randy].value = 2;
                grid.pointsArray[Randx, Randy].box = box;
                IncreaseBoxSize = true;
                break;
            }
        }
    }

    public void InstantiateBoxes()
    {
        for (int x = 0 ; x < GameSize; x++)
        {
            for (int y = 0; y < GameSize; y++)
            {
                if (grid.pointsArray[x,y].value != -1)
                {
                    var box = Instantiate(Box, grid.pointsArray[x, y].pos, Quaternion.identity);
                    box.transform.SetParent(background.transform, false);
                    box.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                    box.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                    box.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
                    box.GetComponent<Image>().color = new Color(255, 255, 255);

                    GameObject textgObj = box.transform.GetChild(0).gameObject;
                    Text text = textgObj.GetComponent<Text>();
                    text.text = ""+ grid.pointsArray[x, y].value;

                    grid.pointsArray[x, y].box=box ;
                    grid.pointsArray[x, y].box.GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);
                    
                }
            }
        }
    }

    private void movePlateDown()
    {
        for (int x = 0; x < GameSize; x++)
        {
            for (int y = GameSize - 2; y >= 0; y--)
            {
                if (grid.pointsArray[x, y].value != -1)
                {
                    for (int j = y+1; j < GameSize ; j++)
                    {
                        if (j != GameSize - 1)
                        {
                            if (grid.pointsArray[x, j + 1].value == -1 || grid.pointsArray[x, j + 1].value == grid.pointsArray[x, y].value)
                            {
                                continue;
                            }
                            else
                            {
                                
                                if (grid.pointsArray[x, j].value == -1)
                                {
                                    switchGridsValuesY(-1,x,y,j,0);
                                    break;
                                }
                                else if (grid.pointsArray[x, j].value == grid.pointsArray[x, y].value)
                                {
                                    switchGridsValuesY(0, x, y, j,0);
                                    
                                    //setScore(grid.pointsgird[x, j].value);
                                    break;

                                }
                            }
                            
                        }
                        else
                        {
                            if (grid.pointsArray[x, j].value == -1)
                            {
                                switchGridsValuesY(-1, x, y, j,0);
                                break;
                            }
                            else if(grid.pointsArray[x, j].value == grid.pointsArray[x, y].value)
                            {
                                if (grid.pointsArray[x, j - 1].value  >0 && grid.pointsArray[x, j].value != grid.pointsArray[x,j-1].value)
                                    break;

                                switchGridsValuesY(0, x, y, j,0);
                                break;

                                //setScore(grid.pointsgird[x, j].value);
                            }
                        }
                    }
                }

            }

        }
    }

    private void movePlateUp()
    {
        for (int x = 0; x < GameSize; x++)
        {
            for (int y = 1 ; y < GameSize ; y++)
            {
                if (grid.pointsArray[x, y].value != -1)
                {
                    for (int j = y - 1; j >= 0 ; j--)
                    {
                        if (j != 0 )
                        {
                            if (grid.pointsArray[x, j - 1].value == -1 || grid.pointsArray[x, j - 1].value == grid.pointsArray[x, y].value)
                            {
                                continue;
                            }
                            else
                            {
                                if (grid.pointsArray[x, j].value == -1)
                                {
                                    switchGridsValuesY(-1, x, y, j,1);
                                    break;
                                }
                                else if (grid.pointsArray[x, j].value == grid.pointsArray[x, y].value)
                                {
                                    switchGridsValuesY(0, x, y, j,1);
                                    //setScore(grid.pointsgird[x, j].value);
                                    break;

                                }
                            }
                        }
                        else
                        {
                            if (grid.pointsArray[x, j].value == -1)
                            {
                                switchGridsValuesY(-1, x, y, j,1);
                                break;
                            }
                            else if (grid.pointsArray[x, j].value == grid.pointsArray[x, y].value)
                            {
                                if (grid.pointsArray[x, j + 1].value > 0 && grid.pointsArray[x, j].value != grid.pointsArray[x, j + 1].value)
                                    break;

                                switchGridsValuesY(0, x, y, j,1);
                                break;

                                //setScore(grid.pointsgird[x, j].value);
                            }
                        }
                    }
                }

            }

        }
    }

    private void movePlateRight()
    {
        for (int x = GameSize -2; x>=0;x--)
        {
            for (int y =0; y < GameSize; y++)
            {
                if (grid.pointsArray[x, y].value != -1)
                {
                    for (int j = x + 1; j < GameSize; j++)
                    {
                        if (j != GameSize - 1)
                        {
                            if (grid.pointsArray[j + 1, y].value == -1 || grid.pointsArray[j + 1, y].value == grid.pointsArray[x, y].value)
                            {
                                continue;
                            }
                            else
                            {
                                if (grid.pointsArray[j, y].value == -1)
                                {
                                    switchGridsValuesX(-1, x, y, j, 2);
                                    break;
                                }
                                else if (grid.pointsArray[j, y].value == grid.pointsArray[x, y].value)
                                {
                                    switchGridsValuesX(0, x, y, j, 2);
                                    //setScore(grid.pointsgird[x, j].value);
                                    break;

                                }
                            }
                        }
                        else
                        {
                            if (grid.pointsArray[j, y].value == -1)
                            {
                                switchGridsValuesX(-1, x, y, j, 2);
                                break;

                            }
                            else if (grid.pointsArray[j, y].value == grid.pointsArray[x, y].value)
                            {
                                if (grid.pointsArray[j - 1, y ].value > 0 && grid.pointsArray[j, y].value != grid.pointsArray[j - 1, y].value)
                                    break;

                                switchGridsValuesX(0, x, y, j, 2);
                                break;

                                //setScore(grid.pointsgird[x, j].value);
                            }
                        }
                    }
                }

            }

        }
    }

    private void movePlateLeft()
    {

        for (int x = 1; x < GameSize; x++)
        {
            for (int y = 0 ; y < GameSize; y++)
            {
                if (grid.pointsArray[x, y].value != -1)
                {
                    for (int j = x - 1; j >=0; j--)
                    {
                        if (j != 0)
                        {
                            if (grid.pointsArray[j - 1, y].value == -1 || grid.pointsArray[j - 1, y].value == grid.pointsArray[x, y].value)
                            {
                                continue;
                            }
                            else
                            {
                                if (grid.pointsArray[j, y].value == -1)
                                {
                                    switchGridsValuesX(-1, x, y, j, 3);

                                    break;
                                }
                                else if (grid.pointsArray[j, y].value == grid.pointsArray[x, y].value)
                                {
                                    switchGridsValuesX(0, x, y, j, 3);

                                    //setScore(grid.pointsgird[x, j].value);
                                    break;

                                }
                            }
                        }
                        else
                        {
                            if (grid.pointsArray[j, y].value == -1)
                            {
                                switchGridsValuesX(-1, x, y, j, 3);

                                break;
                            }
                            else if (grid.pointsArray[j, y].value == grid.pointsArray[x, y].value)
                            {
                                if (grid.pointsArray[j + 1, y].value > 0 && grid.pointsArray[j, y].value != grid.pointsArray[j + 1, y].value)
                                    break;

                                switchGridsValuesX(0, x, y, j, 3);

                                break;

                                //setScore(grid.pointsgird[x, j].value);
                            }
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
            grid.pointsArray[x, j].value += 1 + grid.pointsArray[x, y].value;
            grid.pointsArray[x, j].box = grid.pointsArray[x, y].box;

            MovePlateToDestenation(grid.pointsArray[x, j].box, null, grid.pointsArray[x, j].pos, grid.pointsArray[x, j].value, direction);
        }
        else
        {
            grid.pointsArray[x, j].value += grid.pointsArray[x, y].value;
            MovePlateToDestenation(grid.pointsArray[x, y].box, grid.pointsArray[x, j].box.gameObject, grid.pointsArray[x, j].pos, grid.pointsArray[x, j].value, direction);

            grid.pointsArray[x, j].box = grid.pointsArray[x, y].box;
        }

        grid.pointsArray[x, y].value = -1;

    }

    private void switchGridsValuesX(int value, int x, int y, int j, int direction)
    {
        if (value == -1)
        {
            grid.pointsArray[j, y].value += 1 + grid.pointsArray[x, y].value;
            grid.pointsArray[j, y].box = grid.pointsArray[x, y].box;

            MovePlateToDestenation(grid.pointsArray[j, y].box, null, grid.pointsArray[j, y].pos, grid.pointsArray[j, y].value, direction);
        }
        else
        {
            grid.pointsArray[j, y].value += grid.pointsArray[x, y].value;
            MovePlateToDestenation(grid.pointsArray[j, y].box, grid.pointsArray[j, y].box.gameObject, grid.pointsArray[j, y].pos, grid.pointsArray[j, y].value, direction);

            grid.pointsArray[j, y].box = grid.pointsArray[x, y].box;
        }

        grid.pointsArray[x, y].value = -1;
        
    }

    private void MovePlateToDestenation(Image box, GameObject jBox, Vector3 to, int value, int direction)
    {
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
    }
}
