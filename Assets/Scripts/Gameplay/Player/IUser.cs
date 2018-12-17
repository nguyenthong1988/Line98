public interface IUser
{
    string GetUDID();
    string GetName();
    int GetHighScore();

    void SetUIID(string UDID);
    void SetName(string name);
    void SetHighScore(int score);
}