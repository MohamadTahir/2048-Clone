using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public static int GameSize;

    public Image Box;
    public GameObject background;
    void Start()
    {
        GameSize = GameManager.Instance.GameSize;
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid grid = new grid();

        float backgroundSize = background.GetComponent<RectTransform>().sizeDelta.x;
        float BoxSize = (backgroundSize + GameSize*1.5f) / GameSize;
        Box.GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);
        BoxSize = Box.GetComponent<RectTransform>().sizeDelta.x;
        float PartOfBoxSize = (BoxSize * 0.1f);
        background.GetComponent<RectTransform>().sizeDelta += new Vector2(PartOfBoxSize, PartOfBoxSize);
        BoxSize = BoxSize - PartOfBoxSize;
        Box.GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);

        float posx = BoxSize / 2 + PartOfBoxSize;
        float posy = -BoxSize /2 - PartOfBoxSize;

        for (int x = 0; x < GameSize; x++)
        {
            for (int y = 0; y < GameSize; y++)
            {
                var box = Instantiate(Box, new Vector2(posx, posy), Quaternion.identity);
                box.transform.SetParent(background.transform,false);
                box.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
                box.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
                box.GetComponent<RectTransform>().pivot = new Vector2(0.5f , 0.5f);
                posy -= BoxSize + PartOfBoxSize  -2;
            }
            posy = -BoxSize / 2- PartOfBoxSize ;

            posx += BoxSize + PartOfBoxSize - 2;
        }
    }

    public class grid
    {

        public points[,] pointsArray;

        public grid()
        {
            int index = 0;
            pointsArray = new points[GameSize, GameSize];

            for (int x = 0; x < GameSize; x++)
            {
                for (int y = 0; y < GameSize; y++)
                {
                    index++;
                    pointsArray[x, y] = new points(index, -1);
                }
            }
        }
    }

    public class points
    {
        public int index;
        public int value;
        public points(int _index, int _value)
        {
            index = _index;
            value = _value;
        }
    }
}
