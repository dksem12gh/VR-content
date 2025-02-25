using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManagers;

#pragma warning disable CS0618
#pragma warning disable CS0649


public class GameMain : MonoBehaviour
{
    public static System.Action    initAction;
    [SerializeField] GameState  m_gameState;

    private void Awake()
    {
        MyNetworkManager.Instance.SceneSetFinish += Init;
    }

    //private void Start()
    //{
    //  Init();
    //}

    //private void Update()
    //{
    //    if( Input.GetKeyDown( KeyCode.H ) )
    //        Init();
    //}

    private void OnDestroy()
    {
        MyNetworkManager.Instance.SceneSetFinish -= Init;
        initAction = null;

        EventObjectHandler.Relese();
        GameEvent.Relese();
    }

    public void Init()
    {
        if( null != initAction )
        {
            initAction();
        }

        m_gameState.Init();
    }
}
