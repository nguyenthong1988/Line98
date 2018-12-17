using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class BoardSave
{
    public BoardSave() { }
    public BoardSave(Cell[,] cells, int score = 0 ,float duration = 0, int hardmode = 0)
    {
        if (cells.Length == 0) return;
        Score = score;
        HardMode = hardmode;
        Duration = duration;

        if (CellsState == null) CellsState = new List<int>();
        if (BallsColor == null) BallsColor = new List<int>();
        if (BallsSize == null) BallsSize = new List<int>();
        CellsState.Clear(); BallsColor.Clear(); BallsSize.Clear();

        for (int i = 0; i < Board.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Board.BOARD_SIZE; j++)
            {
                CellsState.Add((int)cells[i, j].CellState);
                BallsColor.Add(cells[i, j].Ball == null ? -1 : (int)cells[i, j].Ball.BallColor);
                BallsSize.Add(cells[i, j].Ball == null ? -1 : (int)cells[i, j].Ball.BallSize);
            }
        }
    }

    public bool MapSaveToBoard(Cell[,] cells)
    {
        if (cells.Length == 0) return false;

        for (int i = 0; i < Board.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Board.BOARD_SIZE; j++)
            {
                if (BallsColor[i * Board.BOARD_SIZE + j] >= 0)
                {
                    cells[i, j].AddBall(DataManager.Instance.TakeBall((Ball.Color)BallsColor[i * Board.BOARD_SIZE + j]));
                    cells[i, j].SetBallSize((Ball.Size)(BallsSize[i * Board.BOARD_SIZE + j]));
                }
            }
        }

        return true;
    }

    [JsonProperty("hard_mode")]
    public int HardMode;

    [JsonProperty("duration")]
    public float Duration;

    [JsonProperty("score")]
    public int Score;

    [JsonProperty("cells_state")]
    public List<int> CellsState;

    [JsonProperty("balls_color")]
    public List<int> BallsColor;

    [JsonProperty("balls_type")]
    public List<int> BallsSize;
}
