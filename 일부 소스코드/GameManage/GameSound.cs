using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagers
{
    public class GameSound : SingletoneMono<GameSound>
    {
        private class SoundPlayList
        {
            private Dictionary<int, MySoundSource> m_dicSources;

            public SoundPlayList()
            {
                m_dicSources = new Dictionary<int, MySoundSource>();
            }
            public MySoundSource this[int _id]
            {
                get
                {
                    MySoundSource source;
                    m_dicSources.TryGetValue(_id, out source);
                    return source;
                }
                set
                {
                    m_dicSources[_id] = value;
                }
            }
        }

        private Dictionary<eSoundIDType, SoundPlayList> m_SoundGroup = new Dictionary<eSoundIDType, SoundPlayList>(new Compares.SoundIDCompare());
        //private Dictionary<eNPCSoundType, SoundSourceNPC> m_NPCSounds;
        protected override void Awake()
        {
            base.Awake();
            //m_SoundGroup = new Dictionary<eSoundIDType, SoundPlayList>(new Compares.SoundIDCompare());
            //m_NPCSounds = new Dictionary<eNPCSoundType, SoundSourceNPC>();
        }

        #region General
        public void RegistSoundSource(eSoundIDType _group, int _id, MySoundSource _source )
        {
            if (!m_SoundGroup.ContainsKey(_group))
                m_SoundGroup.Add(_group, new SoundPlayList());

            m_SoundGroup[_group][_id] = _source;
        }
        public void UnRegistSoundSource(eSoundIDType _group, int _id)
        {
            if (m_SoundGroup != null)
            {
                m_SoundGroup[_group][_id] = null;
            }
        }

        private void Play(eSoundIDType _group, int _id)
        {
            MySoundSource source = m_SoundGroup[_group][_id];
            if (source != null)
            {
                source.Play();
                //Debug.Log(_group + "  :  " + _id + "  :  " + _pos + "  :  " + _startTime);
            }
            else Debug.Log("Call Play not registed Group  /  " + _group + "  " + _id);
        }
        private void Play(eSoundIDType _group, int _id, Vector3 _pos, float _startTime)
        {
            MySoundSource source = m_SoundGroup[_group][_id];
            
            if (source != null)
            {
                source.Play(_pos, _startTime); ;
                //Debug.Log(_group + "  :  " + _id + "  :  " + _pos + "  :  " + _startTime);
            }
            else Debug.Log("Call Play not registed Group  /  " + _group + "  " + _id);
        }
        public void Play(eSoundID_BGM _id)      { Play(eSoundIDType.BGM,        (int)_id); }
        public void Play(eSoundID_Ambient _id)  { Play(eSoundIDType.Ambient,    (int)_id); }
        public void Play(eSoundID_Common _id)   { Play(eSoundIDType.Common,     (int)_id); }        
        //public void Play(eSoundID_EP2 _id)      { Play(eSoundIDType.EP2,        (int)_id); }

        public void Play(eSoundID_BGM _id, Vector3 _pos = default(Vector3), float _startTime = 0)
        {
            Play(eSoundIDType.BGM, (int)_id, _pos, _startTime);
        }

        public void Play(eSoundID_Ambient _id, Vector3 _pos = default(Vector3), float _startTime = 0)
        {
            Play(eSoundIDType.Ambient, (int)_id, _pos, _startTime);
        }

        public void Play(eSoundID_Common _id, Vector3 _pos = default(Vector3), float _startTime = 0)
        {
            Play(eSoundIDType.Common, (int)_id,_pos, _startTime);
        }
        
        //public void Play(eSoundID_EP2 _id, Vector3 _pos = default(Vector3), float _startTime = 0)
        //{ Play(eSoundIDType.EP2, (int)_id, _pos, _startTime); }

        private void Stop(eSoundIDType _group, int _id)
        {
            MySoundSource source = m_SoundGroup[_group][_id];

            if (source != null)
                source.Stop();
            else
                Debug.Log("Call Play not registed Group  /  " + _group + "  " + _id);
        }
        public void Stop(eSoundID_BGM _id) { Stop(eSoundIDType.BGM, (int)_id); }
        public void Stop(eSoundID_Ambient _id) { Stop(eSoundIDType.Ambient, (int)_id); }
        public void Stop(eSoundID_Common _id)   { Stop(eSoundIDType.Common, (int)_id);  }        
        //public void Stop(eSoundID_EP2 _id) { Stop(eSoundIDType.EP2, (int)_id); }


        private float GetClipLength(eSoundIDType _group, int _id)
        {
            MySoundSource source = m_SoundGroup[_group][(int)_id];
            if (source == null) return 0;
            else                return source.ClipLength;
        }
        public float GetClipLength(eSoundID_Common _id) { return GetClipLength(eSoundIDType.Common, (int)_id); }
        public bool  IsPlaying( eSoundID_Common _id )
        {
            return m_SoundGroup[ eSoundIDType.Common ][ ( int )_id ].isPlaying;
        }
        #endregion
    }
}
