using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : IUser
{
    public string GetUDID()
    {
        string udid = PlayerPrefs.GetString("UDID");
        if (string.IsNullOrEmpty(udid))
        {
            udid = SystemInfo.deviceUniqueIdentifier;
            PlayerPrefs.SetString("UDID", udid);
        }

        return udid;
    }

    public string GetName()
    {
        return PlayerPrefs.GetString("Name", "Player " + GetUDID());
    }

    public int GetHighScore()
    {
        return PlayerPrefs.GetInt("HighScore", 0);
    }

    public void SetUIID(string UDID)
    { 
    
    }

    public void SetName(string name)
    {
        PlayerPrefs.SetString("Name", name);
    }

    public void SetHighScore(int score)
    {
        PlayerPrefs.SetInt("HighScore", score);
    }
}