using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonFx.Json;

public class GameEpisodeMgr
{
    public enum PLAY_MODE
    {
        SINGLE_PLAY, MULTI_PLAY
    }

    public enum PLAY_TYPE
    {
        NORMAL, EVALUATE
    }

    static  GameEpisodeMgr  instance;
    public  static  GameEpisodeMgr Instance
    {
        get => instance;
    }

    public static void EnableSingleton()
    {
        instance = new GameEpisodeMgr();
        instance.Init();
    }

    public static void DisableSingleton()
    {
        if( null != instance )
            instance.Relese();
        instance = null;
    }

    InfectionQuizData   m_quizData = null;
    EpisodeDataManger   m_dataManager;
    int                 m_currSelectEpisode = 0;
    int                 m_currSelectEpisodeSub = 0;

    int                 m_saveSelectSubID = 0;

    // ABGA, CBC 추후 이동을 위한 값
    int                 m_selectMode = 0;
    int                 m_currPlayMode = 0;

    public int selectMode
    {
        set
        {
            m_selectMode = value;
            m_currPlayMode = value;
        }
        get
        {
            int iValue = m_selectMode;
            m_selectMode = -1;
            return iValue;
        }
    }

    public int currPlayMode
    {
        get
        {
            return  m_currPlayMode;
        }
    }

    public  int currSelectEpisode
    {
        get => m_currSelectEpisode;
        set
        {
            m_currSelectEpisode = value;
        }
    }
    public  int currSelectEpisodeSub
    {
        get => m_currSelectEpisodeSub;
        set
        {
            m_currSelectEpisodeSub = value;
            m_saveSelectSubID = m_currSelectEpisodeSub;
        }
    }

    public PLAY_MODE m_currMode = PLAY_MODE.SINGLE_PLAY;
    public PLAY_TYPE m_currType = PLAY_TYPE.NORMAL;

    void Init()
    {
        m_dataManager = new EpisodeDataManger();
        m_dataManager.Init();

        m_quizData = new InfectionQuizData();
        m_quizData.LoadData();

        m_currSelectEpisode = 0;
        m_currSelectEpisodeSub = 0;
    }
    void Relese()
    {
        m_dataManager.Relese();
    }

    public bool CheckNextMultiStory()
    {
        return ( currSubData[ m_currSelectEpisodeSub ].nextStorys.Length > 1 );
    }

    public bool SetNextSubEpisode()
    {
        bool bNext;

        m_currSelectEpisodeSub = currSubData[ m_currSelectEpisodeSub ].nextStorys[0];
        bNext = currSubData.ContainsKey( m_currSelectEpisodeSub );

        return bNext;
    }

    public string NetCurrSceneName()
    {
        return m_dataManager.NetSceneName( m_currSelectEpisode, m_currSelectEpisodeSub );
    }

    public EpisodeDataManger.EpisodeData GetEpisodeData()
    {
        return m_dataManager.episodeData[ m_currSelectEpisode ];
    }

    public EpisodeDataManger.SubEpisodeData GetEpisodeSubData()
    {
        return m_dataManager.episodeData[ m_currSelectEpisode ].GetEpisodeData( m_currSelectEpisodeSub );
    }

    /// <summary>
    /// 전체 에피소드 개수
    /// </summary>
    public int EpisodeLength
    {
        get => m_dataManager.episodeData.Count;
    }

    /// <summary>
    /// 현재 서브에피소드 개수
    /// </summary>
    public int CurrSubEpisodeLength
    {
        get => m_dataManager.episodeData[ m_currSelectEpisode ].subData.Count;
    }

    public Dictionary<int, EpisodeDataManger.SubEpisodeData> currSubData
    {
        get => m_dataManager.episodeData[ m_currSelectEpisode ].subData;
    }

    public Dictionary<int, EpisodeDataManger.EpisodeData> episodeDataDic
    {
        get => m_dataManager.episodeData;
    }

    public string PlayTitle
    {
        get => m_dataManager.episodeData[ currSelectEpisode ].GetEpisodeData( m_saveSelectSubID ).viewName;
    }

    public InfectionQuizData.QuizData quizData( int idx )
    {
        return m_quizData.GetData( idx );
    }
}