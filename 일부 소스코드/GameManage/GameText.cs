using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameManagers;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;

using ExcelDataReader;
#endif
namespace GameManagers
{ 
    public class GameText 
    {
        private const string C_EMPTY_STRING = "";
        [SerializeField] private List<TextData> m_listSerializedTextData;

        public class TextDataGroup
        {
            Dictionary<int, TextData> m_dicTextData;

            public TextDataGroup()
            {
                m_dicTextData = new Dictionary<int, TextData>();
            }

            public void AddTextData(TextData _data)
            {
                if (!m_dicTextData.ContainsKey(_data.ID))
                    m_dicTextData.Add(_data.ID, _data);
            }

            public string GetText(int _id)
            {
                if (!m_dicTextData.ContainsKey(_id))
                    return string.Empty;
                else
                    return m_dicTextData[_id].GetText();
            }
        }
        [System.Serializable]
        public class TextData
        {
            [SerializeField] public eGameTextType m_TextType;
            [SerializeField] public int m_iTextID;
            [SerializeField] public string m_sText_ko;
            [SerializeField] public string m_sText_ja;
            [SerializeField] public string m_sText_en;
            [SerializeField] public string m_sText_zh;


            public eGameTextType TextType { get { return m_TextType; } }
            public int ID { get { return m_iTextID; } }

            public string GetText() { return m_refDelegate_GetText(); }

            private System.Func<string> m_refDelegate_GetText;

            private const string C_EMPTY_STRING = "";
            private string GetText_Empty() { return C_EMPTY_STRING; }
            private string GetText_KR() { return m_sText_ko; }
            private string GetText_JP() { return m_sText_ja; }
            private string GetText_ENG() { return m_sText_en; }
            private string GetText_ZH() { return m_sText_zh; }

            public TextData()
            {
                switch(RoomGameLauncherData.Instance.Language)
                {
                    case eGameLanguage.ko: m_refDelegate_GetText = GetText_KR;   break;
                    case eGameLanguage.ja: m_refDelegate_GetText = GetText_JP; break;
                    case eGameLanguage.en: m_refDelegate_GetText = GetText_ENG; break;
                    case eGameLanguage.zh: m_refDelegate_GetText = GetText_ZH; break;
                    default: m_refDelegate_GetText = GetText_Empty;  break;
                }
            }

        }
        
        private Dictionary<eGameTextType, TextDataGroup>    m_dicTextDataGroup;
        private Dictionary<int, Dictionary<int, TextData>>  m_dicItemTextDataGroup;

        private static GameText m_Instance;
        public static GameText Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    //try
                    //{
                        string jsonData = File.ReadAllText(Application.streamingAssetsPath + "/GameTextData.txt");
                        m_Instance = JsonUtility.FromJson<GameText>(jsonData);
                        m_Instance.ParsingTextData();

                        //item text 파싱
                        m_Instance.ParseItemTextData();
                        Debug.Log("HasData");
                    //}
                    //catch
                    //{
                    //    m_Instance = new GameText();
                    //    Debug.Log("New Text Data");
                    //}
                }
                return m_Instance;
            }
        }

        private void ParsingTextData()
        {
            m_dicTextDataGroup = new Dictionary<eGameTextType, TextDataGroup>();

            for (int i = 0; i < m_listSerializedTextData.Count; i++)
            {
                eGameTextType textType = m_listSerializedTextData[i].TextType;
                if (!m_dicTextDataGroup.ContainsKey(textType))
                {
                    m_dicTextDataGroup.Add(textType, new TextDataGroup());
                }

                m_dicTextDataGroup[textType].AddTextData(m_listSerializedTextData[i]);
            }
        }

        /// <summary>
        /// Item Text만 parsing
        /// </summary>
        private void ParseItemTextData()
        {
            string stageStr = "stage";
            string idStr    = "id";
            string krStr    = "kr";

            string     jsonData   = File.ReadAllText( Application.streamingAssetsPath + "/ItemTextTable.txt" );
            JsonHelper jsonHelper = new JsonHelper( jsonData );

            int      stageID;
            TextData textData;

            if( null == m_dicItemTextDataGroup )
                m_dicItemTextDataGroup = new Dictionary<int, Dictionary<int, TextData>>();
            m_dicItemTextDataGroup.Clear();

            foreach( Dictionary<string, object> dic in jsonHelper )
            {
                textData = new TextData();
                textData.m_TextType = eGameTextType.Item;
                               
                textData.m_iTextID  = int.Parse( dic[idStr].ToString() );
                textData.m_sText_ko = dic[ krStr  ].ToString();

                stageID = int.Parse( dic[ stageStr ].ToString() );

                if( false == m_dicItemTextDataGroup.ContainsKey( stageID ) )
                {
                    m_dicItemTextDataGroup.Add( stageID, new Dictionary<int, TextData>() );
                }

                m_dicItemTextDataGroup[ stageID ].Add( textData.m_iTextID, textData );
            }
        }

        public string GetText( eGameTextType _type, int _id )
        {
            if( !m_dicTextDataGroup.ContainsKey( _type ) )
                return C_EMPTY_STRING;
            else
            {
                return m_dicTextDataGroup[ _type ].GetText( _id ); // m_dicTextDataGroup[_type].get
            }
        }

        public string GetItemText( int stage, int id )
        {
            string   outTex   = string.Empty;
            TextData textData;

            if( true == m_dicItemTextDataGroup.ContainsKey( stage ) )
            {
                m_dicItemTextDataGroup[ stage ].TryGetValue( id, out textData );
                outTex = textData.GetText();
            }

            return outTex;
        }


        public string GetText(eGameTextID_Screen _id) { return GetText(eGameTextType.Screen, (int)_id); }
        public string GetText( int _id, int stageID ) { return GetItemText( stageID,   (int)_id); }

        public void SetTextDataList(List<TextData> _list)
        {
            m_listSerializedTextData = _list;
        }
    }
}

#if UNITY_EDITOR
public static class EGameText
{
    [MenuItem("RoomEscape/Create GameText Json file")]
    public static void CreateJsonFile()
    {
        string filepath = EditorUtility.OpenFilePanel( "Select excel file", Application.dataPath + "/~ExcelData", "xls" );
        using( var stream = File.Open( filepath, FileMode.Open, FileAccess.Read ) )
        {
            GameText gameText = new GameText();

            List<GameText.TextData> textList = new List<GameText.TextData>();
            string tapName = "";

            // Auto-detect format, supports:
            //  - Binary Excel files (2.0-2003 format; *.xls)
            //  - OpenXml Excel files (2007 format; *.xlsx)
            using( var reader = ExcelReaderFactory.CreateBinaryReader( stream ) )
            {
                // Choose one of either 1 or 2:

                // 1. Use the reader methods
                do
                {
                    tapName = reader.Name;
                    while( reader.Read() )
                    {
                        if( reader.Depth == 0 )
                            continue;

                        GameText.TextData data = new GameText.TextData();

                        data.m_TextType = ( GameManagers.eGameTextType )System.Enum.Parse( typeof( GameManagers.eGameTextType ), tapName );
                        if( null == reader.GetValue( 0 ) )
                            continue;
                        data.m_iTextID  = ( int )reader.GetDouble( 0 );
                        data.m_sText_ko = reader.GetString( 1 );
                        data.m_sText_ja = reader.GetString( 2 );
                        data.m_sText_en = reader.GetString( 3 );
                        data.m_sText_zh = reader.GetString( 4 );
                        
                        textList.Add( data );
                        // reader.GetDouble(0);
                    }
                } while( reader.NextResult() );

                // 2. Use the AsDataSet extension method
                //var result = reader.AsDataSet();

                // The result of each spreadsheet is in result.Tables

                gameText.SetTextDataList( textList );
                string toJson = JsonUtility.ToJson( gameText, true );

                System.IO.File.WriteAllText( Application.streamingAssetsPath + "/GameTextData.txt", toJson, System.Text.Encoding.UTF8 );
                AssetDatabase.Refresh();
            }
        }

    }
}
#endif