using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    protected const string SAVE_FILE = "game_save.dat";
    public bool LoadBoardSave(Cell[,] cells)
    {
        BoardSave save = IOFile.ReadCacheJson<BoardSave>(SAVE_FILE);
        if (cells.Length == 0 || save == null) return false;
        save.MapSaveToBoard(cells);
        return true;
    }

    public BoardSave GetBoardSave()
    { 
        return IOFile.ReadCacheJson<BoardSave>(SAVE_FILE);
    }

    public bool SaveBoard(Cell[,] cells, float time, int score)
    {
        BoardSave save = new BoardSave(cells, score, time);
        IOFile.WriteCacheJson<BoardSave>(save, SAVE_FILE);
        return false;
    }
}