using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    protected const string SAVE_FILE = "game_save.dat";
    public bool LoadBoardSave(Cell[,] cells)
    {
        BoardData save = IOFile.ReadCacheJson<BoardData>(SAVE_FILE);
        if (cells.Length == 0 || save == null) return false;
        save.MapSaveToBoard(cells);
        return true;
    }

    public BoardData GetBoardSave()
    { 
        return IOFile.ReadCacheJson<BoardData>(SAVE_FILE);
    }

    public bool SaveBoard(Cell[,] cells, float time, int score)
    {
        BoardData save = new BoardData(cells, score, time);
        IOFile.WriteCacheJson<BoardData>(save, SAVE_FILE);
        return false;
    }
}