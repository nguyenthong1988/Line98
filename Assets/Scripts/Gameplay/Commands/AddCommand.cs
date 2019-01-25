using System.Collections;
using UnityEngine;

public class AddCommand : ICommand
{
    public Vector2 Position;

    private Board mBoard; 

    public AddCommand(Board board)
    {
        mBoard = board;
    }

    public AddCommand(Board board, Vector2 position) : this(board)
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
