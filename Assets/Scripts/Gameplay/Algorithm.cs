using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Algorithm
{
    public static List<Vector2Int> CheckPath(Cell[,] cells, Vector2Int from, Vector2Int to)
    {
        Vector2Int[,] dad = new Vector2Int[Board.BOARD_SIZE, Board.BOARD_SIZE];
        Vector2Int[] queue = new Vector2Int[Board.BOARD_SIZE * Board.BOARD_SIZE];
        Vector2Int[] trace = new Vector2Int[Board.BOARD_SIZE * Board.BOARD_SIZE];

        bool ghostCell = cells[from.x, from.y].BallColor >= Ball.Color.Ghost;

        int[] u = { 1, 0, -1, 0 };
        int[] v = { 0, 1, 0, -1 };

        int fist = 0, last = 0, x, y, i, j, k;
        for (x = 0; x < Board.BOARD_SIZE; x++)
            for (y = 0; y < Board.BOARD_SIZE; y++)
            {
                dad[x, y] = new Vector2Int(-1, -1);
                trace[x * Board.BOARD_SIZE + y] = new Vector2Int(-5, -5);
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
                    return trace.Where(p => (p.x > -5 && p.y > -5)).ToList<Vector2Int>();
                }

                if (!IsInside(dir.x, dir.y)) continue;

                if (dad[dir.x, dir.y].x == -1 && ((cells[dir.x, dir.y].IsAvailable || ghostCell)))
                {
                    last++;
                    queue[last] = dir;
                    dad[dir.x, dir.y] = new Vector2Int(x, y);
                }
            }
        }

        return trace.Where(p => (p.x > -5 && p.y > -5)).ToList<Vector2Int>();
    }

    public static List<Vector2Int> CheckLines(Cell[,] cells, Vector2Int point)
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
                if (cells[i, j].IsEmpty || cells[i, j].Ball.BallColor != cells[x, y].Ball.BallColor)
                    break;
                k++;
            }
            i = x; j = y;
            while (true)
            {
                i -= u[t]; j -= v[t];
                if (!IsInside(i, j))
                    break;
                if (cells[i, j].IsEmpty || cells[i, j].Ball.BallColor != cells[x, y].Ball.BallColor)
                    break;
                k++;
            }
            k++;
            if (k >= Board.POINT_TO_SCORE)
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

    public static bool IsInside(int x, int y)
    {
        return 0 <= x && Board.BOARD_SIZE > x && 0 <= y && Board.BOARD_SIZE > y;
    }
}
