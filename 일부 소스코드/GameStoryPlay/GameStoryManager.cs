using System.Collections;
using System.Collections.Generic;
using GameManagers;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class StoryManager : MonoBehaviour
{
    static protected StoryManager instance = null;
    public static StoryManager Instance
    {
        get => instance;
    }

    protected GameStoryData m_storyData = null;
    public GameStoryData stroyData
    {
        get => m_storyData;
    }

    [SerializeField] protected Interactable m_init_interactable   = null;

    public abstract void Init();
    public abstract void StartInit();
    public abstract void StartStory();
    public abstract void Relese();
    public abstract void NextStory();

    public abstract void setStep(ePlayArea area , int _step);
    public abstract void FinishCurrStep( ePlayArea area );

    public abstract GameStoryStep GetCurrMyStep
    {
        get;
    }
    public abstract GameStoryStep GetNextMyStep
    {
        get;
    }
}

public class GameStoryManager : StoryManager, IUIInputCall
{
    [SerializeField]    GameStoryStep[] m_storyStep = null;
    [SerializeField]    AudioClip[]     m_voiceStep = null;
    [SerializeField]    AudioClip       m_clearClip = null;
    [Space]
    [SerializeField]    AudioSource     m_audio = null;

    int m_currStep = 0;

    public override GameStoryStep GetCurrMyStep
    {
        get => m_storyStep[ m_currStep ];
    }

    public override GameStoryStep GetNextMyStep
    {
        get
        {
            int nextStep = m_currStep + 1;

            if( nextStep < m_storyStep.Length )
            {
                return m_storyStep[ nextStep ];
            }
            else
                return null;
        }
    }

    public int StepLength
    {
        get => m_storyStep.Length;
    }

    public GameStoryStep getGameStoryStp(int _index)
    {
        return m_storyStep[_index];
    }
    public override void Init()
    {
        //VRPlayerManager.Instance.ChangeUIAction( VRUIInputState.DEFUALT );

        instance    = this;
        m_storyData = new GameStoryData();
    }

    public override void StartInit()
    {
        if( null != m_init_interactable )
            m_init_interactable.Interact();
    }

    public override void Relese()
    {
        instance = null;
    }

    public override void StartStory()
    {
        //GameStoryStep.count = 0;
        m_currStep = -1;
        NextStory();
    }




    // GameStoryStep 이외에서 호출 시키면 안됨 //
    //GameStoryManager.Instance.FinishCurrStep(m_playerArea ); // story step을 종료시켜줘야뎀

    [ContextMenu("test")]
    public override void NextStory()
    {
        //if( GameArea.IsMyAreaIsA )
        //{
            //Debug.Log( $"NextStory {m_currStep}" );
            m_currStep++;
            if( m_currStep < m_storyStep.Length )
            {
                // 정섭
                if( m_voiceStep != null && m_voiceStep.Length >= 0 )
                {
                    if (m_currStep < m_voiceStep.Length)
                    {
                        m_audio.clip = m_voiceStep[m_currStep];
                    }
                    else
                        m_audio.clip = null;
                }

                m_storyStep[ m_currStep ].StepStart( m_audio );
                UIHand.SetUIInputCall = this;
            }
            else
            {
                FinishStory();
                //Debug.Log($"GameStoryStep {GameStoryStep.count}" ); 
            }
        //}
    }

    public override void FinishCurrStep( ePlayArea area = ePlayArea.A )
    {
        if( m_currStep < m_storyStep.Length )
        {
            m_storyStep[ m_currStep ].FinishObjectAction();
        }
    }

    [ContextMenu( "Finish Test" )]
    void FinishStory()
    {
        bool    multiSelect = GameEpisodeMgr.Instance.CheckNextMultiStory();

        UIManager.Instance.ClosePopup();

        if (null != m_clearClip)
        {
            m_audio.clip = m_clearClip;
            m_audio.Play();
        }
        UIManager.Instance.OpenFinishUI( GameEpisodeMgr.Instance.SetNextSubEpisode(), multiSelect );
    }

    bool    m_isActive = false;
    [ContextMenu( "test2" )]
    public void TriggerDown()
    {
        if( false == m_isActive )
        {
                UIHand.PopUIInputCall();
                if( m_currStep < m_storyStep.Length )
                {
                    GameState.Instance.ChangeGameState(eGameState.Play);
                    m_storyStep[ m_currStep ].FinishStepComment();
                    WaitDelay();
                }
        }
    }

    async   void WaitDelay()
    {
        m_isActive = true;
        await UniTask.Delay( 100 );
        m_isActive = false;
    }

    public override void setStep(ePlayArea area = ePlayArea.A , int _step = 0)
    {
        m_currStep = _step - 1;

        if (m_currStep < m_storyStep.Length)
        {
            m_storyStep[m_currStep].finishComment = true;
            m_storyStep[m_currStep].FinishObjectAction();
        }
    }


#if UNITY_EDITOR

    string inputTex = string.Empty;
    private void OnGUI()
    {
        if( instance == this )
        {
            GUILayout.BeginArea( new Rect( Screen.width - 110.0f, 10.0f, 100.0f, 500.0f ) );
            inputTex = GUILayout.TextField( inputTex );
            if( GUILayout.Button( "진행 변경" ) )
            {
                if( true == int.TryParse( inputTex.Trim(), out int step ) )
                {
                    step = Mathf.Clamp( step, 0, m_storyStep.Length - 1 );

                    m_currStep = step - 1;

                    NextStory();

                }
            }
            GUILayout.EndArea();
        }
    }
#endif
}