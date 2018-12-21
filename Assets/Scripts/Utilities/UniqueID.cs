using UnityEngine;
using System.Collections;

public class UniqueID<T>
{
    private static int mCurrentID = 0;

    public static int NextUID
    {
        get
        {
            mCurrentID++;
            return mCurrentID;
        }
    }
}
