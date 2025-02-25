using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagers
{
    public class GameArea : SingletoneMono<GameArea>
    {
        private static ePlayArea g_MyPlayArea;
        public static ePlayArea MyPlayArea { get { return g_MyPlayArea; } set { g_MyPlayArea = value; } }

        public static bool IsMyArea(ePlayArea _playArea) { return (g_MyPlayArea == _playArea); }
        public static bool IsMyAreaIsA 
        { 
            get => g_MyPlayArea == ePlayArea.A; 
        }
        public static bool IsMyAreaIsB
        { 
            get => g_MyPlayArea == ePlayArea.B; 
        }

        public static bool IsAdmin
        {
            get => g_MyPlayArea == ePlayArea.X; 
        }
    }
}