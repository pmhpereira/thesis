using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/Timer")]
public class TimerNode : BaseNode
{
    public const string ID = "timerNode";
    public override string GetID { get { return ID; } }

	public float timer;
	private float auxTimer;
	private float start;

    public override Node Create(Vector2 pos)
    {
        TimerNode node = CreateInstance<TimerNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 180, 50);
        node.name = "Timer";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    protected override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Constants.Colors.Nodes.Timer;

        if(rect.width != 180)
        {
            rect.width = 180;
            rect.height = 50;
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

        #if UNITY_EDITOR
			timer = EditorGUILayout.FloatField("", timer, GUILayout.MaxWidth(rect.width - 20));
        #else
            GUILayout.Label(timer + "");
        #endif

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

		if(value && start == 0)
		{
			start = Time.realtimeSinceStartup;
			auxTimer = timer;
		}
		
		if(value)
		{
			if(Time.timeScale > 0)
			{
				timer = auxTimer - (Time.realtimeSinceStartup - start);
				timer = Mathf.Max(0, timer);
			}
		}

		if(timer == 0)
		{
			Outputs[0].SetValue<bool>(value);
		}
		else
		{
			Outputs[0].SetValue<bool>(false);
		}

        return true;
    }
}
