using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WidgetBallCount : MonoBehaviour
{
    public Image BallImage;
    public TextMeshProUGUI ScoreText;
    public Ball.Color Color;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public Vector3 HitPointPosition
    {
        get
        {
            if (BallImage) return BallImage.rectTransform.position;

            return transform.position;
        }
    }
}
