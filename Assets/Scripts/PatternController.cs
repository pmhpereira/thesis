using System;
using UnityEngine;

public class PatternController : MonoBehaviour
{
    [HideInInspector]
    public float length;

    private MeshRenderer debugRenderer;
    private Material debugRendererMaterial;
    private LineRenderer startLineRenderer, endLineRenderer;

    private BoxCollider2D exitCollider;

    void Awake()
    {
        foreach(Transform child in transform)
        {
            float childSize = child.position.x + child.localScale.x;
            if (childSize > length)
            {
                length = childSize;
            }
        }
    }

    void Start()
    {
        SetupDebugMode();
        SetupExitCollider();
    }

    void Update()
    {
        startLineRenderer.SetVertexCount(0);
        endLineRenderer.SetVertexCount(0);

        DebugPatternLimits();
    }

    void SetupDebugMode()
    {
        GameObject debugChild = new GameObject();
        debugChild.name = "Debug";
        debugChild.transform.SetParent(this.transform);
        debugChild.transform.localPosition = Vector3.zero;

        debugRendererMaterial = new Material(Shader.Find("Particles/Additive"));

        GameObject leftLineRenderer = new GameObject();
        leftLineRenderer.name = "Left Line Renderer";
        leftLineRenderer.transform.SetParent(debugChild.transform);

        GameObject rightLineRenderer = new GameObject();
        rightLineRenderer.name = "Right Line Renderer";
        rightLineRenderer.transform.SetParent(debugChild.transform);

        startLineRenderer = leftLineRenderer.AddComponent<LineRenderer>();
        startLineRenderer.SetColors(Color.green, Color.green);
        startLineRenderer.SetWidth(0.05f, 0.05f);
        startLineRenderer.material = debugRendererMaterial;

        endLineRenderer = rightLineRenderer.AddComponent<LineRenderer>();
        endLineRenderer.SetColors(Color.red, Color.red);
        endLineRenderer.SetWidth(0.05f, 0.05f);
        endLineRenderer.material = debugRendererMaterial;
    }

    void DebugPatternLimits()
    {
        if (!GameManager.instance.debugMode)
        {
            return;
        }

        startLineRenderer.SetVertexCount(2);
        startLineRenderer.SetPosition(0, transform.position + new Vector3(-0.5f, -1, 0));
        startLineRenderer.SetPosition(1, transform.position + new Vector3(-0.5f, 5, 0));

        endLineRenderer.SetVertexCount(2);
        endLineRenderer.SetPosition(0, transform.position + new Vector3(-0.5f + length, -1, 0));
        endLineRenderer.SetPosition(1, transform.position + new Vector3(-0.5f + length, 5, 0));
    }

    void SetupExitCollider()
    {
        GameObject collider = new GameObject("Exit Collider");
        collider.AddComponent<PatternCheckpoint>();
        collider.transform.SetParent(this.transform);
        collider.transform.localPosition = Vector3.zero;

        exitCollider = collider.AddComponent<BoxCollider2D>();
        exitCollider.isTrigger = true;
        exitCollider.offset = new Vector2(length - 0.5f, 2);
        exitCollider.size = new Vector2(0.1f, 6);
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Player")
        {
            PatternManager.instance.patternsInfo[gameObject.name].AddAttempt(false);
            Destroy(exitCollider);
        }
    }

    void OnGUI()
    {
        if (!GameManager.instance.debugMode)
        {
            return;
        }

        float score = PatternManager.instance.patternsInfo[transform.name].GetScore();
        string scoretext = "" + Math.Round(score, 3);
        GUIContent guiContent = new GUIContent(transform.name + "\n" + scoretext);

        GUIStyle guiStyle = GUI.skin.GetStyle("Label");
        guiStyle.alignment = TextAnchor.LowerCenter;
        guiStyle.fontSize = 14;

        Vector2 guiSize = guiStyle.CalcSize(guiContent);

        var rect = new Rect();
        var offset = -transform.localScale / 2;
        Vector3 lowerLeftPoint = Camera.main.WorldToScreenPoint(transform.position + offset);
        Vector3 upperRightPoint = Camera.main.WorldToScreenPoint(transform.position + offset + new Vector3(length, 0, 0));

        rect.x = (upperRightPoint.x + lowerLeftPoint.x - guiSize.x) / 2;
        rect.y = 0;
        rect.width = guiSize.x;
        rect.height = Screen.height - lowerLeftPoint.y;

        GUI.Label(rect, guiContent, guiStyle);
    }
}
