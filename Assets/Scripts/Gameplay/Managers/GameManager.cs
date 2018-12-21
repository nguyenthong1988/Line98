using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlay
{
    public enum HardMode { Easy, Medium, Hard }
    public enum GameState { None, Initialize, Start, InProgress, Paused, End }
    public enum GameCommand { LoadBoard, SaveBoard, ResetBoard }
    public enum GameScore { Add, Set }
}

public class GameManager : Singleton<GameManager>, IEvent<GameScoreEvent>, IEvent<GameCommandEvent>
{
    public GamePlay.GameState GameState = GamePlay.GameState.None;
    public GamePlay.HardMode HardMode = GamePlay.HardMode.Easy;

    public float PlayedTime { get; protected set; }
    public int GameScore { get; protected set; }
    public Player Player { get; protected set; }

    void Start()
    {
        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.Initialize();
        }

        if (Player == null) Player = new Player();
        HUDManager.Instance.UpdateScore();
    }

    void Update()
    {
        //if(GameState == GamePlay.GameState.InProgress)
        {
            PlayedTime += Time.deltaTime;
            HUDManager.Instance.UpdatePlayedTime();
        }
    }

    protected virtual void OnEnable()
    {
        EventDispatcher.AddListener<GameScoreEvent>(this);
        EventDispatcher.AddListener<GameCommandEvent>(this);
    }

    protected virtual void OnDisable()
    {
        EventDispatcher.RemoveListener<GameScoreEvent>(this);
        EventDispatcher.RemoveListener<GameCommandEvent>(this);
    }

    public int NumOfKindBall
    {
        get 
        {
            if (HardMode > GamePlay.HardMode.Medium) return 7;
            if (HardMode > GamePlay.HardMode.Easy) return 6;

            return 3;
        }
    }

    public void OnEvent(GameScoreEvent gameScoreEvent)
    {
        switch (gameScoreEvent.Action)
        {
            case GamePlay.GameScore.Add:
                GameScore += gameScoreEvent.Score;
                break;
            case GamePlay.GameScore.Set:
                GameScore = gameScoreEvent.Score;
                break;
        }
        Player.SetHighScore(GameScore);
        HUDManager.Instance.UpdateScore();
    }

    public void OnEvent(GameCommandEvent eventType)
    {
        switch (eventType.Command)
        {
            case GamePlay.GameCommand.SaveBoard:
                break;
            case GamePlay.GameCommand.LoadBoard:
                BoardSave boardSave = SaveManager.Instance.GetBoardSave();
                if(boardSave != null)
                {
                    GameScore = boardSave.Score;
                    PlayedTime = boardSave.Duration;
                    HUDManager.Instance.UpdateScore();
                    HUDManager.Instance.UpdatePlayedTime();
                }
                break;
            case GamePlay.GameCommand.ResetBoard:
                GameScore = 0;
                PlayedTime = 0;
                HUDManager.Instance.UpdateScore();
                HUDManager.Instance.UpdatePlayedTime();
                break;
            default:
                break;
        }
    }
}

public struct GameCommandEvent
{
    public GamePlay.GameCommand Command;
    public object Object;

    public GameCommandEvent(GamePlay.GameCommand command, object obj)
    {
        Command = command;
        Object = obj;
    }
}

public struct GameScoreEvent
{
    public GamePlay.GameScore Action;
    public int Score;

    public GameScoreEvent(GamePlay.GameScore action, int score)
    {
        Action = action;
        Score = score;
    }
}
