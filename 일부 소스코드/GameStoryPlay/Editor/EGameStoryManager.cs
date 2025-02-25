using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameStoryManager))]
public class EGameStoryManager : Editor
{
    protected GameStoryManager m_targetObject;
    protected virtual void OnEnable()
    {
        m_targetObject = (GameStoryManager)target;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("스토리 인덱스 배열 순서로 적용"))
        {
            for (int i = 0; i < m_targetObject.StepLength; i++)
            {
                m_targetObject.getGameStoryStp(i).storyIdx = i;

                string name = m_targetObject.getGameStoryStp(i).name;


                int end = name.IndexOf('-');

                string str = name.Substring(end + 1, name.Length - end - 1);

                str = "0_" + i.ToString() + " -" + str;


                m_targetObject.getGameStoryStp(i).name = str;
                Debug.Log(str);

            }
        }
    }
}
