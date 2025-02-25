using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagers
{
    public enum eGameLanguage { ko,ja,zh,en }
    public enum ePlayArea { A = 0, B, X }
    public enum eGameState { Start, Play, Pause, Finish, UI }

    public enum eRoom { PlayRoom}
    public enum eUserEvent
    {
        PlayRoom, PLAY_RETURN, PlayFinish
    }

    public enum eSoundIDType { BGM, Ambient, Common }

    public enum eSoundID_BGM
    {
        FailedEnding, SuccessEnding, 
    }
    public enum eSoundID_Ambient
    {
        WaitRoom, PlayRoom, RoomBridge
    }
    public enum eSoundID_Common
    {   
        GrabItem,
        ItemDrop,
        MoveEnter,
        MoveOpen,
        ActionFinish
    }
    

    public enum eGameTextType
    {
        Screen,
        Item
    }

    public enum eGameTextID_Screen
    {
        NONE,
        OverBoundary = 1,
        MoveToPointOnFront,
        MoveToPoint,
        GameOverAndTurnOffEquipment,
        WaitingForPlayer,
        MoveToArrow,
        WaitSelectEpisode,


        InsertCoin = 99,
    }
}
