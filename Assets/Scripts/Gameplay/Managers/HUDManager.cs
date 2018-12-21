using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : Singleton<HUDManager>
{
    [Header("Display")]
    public TextMeshProUGUI TextHighScore;
    public TextMeshProUGUI TextScore;
    public TextMeshProUGUI TextPlayedTime;

    public virtual void Initialize()
    { 
        
    }

    public void UpdateScore()
    { 
        if(TextHighScore)
        {
            TextHighScore.text = GameManager.Instance.Player.GetHighScore().ToString("D5");
        }

        if(TextScore)
        {
            TextScore.text = GameManager.Instance.GameScore.ToString("D5");
        }
    }

    public void UpdatePlayedTime()
    { 
        if(TextPlayedTime)
        {
            int playedTime = (int)GameManager.Instance.PlayedTime;
            int minutes = playedTime / 60;
            int seconds = playedTime % 60;
            TextPlayedTime.text = minutes.ToString("D2") + " : " + seconds.ToString("D2");
        }
    }

    public void OnButtonSaveClick()
    {
        EventDispatcher.TriggerEvent<GameCommandEvent>(new GameCommandEvent(GamePlay.GameCommand.SaveBoard, null));
    }

    public void OnButtonLoadeClick()
    {
        EventDispatcher.TriggerEvent<GameCommandEvent>(new GameCommandEvent(GamePlay.GameCommand.LoadBoard, null));
    }

    public void OnButtonResetClick()
    {
        EventDispatcher.TriggerEvent<GameCommandEvent>(new GameCommandEvent(GamePlay.GameCommand.ResetBoard, null));
    }
}
