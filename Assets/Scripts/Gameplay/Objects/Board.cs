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
    protected Ball mSelectedBall = null;

    protected List<Vector2Int> mPointsToCheck;
    protected List<Vector2Int> mMovePath;

    protected bool mIsMoving;
    protected bool mIsTouchable = true;

    protected Vector2Int mPointerIndex = new Vector2Int(0, 0);

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
        GameInput.RegisterTouchEvent(GameInput.InputType.TouchDown, OnTouchDown);
        GameInput.RegisterTouchEvent(GameInput.InputType.TouchMove, OnTouchMove);
        GameInput.RegisterTouchEvent(GameInput.InputType.TouchUp, OnTouchUp);

        GameInput.RegisterKeyEvent(GameInput.InputType.KeyPress, OnKeyDown);

        EventDispatcher.AddListener<GameCommandEvent>(this);
    }

    protected virtual void OnDisable()
    {
        GameInput.UnRegisterTouchEvent(GameInput.InputType.TouchDown, OnTouchDown);
        GameInput.UnRegisterTouchEvent(GameInput.InputType.TouchMove, OnTouchMove);
        GameInput.UnRegisterTouchEvent(GameInput.InputType.TouchUp, OnTouchUp);

        GameInput.UnRegisterKeyEvent(GameInput.InputType.KeyPress, OnKeyDown);

        EventDispatcher.RemoveListener<GameCommandEvent>(this);
    }

    private void OnKeyDown(GameInput.KeyEvent keyEvent)
    {
        switch (keyEvent)
        {
            case GameInput.KeyEvent.DpadUp:
                MovePointer(-1, 0);
                break;
            case GameInput.KeyEvent.DpadDown:
                MovePointer(1, 0);
                break;
            case GameInput.KeyEvent.DpadLeft:
                MovePointer(0, -1);
                break;
            case GameInput.KeyEvent.DpadRight:
                MovePointer(0, 1);
                break;
            case GameInput.KeyEvent.ButtonA:
                SelectCell(mPointerIndex);
                break;
        }

        mCells[mPointerIndex.x, mPointerIndex.y].OnHover(true);
    }

    protected void MovePointer(int offsetx, int offsety)
    {
        Vector2Int nextIndex = mPointerIndex + new Vector2Int(offsetx, offsety);
        if (IsInside(nextIndex))
        {
            mCells[mPointerIndex.x, mPointerIndex.y].OnHover(false);
            mPointerIndex = nextIndex;
            mCells[mPointerIndex.x, mPointerIndex.y].OnHover(true);
        }
    }

    public void SelectCell(Vector2Int pointerIndex)
    { 
        if(mSelectedCell)
        {
            if(mSelectedCell.Index == pointerIndex)
            {
                mSelectedCell.ChangeBallState(Ball.State.Idle);
                mSelectedCell = null;

                return;
            }

            int value = PointToCell(pointerIndex);
            if(value == 0)
            {
                mCells[pointerIndex.x, pointerIndex.y].AttachBall(mSelectedCell.DettachBall());
                mCells[pointerIndex.x, pointerIndex.y].ChangeBallState(Ball.State.Idle);
                mPointsToCheck.Add(pointerIndex);
                MoveBallOnPath(mCells[pointerIndex.x, pointerIndex.y].Ball, mMovePath);
                mSelectedCell = null;
            }
            else if(value == 1)
            {
                if (mCells[pointerIndex.x, pointerIndex.y].IsSelectable)
                {
                    if (mSelectedCell.Ball) mSelectedCell.ChangeBallState(Ball.State.Idle);
                    mSelectedCell = mCells[pointerIndex.x, pointerIndex.y];
                    if (mSelectedCell) mSelectedCell.ChangeBallState(Ball.State.Selected);
                }
            }
        }
        else
        { 
            if(mCells[pointerIndex.x, pointerIndex.y].IsSelectable)
            {
                mSelectedCell = mCells[pointerIndex.x, pointerIndex.y];
                if (mSelectedCell) mSelectedCell.ChangeBallState(Ball.State.Selected);
            }
        }
    }

    public int PointToCell(Vector2Int index)
    {
        mMovePath = null;
        if (!mCells[index.x, index.y].IsAvailable)
            return 1;//gray

        mMovePath = BuildPath(mSelectedCell.Index, index);
        if (mMovePath.IsNullOrEmpty())
            return 2;//mask
        return 0;
    }

    public void OnTouchDown(Vector3 postion)
    {
        if (!mIsTouchable) return;

        var seletedIndex = GetIndex(postion);
        if (seletedIndex.x > -1 && seletedIndex.y > -1)
            SelectCell(seletedIndex);
    }

    protected Vector2Int GetIndex(Vector3 position)
    {
        foreach (Cell cell in mCells)
        {
            if (cell.Bounds.Contains(position))
                return cell.Index;
        }

        return new Vector2Int(-1, -1);
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
                    EventDispatcher.TriggerEvent<GameScoreEvent>(new GameScoreEvent(GamePlay.GameScore.Add, 1));
                }

                cell.Ball = null;
            }
        }
    }

    protected List<Vector2Int> BuildPath(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        path = Algorithm.CheckPath(mCells, from, to);
        string debugString = "";
        foreach (var node in path)
        {
            debugString += string.Format("{0}|{1} - ", node.x, node.y);
        }
        Debug.Log(debugString);

        return (path != null && path.Count > 0) ? path : null;
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

        float cellSize = 1f;
        float offset = (BOARD_SIZE - 1) * cellSize / 2;

        float offsetX = -offset, offsetY = offset;

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
                offsetX += cellSize;
            }

            offsetY -= cellSize;
            offsetX = -offset;
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

    public void MoveBallOnPath(Ball ball, List<Vector2Int> cellsIndex)
    {
        if (ball == null || cellsIndex.IsNullOrEmpty()) return;

        List<Vector3> pos = cellsIndex.Select(p => mCells[p.x, p.y].transform.position).ToList<Vector3>();
        ball.transform.position = pos[0];
        ball.Move(pos, OnMovePathDone);
    }

    public void OnMovePathDone()
    {
        Debug.Log("mOnMoveDone");

        CheckBoard();
        var cells = AddBalls(3, GameManager.Instance.NumOfKindBall, Ball.Size.Dot);

        if (mPointsToCheck.Count > 0)
        {
            foreach (var point in mPointsToCheck)
            {
                List<Vector2Int> points = Algorithm.CheckLines(mCells, point);

                if (points != null && points.Count > 0)
                {
                    foreach (var p in points) ExplodeBall(p);
                }
            }

            mPointsToCheck.Clear();
        }
    }

    protected bool IsInside(Vector2Int pos)
    {
        return 0 <= pos.x && BOARD_SIZE > pos.x && 0 <= pos.y && BOARD_SIZE > pos.y;
    }

    public List<Cell> AddBalls(int numOfBall, int numOfType, Ball.Size size)
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
                cell.AttachBall(DataManager.Instance.TakeRandomBall(numOfType));
                cell.SetBallSize(size);

                if(cell.Ball && cell.Ball.BallSize == Ball.Size.Dot)
                {
                    colors.Add(cell.Ball.BallColor);
                }
            }
        }

        if (colors.Count > 0) EventDispatcher.TriggerEvent<BallChangeEvent>(new BallChangeEvent(BallChangeEnum.Change, colors));

        return randomCells;
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
