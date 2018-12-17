using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataManager : Singleton<DataManager>
{
    public int BallPoolSize;
    public int GhostBallRate;
    Dictionary<Ball.Color, Pooler> DataPool = new Dictionary<Ball.Color, Pooler>();
    void Start()
    {
        LoadBalls();
    }

    protected void LoadBalls()
    {
        var balls = Resources.LoadAll<Ball>("Balls");
        foreach (var ball in balls)
        {
            GameObject obj = new GameObject(ball.name);
            Pooler a = obj.AddComponent<Pooler>();
            a.Template = ball;
            a.PoolSize = BallPoolSize;
            a.FillPool();

            DataPool.Add(ball.BallColor, a);
        }
    }

    public Ball TakeRandomBall(int numOfType)
    {
        Ball.Color color = (Ball.Color)UnityEngine.Random.Range(0, numOfType < (int)Ball.Color.Ghost ? numOfType : (int)Ball.Color.Ghost - 1);

        color = UnityEngine.Random.Range(0, 1f) <= (GhostBallRate / 100f) ? Ball.Color.Ghost : color;

        return DataPool[color].GetObject().GetComponent<Ball>();
    }

    public Ball TakeBall(Ball.Color color)
    {
        return DataPool[color].GetObject().GetComponent<Ball>();
    }
}
