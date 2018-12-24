using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    public Vector3 to;
    public Image boxPref;
    private Transform background;
    public int value;
    public bool allowMoveDown = false;
    public bool allowMoveUp = false;
    public bool allowMoveRight = false;
    public bool allowMoveLeft = false;

    private Vector3 RectPos;
    private GameObject[] boxes;
    void Start()
    {
        RectPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        background = InGameManager.instance.background;
        gameObject.GetComponent<Animator>().enabled = false;
    }
    


    private void LateUpdate()
    {
        if (allowMoveDown)
        {
            
            if (RectPos.y > to.y+40)
            {
                RectPos.y -= 40;
                gameObject.GetComponent<RectTransform>().anchoredPosition = RectPos;
            }
            else
            {
                ReachedPoint();
                allowMoveDown = false;
                /*boxes = GameObject.FindGameObjectsWithTag("box");
                gameObject.GetComponent<RectTransform>().anchoredPosition = to;
                allowMoveDown = false;



                GameObject textgObj = transform.GetChild(0).gameObject;
                Text text = textgObj.GetComponent<Text>();

                if (int.Parse(text.text) < value)
                {
                    text.text = ""+value;
                    gameObject.GetComponent<Animator>().enabled = true;
                }
                else
                {
                    createboxes();
                }


                /*if (int.Parse(valueTxt.text) < value)
                {
                    newBox.GetComponent<Animator>().enabled = true;
                }*/

                //                valueTxt.text = value.ToString();

                /*Color boxImageColor = new Color(237, 224, 200);
                switch (value)
                {
                    case 4:
                        boxImageColor = new Color(237, 224, 200,255f);
                        break;
                    case 8:
                        boxImageColor = new Color(242, 177, 121);
                        break;
                    case 16:
                        boxImageColor = new Color(245, 149, 99);
                        break;
                    case 32:
                        boxImageColor = new Color(246, 124, 95);
                        break;
                    case 64:
                        boxImageColor = new Color(246, 94, 59);
                        break;
                    case 128:
                        boxImageColor = new Color(237, 207, 114);
                        break;
                    case 256:
                        boxImageColor = new Color(237, 204, 97);
                        break;
                    case 512:
                        boxImageColor = new Color(237, 200 ,80);
                        break;
                    case 1024:
                        boxImageColor = new Color(237, 197, 63);
                        break;
                    case 2048:
                        boxImageColor = new Color(237, 194, 46);
                        break;
                    default:
                        boxImageColor = new Color(238, 228, 218);
                        break;
                }
                newBox.GetComponent<Image>().color = boxImageColor;*/
            }
        }

        if (allowMoveUp)
        {

            if (RectPos.y < to.y - 40)
            {
                RectPos.y += 40;
                gameObject.GetComponent<RectTransform>().anchoredPosition = RectPos;
            }
            else
            {
                ReachedPoint();
                allowMoveUp = false;
                /*boxes = GameObject.FindGameObjectsWithTag("box");
                gameObject.GetComponent<RectTransform>().anchoredPosition = to;
                allowMoveDown = false;



                GameObject textgObj = transform.GetChild(0).gameObject;
                Text text = textgObj.GetComponent<Text>();

                if (int.Parse(text.text) < value)
                {
                    text.text = "" + value;
                    gameObject.GetComponent<Animator>().enabled = true;
                }
                else
                {
                    createboxes();
                }*/
            }
        }

        if (allowMoveRight)
        {
            if (RectPos.x < to.x - 40)
            {
                RectPos.x +=40;
                gameObject.GetComponent<RectTransform>().anchoredPosition = RectPos;
            }
            else
            {
                ReachedPoint();
                allowMoveRight = false;
            }
        }

        if (allowMoveLeft)
        {
            if (RectPos.x > to.x + 40)
            {
                RectPos.x -= 40;
                gameObject.GetComponent<RectTransform>().anchoredPosition = RectPos;
            }
            else
            {
                ReachedPoint();
                allowMoveLeft = false;
            }
        }
    }

    private void ReachedPoint()
    {
        boxes = GameObject.FindGameObjectsWithTag("box");
        gameObject.GetComponent<RectTransform>().anchoredPosition = to;

        GameObject textgObj = transform.GetChild(0).gameObject;
        Text text = textgObj.GetComponent<Text>();

        if (int.Parse(text.text) < value)
        {
            text.text = "" + value;
            gameObject.GetComponent<Animator>().enabled = true;
        }
        else
        {
            createboxes();
        }
    }

    public void createboxes()
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            if (boxes[i] == gameObject)
                continue;
            Destroy(boxes[i]);
        }
        InGameManager.instance.InstantiateBoxes();
        InGameManager.instance.SpawnBox();

        Destroy(gameObject);
    }
}

