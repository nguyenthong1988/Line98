using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandProcessor : MonoBehaviour
{
    private List<ICommand> mCommands = new List<ICommand>();
    private int mCurrentCommandIndex = 0;

    public void ExecuteCommand(ICommand command)
    {
        mCommands.Add(command);
        command.Execute();
        mCurrentCommandIndex++;
    }

    public void Undo()
    {
        if (mCurrentCommandIndex < 0) return;

        mCommands[mCurrentCommandIndex].Reverse();
        mCommands.RemoveAt(mCurrentCommandIndex);
        mCurrentCommandIndex--;
    }
}
