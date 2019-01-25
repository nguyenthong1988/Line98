using System.Collections;
using UnityEngine;

public class MoveCommand : ICommand
{
    public Vector2 FromPosition, ToPosition;

    private Board mBoard;

    public MoveCommand(Board board)
    {
        mBoard = board;
    }

    public MoveCommand(Board board, Vector2 fromPosition, Vector2 toPosition) : this(board)
    {
        FromPosition = fromPosition;
        ToPosition = toPosition;
    }

    public void Execute()
    {
        
    }

    public void Reverse()
    {

    }
}
