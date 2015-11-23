using UnityEngine;
using NodeEditorFramework;
using UnityEditor;
using System;

[Node(false, "Patterns Tree/Mastery/Pattern Node", false)]
public class PatternMasteryNode : BaseNode
{
    public const string ID = "patternMasteryNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor;

    [HideInInspector]
    public string pattern;
    [HideInInspector]
    public int patternIndex;
    [HideInInspector]
    public int tagIndex = -1;
    [HideInInspector]
    public string mastery;
    [HideInInspector]
    public int masteryIndex;

    [NonSerialized]
    private bool inserted;
    private bool oldValue;

    public override Node Create(Vector2 pos)
    {
        PatternMasteryNode node = CreateInstance<PatternMasteryNode>();
        node.rect = new Rect(pos.x, pos.y, 150, 75);
        node.name = "Pattern Mastery";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    public override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;

        base.DrawNode();

        GUI.backgroundColor = oldColor;
    }

    public override void NodeGUI()
    {
        base.NodeGUI();

        pattern = PatternManager.instance.patternsName[patternIndex];
        mastery = Mastery.values[masteryIndex];

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        for (int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].DisplayLayout();
        }

        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.fontSize = 12;
        guiStyle.normal.textColor = Color.white;
        guiStyle.fontStyle = FontStyle.Bold;
        GUILayout.Label(pattern.ToString(), guiStyle);

        string masteryString = "";
        if(tagIndex >= 0)
        {
            masteryString += tagIndex + " | ";
        }
        masteryString += Mastery.ToId(mastery);
        GUILayout.Label(masteryString, guiStyle);
        GUILayout.EndVertical();

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
        if(!inserted)
        {
            TreeManager.instance.AddPatternMasteryNode(this);
            inserted = true;
        }

        if (Inputs[0].connection != null)
        {
            value = Inputs[0].GetValue<bool>();
        }

        bool realValue = value && CalculateMastery();

        Outputs[0].SetValue<bool>(realValue);

        if (realValue)
        {
            nodeColor = new Color(1f, .65f, 0f);
        }
        else
        {
            nodeColor = new Color(1f, 0f, 0f);
        }

        return true;
    }

    public bool CalculateMastery()
    {
        return TreeManager.instance.IsPatternMasteryResolved(this);
    }
}

[CustomEditor(typeof(PatternMasteryNode))]
public class PatternMasteryNodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int patternIndex = ((PatternMasteryNode)target).patternIndex;
        int tagIndex = ((PatternMasteryNode)target).tagIndex;
        int masteryIndex = ((PatternMasteryNode)target).masteryIndex;

        EditorGUILayout.BeginVertical();
        patternIndex = EditorGUILayout.Popup("Pattern", patternIndex, PatternManager.instance.patternsName.ToArray(), EditorStyles.popup);
        tagIndex = EditorGUILayout.IntSlider("Tag", tagIndex, -1, PatternManager.instance.patternsInfo[PatternManager.instance.patternsName[patternIndex]].tags.Count - 1);
        masteryIndex = EditorGUILayout.Popup("Mastery", masteryIndex, Mastery.values.ToArray(), EditorStyles.popup);
        EditorGUILayout.EndVertical();

        ((PatternMasteryNode)target).patternIndex = patternIndex;
        ((PatternMasteryNode)target).tagIndex = tagIndex;
        ((PatternMasteryNode)target).masteryIndex = masteryIndex;
    }
}