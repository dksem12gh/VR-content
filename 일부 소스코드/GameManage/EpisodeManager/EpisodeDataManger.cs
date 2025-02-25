using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonFx.Json;

public class EpisodeDataManger
{
    public interface IUI_View
    {
        int     id       { get; }
        string  viewName { get; }
    }

    public class EpisodeData : IUI_View
    {
        public  int     episodeID;
        public  string  episodeViewName;

        Dictionary<int, SubEpisodeData> m_subData = new Dictionary<int, SubEpisodeData>();
        public Dictionary<int, SubEpisodeData> subData
        {
            get => m_subData;
        }

        public int id
        {
            get => episodeID;
        }

        public string viewName
        {
            get => episodeViewName;
        }

        public void AddEpisodeData( SubEpisodeData data )
        {
            m_subData.Add( data.id, data );
        }

        public SubEpisodeData   GetEpisodeData( int subID )
        {
            return m_subData[ subID ];
        }        
    }

    public class SubEpisodeData : IUI_View
    {
        public int     subid;
        public string  episodeSceneName = string.Empty;
        public string  episodeViewName  = string.Empty;
        public bool    viewActive       = false;
        public string  scenario_data    = string.Empty;
        public int     playerCount      = 0;
        public int[]   nextStorys       = null;
        public int     storyPack        = 0;
        public string  storyPackName    = string.Empty;

        public string nextStroyStr
        {
            set
            {
                string[] strs = value.Split(',');
                if( strs.Length == 0 )
                {
                    nextStorys = new int[ 0 ];
                }
                else
                {
                    nextStorys = new int[ strs.Length ];
                    for( int i = 0; i < strs.Length; i++ )
                    {
                        nextStorys[ i ] = int.Parse( strs[ i ] );
                    }
                }
            }
        }

        public int id
        {
            get => subid;
        }

        public string viewName
        {
            get => episodeViewName;
        }
    }

    Dictionary<int, EpisodeData >      m_episodeData = new Dictionary<int, EpisodeData>();
    public Dictionary<int, EpisodeData>  episodeData
    {
        get => m_episodeData;
    }

    public void Relese()
    {
        m_episodeData.Clear();
    }

    public void Init()
    {
        string idStr            = "id";
        string subIDSTR         = "sub_id";
        string scene_nameStr    = "scene_name";
        string view_nameStr     = "view_name";
        string scenario_dataStr = "scenario_data";
        string activeStr        = "active";
        string player_countStr  = "player_count";

        LRootEpisodeData();
        LSubEpisodeData();

        void LRootEpisodeData()
        {
            string      str        = File.ReadAllText( $"{Application.streamingAssetsPath}/scenario_info.txt" );
            JsonHelper  jsonHelper = new JsonHelper( str );

            EpisodeData episodeData;

            foreach( Dictionary<string, object> dic in jsonHelper )
            {
                episodeData = new EpisodeData();

                episodeData.episodeID = int.Parse( dic[ idStr     ].ToString() );
                episodeData.episodeViewName = dic[ view_nameStr   ].ToString();

                m_episodeData.Add( episodeData.episodeID, episodeData );
            }
        }

        void LSubEpisodeData()
        {
            string          next_storyStr      = "next_story";
            string          story_packStr      = "story_pack";
            string          story_pack_nameStr = "story_pack_name";
            string          str             = File.ReadAllText( $"{Application.streamingAssetsPath}/scenario_Sub_info.txt" );
            JsonHelper      jsonHelper      = new JsonHelper( str );

            SubEpisodeData  subEpiData;
            int             mainID;

            List<int>       m_nextID = new List<int>(5);

            foreach( Dictionary<string, object> dic in jsonHelper )
            {
                subEpiData = new SubEpisodeData();
                subEpiData.subid            = int.Parse( dic[ subIDSTR ].ToString() );
                subEpiData.episodeSceneName = dic[ scene_nameStr ].ToString();
                subEpiData.episodeViewName  = dic[ view_nameStr  ].ToString();
                subEpiData.viewActive       = bool.Parse( dic[ activeStr ].ToString() );
                subEpiData.scenario_data    = dic[ scenario_dataStr ].ToString();
                subEpiData.playerCount      = int.Parse( dic[ player_countStr ].ToString() );
                subEpiData.nextStroyStr     = dic[ next_storyStr ].ToString();
                subEpiData.storyPack        = int.Parse( dic[ story_packStr ].ToString() );
                subEpiData.storyPackName    = dic[ story_pack_nameStr ].ToString();

                mainID = int.Parse( dic[ idStr ].ToString() );

                LLParseNextStory(ref subEpiData, dic[ next_storyStr ].ToString() );

                m_episodeData[ mainID ].AddEpisodeData( subEpiData );
            }

            void LLParseNextStory( ref SubEpisodeData data, string storyIdData )
            {
                string[]    idSplit = storyIdData.Split(',');

                for( int i = 0; i < idSplit.Length; i++ )
                {
                    m_nextID.Add( int.Parse( idSplit[ i ] ) );
                }

                //if( -1 == m_nextID[0] )
                //{
                //    data.nextStorys = new int[ 0 ];
                //}
                //else
                //{
                    data.nextStorys = m_nextID.ToArray();
                //}

                m_nextID.Clear();
            }
        }
    }

    public string NetSceneName( int sceneID, int subID )
    {
        return m_episodeData[ sceneID ].GetEpisodeData( subID ).episodeSceneName;
    }
}