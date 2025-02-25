using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Networking;

#pragma warning disable CS0618

namespace GameManagers
{
    public class GameState : NetworkBehaviour
    {
        public static event Action onPlayStartUpdate;
        public static event Action onGamePauseUpdate;
        public static event Action onGamePlayUpdate;
        public static event Action onUIPlayUpdate;

        public static event Action onAlwaysUpdate;
        public static event Action onAlwaysLateUpdate;
        public static event Action onAlwaysFixedUpdate;

        public static event Action<eGameState, eGameState> onChangeGameState; 
        public static bool CheckChangeEventNull
        {
            get => null == onChangeGameState;
        }

        private eGameState m_PreviewGameState = eGameState.Start;
        private eGameState m_CurrentGameState = eGameState.Start;

        public eGameState CurrentGameState { get { return m_CurrentGameState; } }

        private Dictionary<eGameState, System.Action> m_GameStateUpdate;

        public static bool IsAddedAlwaysLateUpdate(System.Action _target)
        {
            return MyUtility.IsAddedEvent(onAlwaysLateUpdate, _target);
        }

        private static bool m_bIsApplicationClose = false;
        private static object lockobj = new object();
        protected static GameState m_Instance;
        public static GameState Instance
        {
            get
            {
                if (m_bIsApplicationClose)
                    return null;

                lock (lockobj)
                {
                    if (m_Instance == null)
                    {
                        GameState[] objs = FindObjectsOfType<GameState>();

                        if (objs.Length > 0)
                            m_Instance = objs[0];
                    }
                }

                return m_Instance;
            }
        }
        
        void OnApplicationQuit()
        {
            m_bIsApplicationClose = true;
        }

        private void OnDestroy()
        {
            onPlayStartUpdate   = null;
            onGamePauseUpdate   = null;
            onGamePlayUpdate    = null;
            onUIPlayUpdate      = null;

            onAlwaysUpdate      = null;
            onAlwaysLateUpdate  = null;
            onAlwaysFixedUpdate = null;

            onChangeGameState   = null;

            m_Instance = null;
        }
               
        void Awake()
        {
            //DontDestroyOnLoad(gameObject);

            if (m_Instance == null)
            {
                m_Instance = this;
            }
        
            m_PreviewGameState = m_CurrentGameState;
            m_GameStateUpdate = new Dictionary<eGameState, Action>( new Compares.GameStateComparer() )
            {
                { eGameState.Start,  UpdateStart },
                { eGameState.Play,   UpdatePlay  },
                { eGameState.Pause,  UpdatePause },
                { eGameState.Finish, null        },
                { eGameState.UI,     UpdateUI    }
            };


            ResetState();

            onAlwaysUpdate      += none;
            onAlwaysLateUpdate  += none;
            onAlwaysFixedUpdate += none;
        }

        public void Init()
        {
            onChangeGameState( m_PreviewGameState, m_CurrentGameState ); 
        }

        public void ChangeGameState(eGameState _newState)
        {
            if (m_CurrentGameState == _newState)
                return;

            m_PreviewGameState = m_CurrentGameState;
            m_CurrentGameState = _newState;

            
            if (onChangeGameState != null)
            {
                onChangeGameState(m_PreviewGameState, m_CurrentGameState);
//                m_GameStateUpdate[m_CurrentGameState]?.Invoke();
            }
        }

        public void ResetState()
        {
            m_PreviewGameState = eGameState.Start;
            m_CurrentGameState = eGameState.Start;
        }

        //public void ChangeToPrevState() { ChangeGameState(m_PreviewGameState); }

        private void UpdateStart()  { onPlayStartUpdate?.Invoke(); }
        private void UpdatePause()  { onGamePauseUpdate?.Invoke(); }
        private void UpdatePlay()   { onGamePlayUpdate?.Invoke(); }
        private void UpdateUI()     { onUIPlayUpdate?.Invoke(); onGamePlayUpdate?.Invoke(); }

        void Update()
        {
            onAlwaysUpdate();

            m_GameStateUpdate[ m_CurrentGameState ]?.Invoke();
        }

        void LateUpdate()
        {
            onAlwaysLateUpdate();
        }

        void FixedUpdate()
        {
            onAlwaysFixedUpdate();
        }


        static void none() { }
    }
    
}
