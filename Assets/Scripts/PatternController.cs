using System;
using UnityEngine;
using UnityEngine.UI;

public class PatternController : MonoBehaviour
{
    [HideInInspector]
    public float length;

    private MeshRenderer debugRenderer;
    private Material debugRendererMaterial;
    private LineRenderer startLineRenderer, endLineRenderer;

    private BoxCollider2D exitCollider;

    private Text text;

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
        DebugPatternInfo();
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

        Canvas canvas = debugChild.AddComponent<Canvas>();
        RectTransform canvasRectTransform = canvas.GetComponent<RectTransform>();

        Vector3 canvasPosition = new Vector3();
        canvasPosition.x = length / 2 - 0.5f;
        canvasPosition.z = -1;
        canvasRectTransform.localPosition = canvasPosition;
        canvasRectTransform.sizeDelta = new Vector2(length * 100, 100);
        canvasRectTransform.localScale /= 100;

        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = Camera.main;

        text = debugChild.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 40;
        text.alignment = TextAnchor.MiddleCenter;
        text.horizontalOverflow = HorizontalWrapMode.Overflow;
        text.verticalOverflow = VerticalWrapMode.Overflow;
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

    void DebugPatternInfo()
    {
        if (!GameManager.instance.debugMode)
        {
            text.text = "";
            return;
        }

        string patternName = transform.name;
        float patternScore = PatternManager.instance.patternsInfo[transform.name].GetScore();
        text.text = patternName + "\n" + Math.Round(patternScore, 3);
    }
}
