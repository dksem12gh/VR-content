using GameManagers;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections;

public class GameStoryStep : MonoBehaviour
{
    //public static int   count = 0;

    [SerializeField] int    m_storyIdx = 0;
    public int storyIdx
    {
        get => m_storyIdx;
        set => m_storyIdx = value;
    }

    [Header("도착지점")]
    [SerializeField] TargetPosition     m_targetPosition = null;
    [Header("경유지점")]
    [SerializeField] TargetPosition[]   m_viaPoint = new TargetPosition[0];
    [Space(10)]
    [SerializeField] Interactable       m_actInteractable = null;

    [Header("나레이션만 출력되는 경우 event이후 자동으로 다음단계 이동")]
    [SerializeField] bool               m_autoSkip = false;

    //[SerializeField] AudioClip[]        m_audioClip;
    public Interactable ActInteractable
    {
        get => m_actInteractable;
    }

    string                  m_titleName = string.Empty;
    GameStoryData.StoryData m_storyData = null;

    bool    m_finishComment = false;
    public bool finishComment
    {
        set => m_finishComment = value;
        get => m_finishComment;
    }
    bool    m_isFinishState = false;
    public bool isFinishState
    {
        get => m_isFinishState;
    }

    int     m_viaIdx = 0;

    public GameStoryData.StoryData GetCurrStoryData()
    {
        return StoryManager.Instance.stroyData.GetStoryData( m_storyIdx );
    }

    public void ForceActInteractable()
    {
        if( null != m_actInteractable )
        {
            m_actInteractable.Interact();
        }
    }

    /// <summary>
    /// 현재 step을 시작한다
    /// </summary>
    [ContextMenu("Test")]
    public void StepStart(AudioSource audio)
    {
        if( GameEpisodeMgr.PLAY_TYPE.NORMAL == GameEpisodeMgr.Instance.m_currType )
        {
            m_finishComment = false;

            StartStepComment();
            StartMovingTarget();

            // 정섭
            if (audio != null)
            {
                if( null != audio.clip )
                    audio.Play();
            }
            //if (m_audioClip != null && m_audioClip.Length > 0) StepAudioPlay(audio, m_audioClip);
        }
        // 평가모드시에는 설명글, outline 나오지 않음 ( outline은 object 자체에서 disable 중 )
        else
        {

            //if( null != m_actInteractable )
            //{
            //    count++;
            //    Debug.Log( $"StartObjectActive {count}" );
            //}
            UIManager.Instance.ClosePopup();
            m_finishComment = true;
            StartObjectActive();
        }
    }

    //public void StepAudioPlay(AudioSource audio , AudioClip[] _audioClips)
    //{
    //    mStepAudioPlayCoroutine = StartCoroutine(@StepAudioPlayCoroutine(audio, _audioClips));
    //}

    //Coroutine mStepAudioPlayCoroutine = null;
    //IEnumerator @StepAudioPlayCoroutine(AudioSource audio, AudioClip[] _audioClips)
    //{
    //    audio.loop = false;

    //    foreach(AudioClip audioClip in _audioClips)
    //    {
    //        audio.clip = audioClip;
    //        audio.Play();
    //        while (audio.isPlaying == true)
    //            yield return null;
    //    }

    //}



    public bool ForceViewComment()
    {
        GameStoryData gameStoryData = GameStoryManager.Instance.stroyData;

        m_storyData = gameStoryData.GetStoryData( m_storyIdx );
        m_titleName = GameEpisodeMgr.Instance.PlayTitle;

        return UIManager.Instance.OpenPopup( m_titleName, m_storyData.m_comment, false );
    }

    void StartStepComment()
    {
        bool    isOnlyUI = false;
        GameStoryData gameStoryData = GameStoryManager.Instance.stroyData;

        m_storyData = gameStoryData.GetStoryData( m_storyIdx );
        m_titleName = GameEpisodeMgr.Instance.PlayTitle;

        //이동 포인트가 존재하면 UI에 의한 입력은 무시한다
        if( null == m_targetPosition )
        {
            if( false == GameSound.Instance.IsPlaying( eSoundID_Common.MoveEnter ) )
                GameSound.Instance.Play( eSoundID_Common.ActionFinish );

            if( null != m_actInteractable )
            {
                LActInteractable();
            }
            else
            {
                isOnlyUI = true;
                GameState.Instance.ChangeGameState( eGameState.UI );
            }
        }

        UIManager.Instance.OpenPopup( m_titleName, m_storyData.m_comment, isOnlyUI );

        async void LActInteractable()
        {
            m_actInteractable.Interact();
            while( true == m_actInteractable.bActiveReaction )
            {
                await UniTask.Yield();
            }

            //interactable 실행시 다음 step으로 넘어가는 입력이 들어왔을 경우에는
            //나레이션을 넘길수 있는 상태로 바꾼다
            if (true == m_isFinishState && false == m_autoSkip)
            {
                isOnlyUI = true;
                UIManager.Instance.OpenPopup( m_titleName, m_storyData.m_comment, isOnlyUI );
                GameState.Instance.ChangeGameState( eGameState.UI );
            }
            //interactable 실행이 되는 경우에는 input 상태를 변경시키지 않고
            //나레이션 출력 상태를 무시한다.
            else
            {
                FinishStepComment();
            }
        }
    }

    public void FinishStepComment()
    {
        m_finishComment = true;

        if ( null == m_actInteractable )
            FinishObjectAction();
    }

    void StartMovingTarget()
    {
        //이동 포인트가 존재하면 도착이후 오브젝트 활성화 정보를 실행시킨다
        if( null != m_targetPosition )
        {
            GameSound.Instance.Play( eSoundID_Common.MoveOpen );
            m_finishComment = true;

            //경유지가 존재하지 않음
            if( m_viaPoint.Length == 0 )
            {
                TargetPositionAct();
            }
            else
            {
                m_viaIdx = 0;
                ViaPointAct();
            }
        }
    }

    void TargetPositionAct()
    {
        m_targetPosition.arriveAct = ArriveTargetPosition;
        m_targetPosition.SetActive( true );
    }

    void ViaPointAct()
    {
        m_viaPoint[ m_viaIdx ].arriveAct = ViaPointFinish;
        m_viaPoint[m_viaIdx].SetActive( true );
    }

    void ViaPointFinish()
    {
        m_viaIdx++;
        if( m_viaIdx < m_viaPoint.Length )
        {
            ViaPointAct();
        }
        else
        {
            TargetPositionAct();
        }
    }

    void ArriveTargetPosition()
    {
        GameSound.Instance.Play( eSoundID_Common.MoveEnter );
        StartObjectActive();
    }

    /// <summary>
    /// 오브젝트 동작 시작
    /// </summary>
    void StartObjectActive()
    {
        if( null == m_actInteractable )
        {
            FinishObjectAction();
        }
        else
        {
            m_actInteractable.Interact();
        }
    }

    async public void FinishObjectAction()
    {
        m_isFinishState = true;
        while( false == m_finishComment )
        {
            await UniTask.Yield();
        }
        await UniTask.Yield();

        GameStoryManager.Instance.NextStory();
    }    
}