﻿using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Node(false, "Game Node/Timer", 1)]
public class TimerNode : BaseNode
{
    public const string ID = "timerNode";
    public override string GetID { get { return ID; } }

	private float timer;
	public bool resetOnDisable = true;
	public float initialTimer;
	private float timerStart;

    public override Node Create(Vector2 pos)
    {
        TimerNode node = CreateInstance<TimerNode>();
        node.creationId = GetNextId();
        node.rect = new Rect(pos.x, pos.y, 180, 75);
        node.name = "Timer";

        node.CreateInput("", "Bool");
        node.CreateOutput("", "Bool");

        return node;
    }

    protected override void DrawNode()
    {
        Color oldColor = GUI.backgroundColor;
        GUI.backgroundColor = Constants.Colors.Nodes.Timer;

        if(rect.width != 180 || rect.height != 75)
        {
            rect.width = 180;
            rect.height = 75;
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
			timer = EditorGUILayout.IntField("", Mathf.CeilToInt(timer), GUILayout.MaxWidth(rect.width - 20));
			resetOnDisable = EditorGUILayout.Toggle("Reset on disable", resetOnDisable);
        #else
	        GUILayout.Label(timer + "");
            GUILayout.Label("Reset on disable: " + resetOnDisable);
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
        value = Inputs[0].GetValue<bool>();

		if(timerStart == 0)
		{
			if(initialTimer > 0)
			{
				timer = initialTimer;
			}
			else
			{
				initialTimer = timer;
			}
		}

		if(value)
		{
			if(timerStart == 0)
			{
				timerStart = Time.realtimeSinceStartup;
			}

			if(Time.timeScale > 0)
			{
				timer = initialTimer - (Time.realtimeSinceStartup - timerStart);
				timer = Mathf.Max(0, timer);
			}
			else
			{
				timerStart += timer - (initialTimer - (Time.realtimeSinceStartup - timerStart));
			}
		}
		else
		{
			if(resetOnDisable)
			{
				timer = initialTimer;
				timerStart = 0;
			}
			else
			{
				timerStart += timer - (initialTimer - (Time.realtimeSinceStartup - timerStart));
			}
		}

		Outputs[0].SetValue<bool>(timer == 0 && value);

        return true;
    }
}
