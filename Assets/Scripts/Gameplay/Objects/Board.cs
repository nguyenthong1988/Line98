using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board : MonoBehaviour, IEvent<GameCommandEvent>
{
    public const int BOARD_SIZE = 9;
    public const int POINT_TO_SCORE = 5;

    protected Cell[,] mCells = new Cell[BOARD_SIZE, BOARD_SIZE];

    public Cell CellTemplate;
    public Ball BallTemplate;

    protected Cell mSelectedCell = null;

    protected List<Vector2Int> mPointsToCheck;

    protected bool mIsMoving;
    protected bool mIsTouchable = true;

    void Awake() => Initialize();

    protected virtual void Initialize()
    {
        mPointsToCheck = new List<Vector2Int>();
    }

    void Start()
    {
        CreateBoard();
        AddBalls(5, GameManager.Instance.NumOfKindBall, Ball.Size.Ball);
        AddBalls(3, GameManager.Instance.NumOfKindBall, Ball.Size.Dot);
    }

    void Update()
    {

    }

    protected virtual void OnEnable()
    {
        GameInput.RegisterListener(GameInput.TouchType.TouchDown, OnTouchDown);
        GameInput.RegisterListener(GameInput.TouchType.TouchMove, OnTouchMove);
        GameInput.RegisterListener(GameInput.TouchType.TouchUp, OnTouchUp);

        GameEvent.AddListener<GameCommandEvent>(this);
    }

    protected virtual void OnDisable()
    {
        GameInput.UnRegisterListener(GameInput.TouchType.TouchDown, OnTouchDown);
        GameInput.UnRegisterListener(GameInput.TouchType.TouchMove, OnTouchMove);
        GameInput.UnRegisterListener(GameInput.TouchType.TouchUp, OnTouchUp);

        GameEvent.RemoveListener<GameCommandEvent>(this);
    }

    public void OnTouchDown(Vector3 postion)
    {
        if (!mIsTouchable) return;
        if (mSelectedCell)
        {
            List<Vector3> path;
            Cell cell = GetMovableCell(postion, out path);
            if (cell)
            {
                cell.Ball = mSelectedCell.Ball;
                cell.Ball.Idle();
                //cell.Ball.transform.position = cell.transform.position;

                CheckBoard();
                AddBalls(3, GameManager.Instance.NumOfKindBall, Ball.Size.Dot);
                mPointsToCheck.Add(cell.Index);

                if(path != null || path.Count > 0)
                {
                    cell.Ball.Move(path, OnMoveDone);
                }
                if (mSelectedCell.Ball) mSelectedCell.Ball.Idle();
                mSelectedCell.Empty();
            }
            else
            {
                if (mSelectedCell.Ball) mSelectedCell.Ball.Idle();
            }

            mSelectedCell = null;
        }
        else
        {
            mSelectedCell = SelectCell(postion);
            if (mSelectedCell) mSelectedCell.Ball.Selected();
        }
    }

    protected void ExplodeBall(Vector2Int cellIndex)
    {
        foreach (Cell cell in mCells)
        {
            if (cell.Index.x == cellIndex.x && cell.Index.y == cellIndex.y)
            {
                Ball ball = cell.Ball;
                if (ball)
                {
                    ball.Explode();
                    GameEvent.TriggerEvent<GameScoreEvent>(new GameScoreEvent(GamePlay.GameScore.Add, 1));
                }

                cell.Ball = null;
            }
        }
    }

    protected Cell GetEmptyCell(Vector3 postion)
    {
        foreach (Cell cell in mCells)
        {
            if (cell.Bounds.Contains(postion) && cell.IsEmpty)
            {
                Debug.Log("Get empty cell " + cell.GetDebugString());
                var path = CheckPath(mCells, mSelectedCell.Index, cell.Index);
                string debugString = "";
                List<Vector2> m = new List<Vector2>();
                foreach(var node in path)
                {
                    debugString += string.Format("{0}|{1} - ", node.x, node.y);
                }
                Debug.Log(debugString);

                return cell;
            }
        }

        return null;
    }

    protected Cell GetMovableCell(Vector3 position, out List<Vector3> path)
    {
        foreach (Cell cell in mCells)
        {
            if (cell.Bounds.Contains(position) && cell.IsEmpty)
            {
                Debug.Log("Get empty cell " + cell.GetDebugString());
                path = CheckPath(mCells, mSelectedCell.Index, cell.Index);
                string debugString = "";
                foreach (var node in path)
                {
                    debugString += string.Format("{0}|{1} - ", node.x, node.y);
                }
                Debug.Log(debugString);

                return (path != null && path.Count > 0) ? cell : null;
            }
        }

        path = null;
        return null;
    }

    protected Cell SelectCell(Vector3 postion)
    {
        foreach (Cell cell in mCells)
        {
            if (cell.Bounds.Contains(postion) && cell.IsSelectable)
            {
                Debug.Log("Select cell " + cell.GetDebugString());
                return cell;
            }
        }

        return null;
    }

    public void CheckBoard()
    {
        foreach (Cell cell in mCells)
        {
            Ball ball = cell.Ball;
            if (ball && ball.BallSize == Ball.Size.Dot)
            {
                mPointsToCheck.Add(new Vector2Int(cell.Index.x, cell.Index.y));
                ball.SetSize(Ball.Size.Ball);
            }
        }
    }

    public void OnTouchMove(Vector3 postion)
    {

    }

    public void OnTouchUp(Vector3 postion)
    {

    }

    public void CreateBoard()
    {
        if (CellTemplate == null) return;

        float offsetX = -3, offsetY = 3;

        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                mCells[i, j] = Instantiate(CellTemplate, transform);
                mCells[i, j].transform.position = new Vector3(offsetX, offsetY);
                mCells[i, j].Index = new Vector2Int(i, j);
                if ((i * BOARD_SIZE + j) % 2 != 0)
                    mCells[i, j].SetColorLight();
                else
                    mCells[i, j].SetColorDark();
                offsetX += 0.75f;
            }

            offsetY -= 0.75f;
            offsetX = -3;
        }
    }


    public void CleanBoard()
    {
        if (mCells.Length == 0) return;

        foreach (Cell cell in mCells)
        {
            if (cell.Ball) cell.Ball.Destroy();

            cell.Ball = null;
        }

    }

    public void LoadFromSave()
    {
        if (mCells.Length == 0) return;

        SaveManager.Instance.LoadBoardSave(mCells);
    }

    public void OnMoveDone()
    {
        Debug.Log("mOnMoveDone");

        if (mPointsToCheck.Count > 0)
        {
            foreach (var point in mPointsToCheck)
            {
                List<Vector2Int> points = CheckLines(point);

                if (points != null && points.Count > 0)
                {
                    foreach (Vector2Int p in points)
                    {
                        ExplodeBall(p);
                    }
                }
            }

            mPointsToCheck.Clear();
        }
    }

    protected bool IsInside(int x, int y)
    {
        return 0 <= x && BOARD_SIZE > x && 0 <= y && BOARD_SIZE > y;
    }

    public void AddBalls(int numOfBall, int numOfType, Ball.Size size)
    {
        List<Cell> emptyCells = new List<Cell>();
        foreach (Cell cell in mCells)
        {
            if (cell.IsEmpty) emptyCells.Add(cell);
        }

        System.Random rnd = new System.Random();
        List<Cell> randomCells = emptyCells.OrderBy(x => rnd.Next()).Take(numOfBall).ToList<Cell>();
        List<Ball.Color> colors = new List<Ball.Color>();

        for(int i = 0; i < numOfBall; i++)
        {
            Cell cell = randomCells[i];
            if (cell)
            {
                cell.AddBall(DataManager.Instance.TakeRandomBall(numOfType));
                cell.SetBallSize(size);

                if(cell.Ball && cell.Ball.BallSize == Ball.Size.Dot)
                {
                    colors.Add(cell.Ball.BallColor);
                }
            }
        }

        if (colors.Count > 0) GameEvent.TriggerEvent<BallChangeEvent>(new BallChangeEvent(BallChangeEnum.Change, colors));
    }

    //
    // From here I use an algorithm that I found in internet
    // My weakness is in algorithm
    //
    public List<Vector3> CheckPath(Cell[,] cells, Vector2Int from, Vector2Int to)
    {
        Vector2Int[,] dad = new Vector2Int[BOARD_SIZE, BOARD_SIZE];
        Vector2Int[] queue = new Vector2Int[BOARD_SIZE * BOARD_SIZE];
        Vector2Int[] trace = new Vector2Int[BOARD_SIZE * BOARD_SIZE];

        bool ghostCell = cells[from.x, from.y].BallColor >= Ball.Color.Ghost;

        int[] u = { 1, 0, -1, 0 };
        int[] v = { 0, 1, 0, -1 };

        int fist = 0, last = 0, x, y, i, j, k;
        for (x = 0; x < BOARD_SIZE; x++)
            for (y = 0; y < BOARD_SIZE; y++)
            {
                dad[x, y] = new Vector2Int(-1, -1);
                trace[x * BOARD_SIZE + y] = new Vector2Int(-5, -5);
            } 

        queue[0] = to;
        dad[to.x, to.y].x = -2;

        Vector2Int dir = new Vector2Int();

        while (fist <= last)
        {
            x = queue[fist].x; y = queue[fist].y;
            fist++;
            for (k = 0; k < 4; k++)
            {
                dir.x = x + u[k];
                dir.y = y + v[k];
                if (dir.x == from.x && dir.y == from.y)
                {
                    dad[from.x, from.y] = new Vector2Int(x, y);

                    i = 0;
                    while (true)
                    {
                        trace[i] = from;
                        i++;
                        k = from.x;
                        from.x = dad[from.x, from.y].x;
                        if (from.x == -2) break;
                        from.y = dad[k, from.y].y;
                    }
                    return trace.Where(p => (p.x > -5 && p.y > -5)).Select(pp => cells[pp.x, pp.y].gameObject.transform.position).ToList<Vector3>();
                }

                if (!IsInside(dir.x, dir.y)) continue;

                if(dad[dir.x, dir.y].x == -1 && ((cells[dir.x, dir.y].IsBallMoveable || ghostCell)))
                {
                    last++;
                    queue[last] = dir;
                    dad[dir.x, dir.y] = new Vector2Int(x, y);
                }
            }
        }

        return trace.Where(p => (p.x > -5 && p.y > -5)).Select(pp => cells[pp.x, pp.y].gameObject.transform.position).ToList<Vector3>();
    }

    //
    // End
    //

    public List<Vector2Int> CheckLines(Vector2Int point)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        int x = (int)point.x, y = (int)point.y;
        int[] u = { 0, 1, 1, 1 };
        int[] v = { 1, 0, -1, 1 };
        int i, j, k;

        for (int t = 0; t < 4; t++)
        {
            k = 0; i = x; j = y;
            while (true)
            {
                i += u[t]; j += v[t];
                if (!IsInside(i, j))
                    break;
                if (mCells[i, j].IsEmpty || mCells[i, j].Ball.BallColor != mCells[x, y].Ball.BallColor)
                    break;
                k++;
            }
            i = x; j = y;
            while (true)
            {
                i -= u[t]; j -= v[t];
                if (!IsInside(i, j))
                    break;
                if (mCells[i, j].IsEmpty || mCells[i, j].Ball.BallColor != mCells[x, y].Ball.BallColor)
                    break;
                k++;
            }
            k++;
            if (k >= POINT_TO_SCORE)
                while (k-- > 0)
                {
                    i += u[t]; j += v[t];
                    if (i != x || j != y)
                        list.Add(new Vector2Int(i, j));
                }
        }

        if (list.Count > 0)
        {
            list.Add(new Vector2Int(x, y));
        } 
        else list = null;
        return list;
    }

    public void OnEvent(GameCommandEvent eventType)
    {
        switch(eventType.Command)
        {
            case GamePlay.GameCommand.SaveBoard:
                SaveManager.Instance.SaveBoard(mCells, GameManager.Instance.PlayedTime, GameManager.Instance.GameScore);
                break;
            case GamePlay.GameCommand.LoadBoard:
                CleanBoard();
                LoadFromSave();
                break;
            case GamePlay.GameCommand.ResetBoard:
                CleanBoard();
                AddBalls(5, GameManager.Instance.NumOfKindBall, Ball.Size.Ball);
                AddBalls(3, GameManager.Instance.NumOfKindBall, Ball.Size.Dot);
                break;
            default:
                break;
        }
    }
}

public enum BallChangeEnum { Add, Change }
public struct BallChangeEvent
{
    public BallChangeEnum ChangeType;
    public List<Ball.Color> Colors;

    public BallChangeEvent(BallChangeEnum type, List<Ball.Color> colors)
    {
        ChangeType = type;
        Colors = colors;
    }
}
