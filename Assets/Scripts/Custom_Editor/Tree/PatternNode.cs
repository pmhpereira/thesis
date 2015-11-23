using UnityEngine;
using NodeEditorFramework;
using UnityEditor;

[Node(false, "Patterns Tree/Pattern Node", false)]
public class PatternNode : BaseNode
{
    public const string ID = "patternNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor;

    [HideInInspector]
    public string pattern;
    [HideInInspector]
    public int patternIndex;

    private bool oldValue;

    public override Node Create(Vector2 pos)
    {
        PatternNode node = CreateInstance<PatternNode>();
        node.rect = new Rect(pos.x, pos.y, 150, 50);
        node.name = "Pattern";

        patternIndex = 0;

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

        GUILayout.FlexibleSpace();

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical();

        for (int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].DisplayLayout();
        }

        GUILayout.EndVertical();

        GUIStyle guiStyle = new GUIStyle();
        guiStyle.alignment = TextAnchor.MiddleCenter;
        guiStyle.fontSize = 16;
        guiStyle.normal.textColor = Color.white;
        guiStyle.fontStyle = FontStyle.Bold;
        GUILayout.Label(pattern.ToString(), guiStyle);

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

        Outputs[0].SetValue<bool>(value);

        if (value)
        {
            nodeColor = new Color(0.8f, .65f, 0f);
        }
        else
        {
            nodeColor = new Color(0.5f, 0.3f, 0.1f);
        }

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

[CustomEditor(typeof(PatternNode))]
public class PatternNodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        int index = ((PatternNode)target).patternIndex;

        EditorGUILayout.BeginVertical();
        index = EditorGUILayout.Popup("Pattern", index, PatternManager.instance.patternsName.ToArray(), EditorStyles.popup);
        EditorGUILayout.EndVertical();

        ((PatternNode)target).patternIndex = index;
    }
}