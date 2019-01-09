using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WidgetBallsCount : MonoBehaviour
{
    public WidgetBallCount Template;

    protected List<WidgetBallCount> mWidgetsCounter;

    void Start()
    {

    }

    void Update()
    {
        
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
    }

    protected WidgetBallCount InstantiateWidget(Ball.Color color)
    {
        WidgetBallCount widget = Instantiate(Template);
        widget.InitWithColor(color);
        return widget;
    }

    protected bool AddWidget(WidgetBallCount widget)
    {
        if (widget) mWidgetsCounter.Add(widget);
        return true;
    }
}
