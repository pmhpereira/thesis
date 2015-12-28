﻿using UnityEngine;
using NodeEditorFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Node(false, "Game Node/Challenge", false)]
public class PatternNode : BaseNode
{
    public const string ID = "patternNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor = new Color(0.6f, 0.4f, 0.2f);

    [HideInInspector]
    public string pattern;
    [HideInInspector]
    public int patternIndex;

    private bool oldValue;

    public override Node Create(Vector2 pos)
    {
        PatternNode node = CreateInstance<PatternNode>();
        node.rect = new Rect(pos.x, pos.y, 175, 50);
        node.name = "Challenge";

        patternIndex = 0;

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    public override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;

        if(rect.width != 175)
        {
            rect.width = 175;
            rect.height = 50;
        }

        base.DrawNode();

        GUI.backgroundColor = oldColor;
    }

    public override void NodeGUI()
    {
        base.NodeGUI();

        pattern = PatternManager.instance.patternsName[patternIndex];

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        for (int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].DisplayLayout();
        }
        GUILayout.EndVertical();

        #if UNITY_EDITOR
            patternIndex = EditorGUILayout.Popup("", patternIndex, PatternManager.instance.patternsName.ToArray(), GUILayout.MaxWidth(rect.width * .8f));
        #else
            GUILayout.Label(pattern);
        #endif
        GUILayout.Label(Mastery.ToId(TreeManager.instance.GetPatternMastery(pattern)));

        GUILayout.BeginVertical();
        for (int i = 0; i < Outputs.Count; i++)
        {
            Outputs[i].DisplayLayout();
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
    }

    public override bool Calculate()
    {
        if (Inputs[0].connection != null)
        {
            value = Inputs[0].GetValue<bool>();
        }
        else
        {
            value = true;
        }

        Outputs[0].SetValue<bool>(value);

        if (value != oldValue)
        {
            oldValue = value;
            UpdateTreeManager();
        }

        return true;
    }

    void UpdateTreeManager()
    {
        if (Application.isPlaying)
        {
            TreeManager.instance.UpdatePattern(this);
        }
    }
}
