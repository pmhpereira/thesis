using UnityEngine;
using NodeEditorFramework;
using UnityEditor;
using System;

[Node(false, "Patterns Tree/Mastery/Tag Node", false)]
public class TagMasteryNode : BaseNode
{
    public const string ID = "tagMasteryNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor;

    [HideInInspector]
    public string tag;
    [HideInInspector]
    public int tagIndex;
    [HideInInspector]
    public string mastery;
    [HideInInspector]
    public int masteryIndex;

    [NonSerialized]
    private bool inserted;

    private bool oldValue;

    public override Node Create(Vector2 pos)
    {
        TagMasteryNode node = CreateInstance<TagMasteryNode>();
        node.rect = new Rect(pos.x, pos.y, 150, 75);
        node.name = "Tag Mastery";

        tagIndex = 0;
        masteryIndex = 0;

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

        tag = Tag.values[tagIndex];
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
        GUILayout.Label(tag.ToString(), guiStyle);

        GUILayout.Label(Mastery.ToId(mastery), guiStyle);
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
        if (!inserted)
        {
            TreeManager.instance.AddTagMasteryNode(this);
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

        if (value != oldValue)
        {
            oldValue = value;
        }

        return true;
    }

    public bool CalculateMastery()
    {
        return TreeManager.instance.IsTagMasteryResolved(this);
    }
}

[CustomEditor(typeof(TagMasteryNode))]
public class TagMasteryNodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int tagIndex = ((TagMasteryNode)target).tagIndex;
        int masteryIndex = ((TagMasteryNode)target).masteryIndex;

        EditorGUILayout.BeginVertical();
        tagIndex = EditorGUILayout.Popup("Tag", tagIndex, Tag.values.ToArray(), EditorStyles.popup, GUILayout.ExpandWidth(true));
        masteryIndex = EditorGUILayout.Popup("Mastery", masteryIndex, Mastery.values.ToArray(), EditorStyles.popup, GUILayout.ExpandWidth(true));
        EditorGUILayout.EndVertical();

        ((TagMasteryNode)target).tagIndex = tagIndex;
        ((TagMasteryNode)target).masteryIndex = masteryIndex;
    }
}