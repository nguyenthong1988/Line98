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

    public bool SaveBoard(Cell[,] cells, float time)
    {
        BoardSave save = new BoardSave(cells, time);
        IOFile.WriteCacheJson<BoardSave>(save, SAVE_FILE);
        return false;
    }
}