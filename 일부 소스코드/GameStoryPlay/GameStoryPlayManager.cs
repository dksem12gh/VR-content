using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStoryPlayManager
{
    static GameStoryPlayManager instance = null;
    public static GameStoryPlayManager Instance {
        get => instance;
    }

    public static void EnableSingleton()
    {
        instance = new GameStoryPlayManager();
    }

    public static void DisableSingleton()
    {
        if( null != instance )
        {
            instance.Relese();
            instance = null;
        }
    }

    public static void Clear()
    {
        instance.Relese();
    }



    void Relese()
    {

    }
}