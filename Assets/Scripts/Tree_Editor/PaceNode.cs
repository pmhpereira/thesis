using UnityEngine;
using NodeEditorFramework;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/Pace/Single", 4)]
public class PaceNode : BaseNode
{
    public const string ID = "paceNode";
    public override string GetID { get { return ID; } }

    [HideInInspector]
    public int paceIndex;
    [HideInInspector]
    public int instancesCount;
    [HideInInspector]
    public string paceName = "";

    public override Node Create(Vector2 pos)
    {
        PaceNode node = CreateInstance<PaceNode>();
        node.creationId = GetNextId();
		node.name = "Pace";
        node.rect = new Rect(pos.x, pos.y, 170, 80);

        node.CreateInput("", "Bool");
        node.CreateInput("", "Blocker");

        node.CreateOutput("", "Bool");

        return node;
    }

    protected internal override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Constants.Colors.Nodes.Pace;
		
        if(rect.width != 170)
        {
            rect.width = 170;
            rect.height = 80;
        }

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
            paceName = paceName.Replace(' ', '_');
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

        if(PaceManager.instance.pacesInfo.ContainsKey(paceName))
        {
            GUILayout.Label(Mastery.ToId(PaceManager.instance.pacesInfo[paceName].GetMastery()));
        }

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

    protected internal override void OnDelete()
    {
        TreeManager.instance.RemoveNode(this);
    }
}
