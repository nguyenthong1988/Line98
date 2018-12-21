using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallPanel : MonoBehaviour, IEvent<BallChangeEvent>
{
    [Header("Images")]
    public Image BallImage1;
    public Image BallImage2;
    public Image BallImage3;

    [Header("Sprites")]
    public Sprite BallRed;
    public Sprite BallBlue;
    public Sprite BallGreen;
    public Sprite BallCyan;
    public Sprite BallMagenta;
    public Sprite BallYellow;
    public Sprite BallBrown;
    public Sprite BallGhost;

    void Start()
    {
        SetImage(BallImage1, Ball.Color.Red);
        SetImage(BallImage2, Ball.Color.Red);
        SetImage(BallImage3, Ball.Color.Red);
    }

    protected virtual void OnEnable()
    {
        EventDispatcher.AddListener<BallChangeEvent>(this);
    }

    protected virtual void OnDisable()
    {
        EventDispatcher.RemoveListener<BallChangeEvent>(this);
    }

    protected void SetImage(Image image, Ball.Color color)
    {
        if (image == null) return;
        image.gameObject.SetActive(color >= Ball.Color.Red);

        switch (color)
        {
            case Ball.Color.Red:
                image.sprite = BallRed;
                break;
            case Ball.Color.Green:
                image.sprite = BallGreen;
                break;
            case Ball.Color.Blue:
                image.sprite = BallBlue;
                break;
            case Ball.Color.Cyan:
                image.sprite = BallCyan;
                break;
            case Ball.Color.Magenta:
                image.sprite = BallMagenta;
                break;
            case Ball.Color.Yellow:
                image.sprite = BallYellow;
                break;
            case Ball.Color.Brown:
                image.sprite = BallBrown;
                break;
            case Ball.Color.Ghost:
                image.sprite = BallGhost;
                break;
        }
    }

    public void OnEvent(BallChangeEvent ballEvent)
    {
        SetImage(BallImage1, ballEvent.Colors[0]);
        SetImage(BallImage2, ballEvent.Colors[1]);
        SetImage(BallImage3, ballEvent.Colors[2]);
    }
}
