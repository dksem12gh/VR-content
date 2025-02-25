using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStoryData
{
    //public string GetTitleName( int idx )
    //{
    //    return GameEpisodeMgr.Instance.episodeDataDic[idx].viewName;
    //}

    public class StoryData
    {
        public readonly  int    m_titleID;
        public readonly  int    m_ID;
        public readonly  string m_comment;
        public readonly  bool   m_noneState;

        public StoryData( int titleID, int id, string comment, bool none )
        {
            m_titleID   = titleID;
            m_ID        = id;
            m_comment   = comment;
            m_noneState = none;
        }
    }

    Dictionary<int, Dictionary<int, StoryData>> m_storyData = new Dictionary<int, Dictionary<int, StoryData>>();

    //StoryData[] m_stroyData = null;
    public StoryData GetStoryData( int idx )
    {
        return m_storyData[ GameEpisodeMgr.Instance.currSelectEpisodeSub ][ idx ];
    }
    
    public GameStoryData()
    {
        const string titleIDStr = "title_id"; //Data Title
        const string idStr      = "id";       //Data ID
        const string commentStr = "comment";  //대사
        const string noneStr    = "none";
        const string idxStr     = "idx";      //순서

        EpisodeDataManger.EpisodeData data = GameEpisodeMgr.Instance.GetEpisodeData();
        EpisodeDataManger.SubEpisodeData subData = GameEpisodeMgr.Instance.GetEpisodeSubData();
        string path = $"{Application.streamingAssetsPath}/{subData.scenario_data}.txt";
        
        if( false == System.IO.File.Exists( path ) )
        {
            Debug.Log( $"{data.episodeID} data file don't Exists" );
            return;
        }

        string jsonData = System.IO.File.ReadAllText( path );
        JsonHelper jsonHelper = new JsonHelper( jsonData );

        foreach( Dictionary<string, object> dic in jsonHelper )
        {
            LParsData( dic );
        }

        void LParsData( Dictionary<string, object> dic )
        {
            int idx        = int.Parse( dic[ idxStr     ].ToString() );
            int titleID    = int.Parse( dic[ titleIDStr ].ToString() );
            int id         = int.Parse( dic[ idStr      ].ToString() );
            string comment = "";// = dic[ commentStr ].ToString();
            if(dic.ContainsKey(commentStr) == true)
                if(dic[commentStr] != null)
                    comment = dic[commentStr].ToString();

            if(comment.Contains("//"))
            {
                comment = "";
            }

            bool noneState = false;
            if (dic.ContainsKey( noneStr ) != false)
                noneState = int.Parse( dic[ noneStr ].ToString() ) == 0;

            if ( false == m_storyData.ContainsKey( titleID ) )
            {
                m_storyData.Add( titleID, new Dictionary<int, StoryData>() );
            }

            m_storyData[ titleID ].Add( id, new StoryData( titleID, id, comment, noneState ) );
        }
    }
}