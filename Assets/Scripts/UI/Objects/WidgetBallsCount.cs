using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WidgetBallsCount : MonoBehaviour, IEvent<GameScoreEvent>
{
    public WidgetBallCount Template;

    protected List<WidgetBallCount> mWidgetsCounter;

    void Start()
    {
        InitWidgets();
    }

    protected virtual void OnEnable()
    {
        EventDispatcher.AddListener<GameScoreEvent>(this);
    }

    protected virtual void OnDisable()
    {
        EventDispatcher.RemoveListener<GameScoreEvent>(this);
    }

    public void InitWidgets()
    {
        if (mWidgetsCounter.IsNullOrEmpty()) mWidgetsCounter = new List<WidgetBallCount>();
        foreach(var widget in mWidgetsCounter)
        {
            Destroy(widget.gameObject);
        }
        mWidgetsCounter.Clear();

        AddWidget(InstantiateWidget(Ball.Color.Red));
        AddWidget(InstantiateWidget(Ball.Color.Green));
        AddWidget(InstantiateWidget(Ball.Color.Blue));

        if(GameManager.Instance.HardMode >= GamePlay.HardMode.Medium)
        {
            AddWidget(InstantiateWidget(Ball.Color.Cyan));
            AddWidget(InstantiateWidget(Ball.Color.Magenta));
            AddWidget(InstantiateWidget(Ball.Color.Yellow));
        }

        if (GameManager.Instance.HardMode >= GamePlay.HardMode.Hard)
        {
            AddWidget(InstantiateWidget(Ball.Color.Brown));
        }

        AddWidget(InstantiateWidget(Ball.Color.Ghost));
    }

    protected WidgetBallCount InstantiateWidget(Ball.Color color)
    {
        WidgetBallCount widget = Instantiate(Template, transform);
        widget.InitWithColor(color);
        return widget;
    }

    protected bool AddWidget(WidgetBallCount widget)
    {
        if (widget) mWidgetsCounter.Add(widget);
        return true;
    }

    public Vector3 GetWidgetPosition(Ball.Color color)
    {
        WidgetBallCount widget = mWidgetsCounter.FirstOrDefault(w => w.BallColor == color);
        if (widget) return widget.HitPointPosition;

        return Vector3.zero;
    }

    public void OnEvent(GameScoreEvent gameScoreEvent)
    {
        switch (gameScoreEvent.Action)
        {
            case GamePlay.GameScore.Add:
                WidgetBallCount widget = mWidgetsCounter.FirstOrDefault(w => w.BallColor == gameScoreEvent.BallColor);
                if (widget)
                {
                    widget.Score++;
                    widget.SetTextScore(widget.Score);
                }
                break;
        }
    }
}
