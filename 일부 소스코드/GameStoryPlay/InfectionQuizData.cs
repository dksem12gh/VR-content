using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class InfectionQuizData
{
    public class QuizData
    {
        public enum COMMENT_TYPE
        {
            NONE, TEXT, IMG, BOTH
        }

        public readonly string    m_question        = string.Empty;
        public readonly AudioClip m_questionAudio   = null;
        public readonly string[]  m_list            = null;
        public readonly string[]  m_commentary      = null;
        public readonly AudioClip m_commentaryAudio = null;

        private readonly int     m_answer     = 0;

        public readonly COMMENT_TYPE    m_type;

        public QuizData( string question, string[] list, int answer, string questionAudio, string commentaryAudio )
        {
            m_question = question;
            m_list     = list;
            m_answer   = answer;
            m_type     = COMMENT_TYPE.NONE;

            if( false == questionAudio.Equals( string.Empty) )
                m_questionAudio   = Resources.Load<AudioClip>( questionAudio );
            if (false == commentaryAudio.Equals( string.Empty ))
                m_commentaryAudio = Resources.Load<AudioClip>( commentaryAudio );
        }

        public QuizData( string question, string[] list, int answer, string commentary, bool isImg, string questionAudio, string commentaryAudio )
        {
            m_question = question;
            m_list     = list;
            m_answer   = answer;

            m_commentary      = new string[ 1 ];
            m_commentary[ 0 ] = commentary;

            m_type = ( false == isImg ) ? COMMENT_TYPE.TEXT : COMMENT_TYPE.IMG;

            if (false == questionAudio.Equals( string.Empty ))
                m_questionAudio = Resources.Load<AudioClip>( questionAudio );
            if (false == commentaryAudio.Equals( string.Empty ))
                m_commentaryAudio = Resources.Load<AudioClip>( commentaryAudio );
        }

        public QuizData( string question, string[] list, int answer, string commentary, string commentary_path, string questionAudio, string commentaryAudio )
        {
            m_question = question;
            m_list     = list;
            m_answer   = answer;

            m_commentary      = new string[ 2 ];
            m_commentary[ 0 ] = commentary;
            m_commentary[ 1 ] = commentary_path;

            m_type = COMMENT_TYPE.BOTH;
            
            if( false == questionAudio.Equals( string.Empty) )
                m_questionAudio   = Resources.Load<AudioClip>( questionAudio );
            if (false == commentaryAudio.Equals( string.Empty ))
                m_commentaryAudio = Resources.Load<AudioClip>( commentaryAudio );
        }

        public bool isAnswer( int idx )
        {
            return m_answer == idx;
        }
    }

    readonly List<QuizData>  m_quizList = new List<QuizData>();
    public QuizData GetData( int idx )
    {
        return ( idx < m_quizList.Count ) ? m_quizList[ idx ] : null;
    }

    public void LoadData()
    {
        string  questionStr        = "question";
        string  questionAudioStr   = "question_audio";
        string  listStr            = "list";
        string  answerStr          = "answer";
        string  commentaryStr      = "commentary";
        string  commentaryAudioStr = "commentary_audio";
        string  commentary_imgStr  = "commentary_img";

        string audioClipPath = "Sounds/quiz/";

        string   question;
        string   questionAudio;
        string[] list;
        int      answer;
        string   commentary = string.Empty;
        string   commentaryAudio = string.Empty;
        string   commentary_path = string.Empty;

        int      commentType = 0;

        QuizData    quizData;

        m_quizList.Clear();

        JsonHelper  helper = new JsonHelper( File.ReadAllText( $"{Application.streamingAssetsPath}/quiz.json" ) );

        foreach( Dictionary<string, object> data in helper)
        {
            question = ( string )data[ questionStr ];
            list = ( string[] )data[ listStr ];
            answer = ( int )data[ answerStr ];

            if( true == data.ContainsKey( commentaryStr ))
            {
                commentary = ( string )data[ commentaryStr ];
                commentType += 1;
            }
            if( true == data.ContainsKey( commentary_imgStr ))
            {
                commentary_path = ( string )data[ commentary_imgStr ];
                commentType += 2;
            }

            questionAudio   = data.ContainsKey( questionAudioStr   ) ? audioClipPath + ( string )data[ questionAudioStr   ] : string.Empty;
            commentaryAudio = data.ContainsKey( commentaryAudioStr ) ? audioClipPath + ( string )data[ commentaryAudioStr ] : string.Empty;

            if ( 1 == commentType)
                quizData = new QuizData( question, list, answer, commentary, false, questionAudio, commentaryAudio );
            else if( 2 == commentType)
                quizData = new QuizData( question, list, answer, commentary_path, true, questionAudio, commentaryAudio );
            else if( 3 == commentType)
                quizData = new QuizData( question, list, answer, commentary, commentary_path, questionAudio, commentaryAudio );
            else
                quizData = new QuizData( question, list, answer, questionAudio, commentaryAudio );

            m_quizList.Add( quizData );

            //Debug.Log( $"{quizData.m_question} {quizData.m_type}" );

            commentType = 0;
        }
    }
}