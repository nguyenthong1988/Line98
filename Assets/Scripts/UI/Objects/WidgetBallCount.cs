using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WidgetBallCount : MonoBehaviour
{
    public Image BallImage;
    public TextMeshProUGUI ScoreText;
    public Ball.Color BallColor;

    public int Score { get; set; }

    public void InitWithColor(Ball.Color color)
    {
        Score = 0;
        BallColor = color;
        if (BallImage)
        {
            BallImage.sprite = Resources.Load<Sprite>("Sprites/Balls/ball_" + BallColor.ToString("g").ToLower());
        }
        SetTextScore(Score);
    }

    public Vector3 HitPointPosition
    {
        get
        {
            if (BallImage) return BallImage.rectTransform.position;

            return transform.position;
        }
    }

    public void SetTextScore(int score)
    {
        if (ScoreText && score >= 0) ScoreText.text = string.Format("x{0}", score);
    }
}
