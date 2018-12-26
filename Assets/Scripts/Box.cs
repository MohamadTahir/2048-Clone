using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    public Vector3 to;
    public GameObject box;
    public int value, x, y;
    public bool allowMoveDown = false, allowMoveUp = false , allowMoveRight = false, allowMoveLeft = false;
    
    private Vector3 RectPos;
    private float BoxSize;
    private Transform background;

    void Start()
    {
        RectPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        background = InGameManager.instance.background;
        BoxSize = InGameManager.instance.BoxSize;
        gameObject.GetComponent<Animator>().enabled = false;
    }
    


    private void LateUpdate()
    {
        if (allowMoveDown)
        {
            
            if (RectPos.y > to.y+20)
            {
                RectPos.y -= 20;
                gameObject.GetComponent<RectTransform>().anchoredPosition = RectPos;
            }
            else
            {
                ReachedPoint();
                allowMoveDown = false;
            }
        }

        if (allowMoveUp)
        {

            if (RectPos.y < to.y - 20)
            {
                RectPos.y += 20;
                gameObject.GetComponent<RectTransform>().anchoredPosition = RectPos;
            }
            else
            {
                ReachedPoint();
                allowMoveUp = false;
            }
        }

        if (allowMoveRight)
        {
            if (RectPos.x <= to.x - 20)
            {
                RectPos.x +=20;
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
            if (RectPos.x >= to.x + 20)
            {
                RectPos.x -= 20;
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

        //InGameManager.instance.SpawnBox();
        gameObject.GetComponent<RectTransform>().anchoredPosition = to;
        RectPos = to;
        GameObject textgObj = transform.GetChild(0).gameObject;
        Text text = textgObj.GetComponent<Text>();
        
        if (int.Parse(text.text) < value)
        {
            gameObject.GetComponent<Animator>().enabled = true;

            Destroy(box);
        }
        text.text = "" + value;

        Color boxImageColor = new Color(237, 224, 200);
        switch (value)
        {
            case 4:
                boxImageColor = new Color(237, 224, 200);
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
                boxImageColor = new Color(237, 200, 80);
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
        gameObject.GetComponent<Image>().color = boxImageColor;
    }

    public void DestroyJbox()
    {
        gameObject.GetComponent<Animator>().enabled = false;
    }
}

