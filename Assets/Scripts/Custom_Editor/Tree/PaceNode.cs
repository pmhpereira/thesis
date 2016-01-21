using UnityEngine;
using NodeEditorFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/Pace", false)]
public class PaceNode : BaseNode
{
    public const string ID = "paceNode";
    public override string GetID { get { return ID; } }
    private Color nodeColor = new Color(1f, .4f, 0f);

    [HideInInspector]
    public int paceIndex;
    [HideInInspector]
    public int instancesCount;
    [HideInInspector]
    public string paceName;

    public override Node Create(Vector2 pos)
    {
        PaceNode node = CreateInstance<PaceNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 150, 80);
        node.name = "Pace";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    public override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = nodeColor;

        base.DrawOutlinedNode();

        GUI.backgroundColor = oldColor;
    }

    public override void NodeGUI()
    {
        base.NodeGUI();

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        for (int i = 0; i < Inputs.Count; i++)
        {
            Inputs[i].DisplayLayout();
        }
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        #if UNITY_EDITOR
            float oldLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 40;
            paceName = EditorGUILayout.TextField("Name", paceName, GUILayout.MaxWidth(rect.width - 20));
            paceIndex = EditorGUILayout.Popup("", paceIndex, Pace.values.ToArray(), GUILayout.MaxWidth(rect.width - 20));
            EditorGUIUtility.labelWidth = 70;
            instancesCount = EditorGUILayout.IntField("Instances", instancesCount);
            instancesCount = Mathf.Max(1, instancesCount);
            EditorGUIUtility.labelWidth = oldLabelWidth;
        #else
            GUILayout.Label(paceName);
            GUILayout.Label(Pace.values[paceIndex]);
            GUILayout.Label("Instances: " + instancesCount);
        #endif
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
        if (Inputs[0].connection != null)
        {
            value = Inputs[0].GetValue<bool>();
        }

        if(TreeManager.instance != null)
        {
            TreeManager.instance.UpdateNode(this);
        }

        Outputs[0].SetValue<bool>(value);

        return true;
    }

    public override void OnDelete()
    {
        TreeManager.instance.RemoveNode(this);
    }
}
