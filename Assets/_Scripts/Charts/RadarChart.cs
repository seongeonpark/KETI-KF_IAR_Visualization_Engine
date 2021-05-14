using TMPro;
using UnityEngine;

public class RadarChart : MonoBehaviour
{
    #region Server
    
    [Header("Server")]
    public GetXMLs XML;
    
    private VirtualServer mVirtualServer;

    #endregion

    #region Chart customizing var
    
    [Header("Data")]
    [SerializeField] private Sensor[] Sensors;
    [Header("Background UI")]
    [SerializeField] private bool HasBackgroundMesh = false;
    [SerializeField] private Material BackgroundMeshMaterial;
    [Space]
    [SerializeField] private int BackgroundGridCount = 3;
    [Space]
    [SerializeField] private bool HasAxis = true;
    [SerializeField] private Material BackgroundLineMaterial;
    [SerializeField] private float AxisThickness = 0.2f;
    [Header("Chart UI")]
    [SerializeField] private bool HasChartMesh = true;
    [SerializeField] private Material radarMaterial;
    [Space]
    [SerializeField] private bool HasChartLine = true;
    [SerializeField] private Material RadarLineMaterial;
    [SerializeField] private float RadarLineThickness = 0.005f;
    [Header("Label UI")]
    [SerializeField] private float FontSize = 250f;
    [SerializeField] private float Location = 1.2f;
    [SerializeField] private Color FontColor = new Color(255, 255, 255, 255);
    [SerializeField] private TMP_FontAsset FontAsset;
    
    #endregion

    private CanvasRenderer mBackgroundMeshCanvasRenderer;
    private LineRenderer[] mBackgroundLineRenderers;
    private LineRenderer[] mBackgroundPolyRenderers;
    private CanvasRenderer mRadarMeshCanvasRenderer;
    private LineRenderer mRadarLineRenderer;

    private const float CHART_SIZE = 290f;
    private float mAngleIncrement;

    private void Start()
    {
        #region Server

        // Recieve data from server
        mVirtualServer = transform.GetComponent<VirtualServer>();
        
        #endregion

        // Chart
        mAngleIncrement = 360f / Sensors.Length;
        mBackgroundPolyRenderers = new LineRenderer[BackgroundGridCount];
        mBackgroundLineRenderers = new LineRenderer[Sensors.Length];

        var backgroundObj = transform.Find("background").GetComponent<RectTransform>();

        // #1. draw background mesh
        if (HasBackgroundMesh)
        {
            GameObject backgroundMesh = new GameObject("backgroundMesh", typeof(CanvasRenderer));
            backgroundMesh.transform.SetParent(backgroundObj, false);
            mBackgroundMeshCanvasRenderer = backgroundMesh.transform.GetComponent<CanvasRenderer>();
        }

        // #2. draw background lines
        if (BackgroundGridCount > 0)
        {
            for (int i = 0; i < BackgroundGridCount; i++)
            {
                GameObject backgroundLine = new GameObject("backgroundLine", typeof(LineRenderer));
                backgroundLine.transform.SetParent(backgroundObj, false);
                mBackgroundPolyRenderers[i] = backgroundLine.transform.GetComponent<LineRenderer>();
            }
        }

        if (HasAxis)
        {
            for (int i = 0; i < Sensors.Length; i++)
            {
                GameObject backgroundLine = new GameObject("backgroundLine", typeof(LineRenderer));
                backgroundLine.transform.SetParent(backgroundObj, false);
                mBackgroundLineRenderers[i] = backgroundLine.transform.GetComponent<LineRenderer>();
            }
        }

        // #3. draw radar chart
        if (HasChartMesh)
        {
            mRadarMeshCanvasRenderer = transform.Find("radarMesh").GetComponent<CanvasRenderer>();
        }

        if (HasChartLine)
        {
            GameObject backgroundLine = new GameObject("backgroundLine", typeof(LineRenderer));
            backgroundLine.transform.SetParent(backgroundObj, false);
            mRadarLineRenderer = backgroundLine.transform.GetComponent<LineRenderer>();
        }

        // #4. create label
        var textContainer = transform.Find("TextContainer").GetComponent<RectTransform>();
        string[] names = new string[Sensors.Length];
        for (int i = 0; i < Sensors.Length; i++)
        {
            names[i] = Sensors[i].name;
        }
        CreateLabel(textContainer, names, FontSize, Location, FontColor, FontAsset);
    }

    private void Update()
    {
        //float[] IoTData = Normalize(XML.Data, Sensors);
        float[] IoTData = Normalize(mVirtualServer.IoTData, Sensors);

        if (HasBackgroundMesh)
        {
            DrawBackground(Sensors.Length);
        }

        if (BackgroundGridCount > 0)
        {
            for (int i = 0; i < BackgroundGridCount; i++)
            {
                float size = CHART_SIZE * (i + 1) / BackgroundGridCount;
                DrawBackgroundPolygon(mBackgroundPolyRenderers[i], Sensors.Length, size, Vector3.zero, AxisThickness, AxisThickness);
            }
        }

        if (HasAxis)
        {
            for (int i = 0; i < Sensors.Length; i++)
            {
                var center = Vector3.zero;
                var pos = Quaternion.Euler(0, 0, -mAngleIncrement * i) * Vector3.up * CHART_SIZE * 1f;
                DrawBackgroundLine(mBackgroundLineRenderers[i], center, pos, AxisThickness, AxisThickness);
            }
        }

        if (HasChartMesh)
        {
            //DrawRadarChart(Items.Length, mVirtualServer.IoTData);
            DrawRadarChart(Sensors.Length, IoTData);
        }

        if (HasChartLine)
        {
            var center = Vector3.zero;

            //DrawRadarLine(mRadarLineRenderer, Items.Length, mVirtualServer.IoTData, center, RadarLineThickness, RadarLineThickness);
            DrawRadarLine(mRadarLineRenderer, Sensors.Length, IoTData, center, RadarLineThickness, RadarLineThickness);
        }
    }

    // Normalize Server (XML) data to Sensors (ruled) data
    private float[] Normalize(float[] server, Sensor[] sensors)
    {
        var data = new float[sensors.Length];

        for (int i = 0; i < sensors.Length; i++)
        {
            var x = Mathf.Clamp(server[i], sensors[i].min, sensors[i].max);
            data[i] = (x - sensors[i].min) / (sensors[i].max - sensors[i].min);
        }

        return data;
    }

    private void DrawRadarChart(int itemCount, float[] values)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[3 * itemCount];
        Vector2[] uv = new Vector2[3 * itemCount];
        int[] triangles = new int[3 * itemCount];

        int[] itemIndexes = new int[itemCount];

        // Center of radar chart
        vertices[0] = Vector3.zero;

        // Items of radar chart
        for (int i = 0; i < itemCount; i++)
        {
            vertices[i + 1] = Quaternion.Euler(0, 0, -mAngleIncrement * i) * Vector3.up * CHART_SIZE * values[i];

            itemIndexes[i] = i + 1;
        }

        // order of drawing triangles
        int drawingOrder = 0;
        for (int i = 0; i < itemCount; i++)
        {
            triangles[drawingOrder] = 0;                    // center of radar chart
            triangles[drawingOrder + 1] = itemIndexes[(i % itemCount)];
            triangles[drawingOrder + 2] = itemIndexes[((i + 1) % itemCount)];

            drawingOrder += 3;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mRadarMeshCanvasRenderer.SetMesh(mesh);
        mRadarMeshCanvasRenderer.SetMaterial(radarMaterial, null);
    }

    private void DrawRadarLine(LineRenderer renderer, int itemCount, float[] values, Vector3 centerPos, float startWidth, float endWidth)
    {
        renderer.material = RadarLineMaterial;
        renderer.startWidth = startWidth;
        renderer.endWidth = endWidth;
        renderer.loop = true;
        float angle = 2 * Mathf.PI / itemCount;
        renderer.positionCount = itemCount;
        renderer.useWorldSpace = false;

        for (int i = 0; i < itemCount; i++)
        {
            renderer.SetPosition(i, Quaternion.Euler(0, 0, -mAngleIncrement * i) * Vector3.up * CHART_SIZE * values[i]);
        }
    }

    private void DrawBackground(int itemCount)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[3 * itemCount];
        Vector2[] uv = new Vector2[3 * itemCount];
        int[] triangles = new int[3 * itemCount];

        int[] itemIndexes = new int[itemCount];

        // Center of radar chart
        vertices[0] = Vector3.zero;

        // Items of radar chart
        for (int i = 0; i < itemCount; i++)
        {
            vertices[i + 1] = Quaternion.Euler(0, 0, -mAngleIncrement * i) * Vector3.up * CHART_SIZE * 1;
            itemIndexes[i] = i + 1;
        }

        // line texture
        uv[0] = Vector2.zero;
        uv[1] = Vector2.one;
        uv[2] = Vector2.one;
        uv[3] = Vector2.one;
        uv[4] = Vector2.one;
        uv[5] = Vector2.one;


        // order of drawing triangles
        int drawingOrder = 0;
        for (int i = 0; i < itemCount; i++)
        {
            triangles[drawingOrder] = 0;                    // center of radar chart
            triangles[drawingOrder + 1] = itemIndexes[(i % itemCount)];
            triangles[drawingOrder + 2] = itemIndexes[((i + 1) % itemCount)];

            drawingOrder += 3;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mBackgroundMeshCanvasRenderer.SetMesh(mesh);
        mBackgroundMeshCanvasRenderer.SetMaterial(BackgroundMeshMaterial, null);
    }

    private void DrawBackgroundPolygon(LineRenderer renderer, int itemCount, float radius, Vector3 centerPos, float startWidth, float endWidth)
    {
        renderer.material = BackgroundLineMaterial;
        renderer.startWidth = startWidth;
        renderer.endWidth = endWidth;
        renderer.loop = true;
        float angle = 2 * Mathf.PI / itemCount;
        renderer.positionCount = itemCount;
        renderer.useWorldSpace = false;


        for (int i = 0; i < itemCount; i++)
        {
            Matrix4x4 rotationMatrix = new Matrix4x4(new Vector4(Mathf.Cos(angle * i), Mathf.Sin(angle * i), 0, 0),
                                                     new Vector4(-1 * Mathf.Sin(angle * i), Mathf.Cos(angle * i), 0, 0),
                                                     new Vector4(0, 0, 1, 0),
                                                     new Vector4(0, 0, 0, 1)
                                                     );
            Vector3 initialRelativePosition = new Vector3(0, radius, 0);
            renderer.SetPosition(i, centerPos + rotationMatrix.MultiplyPoint(initialRelativePosition));
        }
    }

    private void DrawBackgroundLine(LineRenderer renderer, Vector3 p1, Vector3 p2, float startWidth, float endWidth)
    {
        renderer.material = BackgroundLineMaterial;
        renderer.startWidth = startWidth;
        renderer.endWidth = endWidth;
        renderer.loop = false;
        renderer.positionCount = 2;
        renderer.useWorldSpace = false;

        renderer.SetPosition(0, p1);
        renderer.SetPosition(1, p2);
    }

    private void CreateLabel(RectTransform textContainer, string[] items, float size, float location, Color color, TMP_FontAsset font)
    {
        for (int i = 0; i < items.Length; i++)
        {
            // create text
            GameObject label = new GameObject("Label", typeof(TextMeshPro));
            label.transform.SetParent(textContainer, false);

            var labelTMPro = label.transform.GetComponent<TextMeshPro>();
            labelTMPro.text = items[i];
            labelTMPro.fontStyle = FontStyles.Italic | FontStyles.Bold;
            labelTMPro.fontSize = size;
            labelTMPro.alignment = TextAlignmentOptions.Center;
            labelTMPro.color = color;
            labelTMPro.enableWordWrapping = false;
            labelTMPro.font = font;

            // location
            var pos = Quaternion.Euler(0, 0, -mAngleIncrement * i) * Vector3.up * CHART_SIZE * location;
            label.transform.GetComponent<RectTransform>().anchoredPosition = pos;
        }

    }
}