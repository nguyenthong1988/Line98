using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : IUser
{
    protected User mUser;

    protected void Initialize()
    {
        if (mUser == null)
        {
            mUser = new User();
        }
    }

    public string GetUDID()
    {
        Initialize();
        return mUser.GetUDID();
    }
    public string GetName()
    {
        Initialize();
        return mUser.GetName();
    } 
    public int GetHighScore()
    {
        Initialize();
        return mUser.GetHighScore();
    }

    public void SetUIID(string UDID)
    {

    }

    public void SetName(string name)
    {
        if (string.IsNullOrEmpty(name)) return;
        Initialize();
        mUser.SetName(name);
    }

    public void SetHighScore(int score)
    {
        Initialize();
        if (mUser.GetHighScore() >= score) return;
        mUser.SetHighScore(score);
    }
}
