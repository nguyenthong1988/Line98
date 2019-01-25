using System.Collections;
using UnityEngine;

public class RemoveCommand : ICommand
{
    public Vector2 Position;

    private Board mBoard;

    public RemoveCommand(Board board)
    {
        mBoard = board;
    }

    public RemoveCommand(Board board, Vector2 position) : this(board)
    {
        Position = position;
    }

    public void Execute()
    {

    }

    public void Reverse()
    {

    }
}
