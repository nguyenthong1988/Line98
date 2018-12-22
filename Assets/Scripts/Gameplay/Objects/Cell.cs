using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : ObjectBound
{
    public enum State { None, Block, Ball }

    public State CellState = State.None;

    [Header("Sprite")]
    public Sprite SpriteLight;
    public Sprite SpriteDark;
    public Vector2Int Index { get; set; }
    public Ball Ball { get; set; }

    [Header("Highlight")]
    public GameObject HightLight;

    protected SpriteRenderer mSpriteRenderer;

    protected override void Initialize()
    {
        base.Initialize();

        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public string GetDebugString()
    {
        return string.Format("[{0},{1}]", Index.x, Index.y);
    }

    public void SetColorLight()
    {
        if (mSpriteRenderer && SpriteLight) mSpriteRenderer.sprite = SpriteLight;
    }

    public void SetColorDark()
    {
        if (mSpriteRenderer && SpriteDark) mSpriteRenderer.sprite = SpriteDark;
    }

    public void AttachBall(Ball ball)
    {
        if (!ball) return;

        if(Ball != null) DettachBall().Destroy();

        Ball = ball;
        Ball.transform.position = transform.position;
        Ball.gameObject.SetActive(true);
    }

    public Ball DettachBall()
    {
        Ball ball = Ball;
        Ball = null;
        return ball;
    }

    public void SetBallSize(Ball.Size size)
    {
        if (Ball == null) return;
        Ball.SetSize(size);
    }

    public Ball.Color BallColor
    {
        get
        {
            if (!Ball) return Ball.Color.None;
            return Ball.BallColor;
        }
    }

    public bool IsEmpty
    {
        get
        {
            return Ball == null && CellState < State.Block;
        }
    }

    public bool IsSelectable
    {
        get 
        {
            return Ball != null && Ball.BallSize == Ball.Size.Ball;
        }
    }

    public bool IsAvailable
    { 
        get
        {
            return !Ball || Ball.BallSize <= Ball.Size.Dot;
        }
    }

    public void OnHover(bool isHover)
    {
        HightLight.SetActive(isHover);
    }

    //Ball state
    public void ChangeBallState(Ball.State state)
    {
        if (state == Ball.State.Idle && Ball != null) Ball.Idle();
        else if (state == Ball.State.Selected && Ball != null) Ball.Selected();
    }
}
