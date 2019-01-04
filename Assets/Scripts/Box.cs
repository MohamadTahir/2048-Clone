using UnityEngine;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    public static Box instance;

    public Vector3 to;
    public GameObject box;
    public int value, x, y;
    public bool allowMoveDown = false, allowMoveUp = false , allowMoveRight = false, allowMoveLeft = false;
    
    private Vector3 RectPos;
    private float BoxSize;
    private Transform background;

    void Start()
    {
        instance = this;
        RectPos = gameObject.GetComponent<RectTransform>().anchoredPosition;
        background = InGameManager.instance.background;
        BoxSize = InGameManager.instance.BoxSize;
        gameObject.GetComponent<Animator>().enabled = false;
    }
    
    private void LateUpdate()
    {
        if (allowMoveDown)
        {
            
            if (RectPos.y > to.y+30)
            {
                RectPos.y -= 30;
                gameObject.GetComponent<RectTransform>().anchoredPosition = RectPos;
            }
            else
            {
                ReachedPoint();
                allowMoveDown = false;
            }
        }

        else if (allowMoveUp)
        {

            if (RectPos.y < to.y - 30)
            {
                RectPos.y += 30;
                gameObject.GetComponent<RectTransform>().anchoredPosition = RectPos;
            }
            else
            {
                ReachedPoint();
                allowMoveUp = false;
            }
        }

        else if (allowMoveRight)
        {
            if (RectPos.x <= to.x - 30)
            {
                RectPos.x += 30;
                gameObject.GetComponent<RectTransform>().anchoredPosition = RectPos;
            }
            else 
            {
                ReachedPoint();
                allowMoveRight = false;
            }
        }

        else if (allowMoveLeft)
        {
            if (RectPos.x >= to.x + 30)
            {
                RectPos.x -= 30;
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
        if(GetComponent<RectTransform>().sizeDelta.x != BoxSize)
            GetComponent<RectTransform>().sizeDelta = new Vector2(BoxSize, BoxSize);

        gameObject.GetComponent<RectTransform>().anchoredPosition = to;
        RectPos = to;


        setBoxTxtAndColor();

        InGameManager.instance.grid.pointsArray[x, y].moving = false;
        InGameManager.instance.SpawnBox();
    }

    private void setBoxTxtAndColor()
    {
        GameObject textgObj = transform.GetChild(0).gameObject;
        Text text = textgObj.GetComponent<Text>();

        Color boxImageColor = new Color(237, 224, 200);
        switch (value)
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
        gameObject.GetComponent<Image>().color = boxImageColor;

        if (int.Parse(text.text) < value)
        {
            gameObject.GetComponent<Animator>().enabled = true;

            Destroy(box);
        }
        text.text = "" + value;
    }

    public void DestroyJbox()
    {
        gameObject.GetComponent<Animator>().enabled = false;
    }
    
}

