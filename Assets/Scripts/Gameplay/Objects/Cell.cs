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

    public void SetBallSize(Ball.Size size)
    {
        if (Ball == null) return;
        Ball.SetSize(size);
    }

    public void AddBall(Ball ball)
    {
        if (!ball) return;

        Ball = ball;
        Ball.transform.position = transform.position;
        Ball.gameObject.SetActive(true);
    }

    public Ball.Color BallColor
    {
        get
        {
            if (!Ball) return Ball.Color.None;
            return Ball.BallColor;
        }
    }

    public void Empty()
    {
        Ball = null;
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

    public bool IsBallMoveable
    { 
        get
        {
            return !Ball;
        }
    }
}
