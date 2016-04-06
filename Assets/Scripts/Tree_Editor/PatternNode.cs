using UnityEngine;
using NodeEditorFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif
[Node(false, "Game Node/Challenge/Single")]
public class PatternNode : BaseNode
{
    public const string ID = "patternNode";
    public override string GetID { get { return ID; } }

    [HideInInspector]
    public string pattern;
    [HideInInspector]
    public int patternIndex;

    public override Node Create(Vector2 pos)
    {
        PatternNode node = CreateInstance<PatternNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 175, 70);
        node.name = "Challenge";

        patternIndex = 0;

        node.CreateInput("", "Bool");
        node.CreateInput("", "Blocker");
        node.CreateOutput("", "Bool");

        return node;
    }

    protected override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Constants.Colors.Nodes.Pattern;

        if(rect.width != 175)
        {
            rect.width = 175;
            rect.height = 50;
        }

        base.DrawOutlinedNode();

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
        value = Inputs[0].GetValue<bool>();
        Outputs[0].SetValue<bool>(value);

        value = value && !Inputs[1].GetValue<bool>();

		if(TreeManager.instance != null)
        {
            TreeManager.instance.UpdateNode(this);
        }

        return true;
    }
}
