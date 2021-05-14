using UnityEngine;
using TMPro;

public class FunnelChart : MonoBehaviour
{
    #region Get data from server
    private VirtualServer mVirtualServer;

    [Header("Server")]
    public GetXMLs XML;
    #endregion

    #region Inspector_Window_Variables

    [Header("Chart Settings")]
    [SerializeField] private Sensor[] Sensors;
    [SerializeField] private Material[] BarMaterials;
    [SerializeField] private Material BarOutlineMaterial;

    [Header("Label")]
    [SerializeField] private float FontSize = 250f;
    [SerializeField] private float Distance = 130f;
    [SerializeField] private float Margin = 15f;
    [SerializeField] private TMP_FontAsset FontAsset;
    [SerializeField] private Color FontColor = new Color(255, 255, 255, 255);
    [SerializeField] private float LabellineWidth = 0.01f;

    #endregion  // Inspector_Window_Variables

    #region PRIVATE_VARIABLES

    private RectTransform mChartContainer;
    private RectTransform mTextContainer;

    private CanvasRenderer[] mBarMeshs;
    //private LineRenderer[] mBarOutlines;
    //private LineRenderer[] mBarToLabelLines;
    private TextMeshPro[] mTexts;

    private float CHART_HEIGHT = 200f;
    [SerializeField] private float CHART_WIDTH = 250f;
    private float mBarHeight;
    private float mVerticalMovement;
    private float[] mY_Coordinates;

    #endregion  // PRIVATE_VARIABLES

    #region UNITY_MONOBEHAVIOUR_METHODS


    private void Start()
    {
        #region Server

        // Recieve data from server
        mVirtualServer = transform.GetComponent<VirtualServer>();
        #endregion

        mBarHeight = CHART_HEIGHT / Sensors.Length;
        mVerticalMovement = -0.5f * CHART_HEIGHT;
        
        // Y-coordinates of each bars
        mY_Coordinates = new float[Sensors.Length];
        for (int i = 0; i < Sensors.Length; i++)
        {
            mY_Coordinates[i] = mBarHeight * (i + 1) + mVerticalMovement;
        }

        // Initalize
        mChartContainer  = transform.Find("ChartContainer").GetComponent<RectTransform>();
        mTextContainer   = transform.Find("TextContainer").GetComponent<RectTransform>();
        mBarMeshs        = new CanvasRenderer[Sensors.Length];
        //mBarOutlines     = new LineRenderer[Items.Length];
        //mBarToLabelLines = new LineRenderer[Items.Length];
        mTexts           = new TextMeshPro[Sensors.Length];

        for (int i = 0; i < Sensors.Length; i++)
        {
            // Bar meshs
            GameObject barMesh = new GameObject("barMesh", typeof(CanvasRenderer));
            barMesh.transform.SetParent(mChartContainer, false);
            mBarMeshs[i] = barMesh.transform.GetComponent<CanvasRenderer>();

            // Labels
            GameObject label = new GameObject("label", typeof(TextMeshPro));
            label.transform.SetParent(mTextContainer, false);
            mTexts[i] = label.transform.GetComponent<TextMeshPro>();

            float y = mY_Coordinates[i] - 0.5f * mBarHeight;
            CreateLabel(mTexts[i], Sensors[i].name, FontSize, Distance, y, FontAsset, BarMaterials[i]);

            #region optimization

            //// Label lines
            //GameObject labelLine = new GameObject("labelLine", typeof(LineRenderer));
            //labelLine.transform.SetParent(mChartContainer, false);
            //mBarToLabelLines[i] = labelLine.transform.GetComponent<LineRenderer>();

            //var p1 = new Vector3(0, y, 0);
            //var p2 = new Vector3(Distance - Margin, y, 0);
            //CreateLineToLabel(mBarToLabelLines[i], p1, p2, LabellineWidth, LabellineWidth, BarMaterials[i]);

            //// Bar outlines
            //GameObject barOutline = new GameObject("barOutline", typeof(LineRenderer));
            //barOutline.transform.SetParent(mChartContainer, false);
            //mBarOutlines[i] = barOutline.transform.GetComponent<LineRenderer>();
            
            #endregion
        }
    }

    private void Update()
    {
        //float[] IoTData = Normalize(XML.Data, Sensors);
        float[] IoTData = Normalize(mVirtualServer.IoTData, Sensors);

        for (int i = 0; i < Sensors.Length; i++)
        {
            // create first bar
            if (i == 0)
            {
                var p0_startPos = new Vector3(0, mVerticalMovement, 0);
                var p1_leftTop  = new Vector3(IoTData[i] * CHART_WIDTH * -0.5f, mY_Coordinates[i], 0);
                var p2_rightTop = new Vector3(IoTData[i] * CHART_WIDTH * 0.5f, mY_Coordinates[i], 0);

                CreateBar(p0_startPos, p1_leftTop, p2_rightTop, mBarMeshs[i], BarMaterials[i]);
            }

            // create other bar
            if (i != 0)
            {
                float margin = 0.15f;   // -> thickness of outline
                var v1_leftBottom = new Vector3(IoTData[i - 1]* CHART_WIDTH * -0.5f + margin, mY_Coordinates[i - 1] + margin, 0);
                var v2_rightBottom = new Vector3(IoTData[i - 1] * CHART_WIDTH * 0.5f - margin, mY_Coordinates[i - 1] + margin, 0);
                var v3_leftTop = new Vector3(IoTData[i]* CHART_WIDTH * -0.5f, mY_Coordinates[i], 0);
                var v4_RightTop = new Vector3(IoTData[i]* CHART_WIDTH * 0.5f, mY_Coordinates[i], 0);

                CreateBar(v1_leftBottom, v2_rightBottom, v3_leftTop, v4_RightTop, mBarMeshs[i], BarMaterials[i]);
                //CreateBarOutline(mBarOutlines[i], v1_leftBottom, v2_rightBottom, margin, margin);
            }
        }
    }

    #endregion  // UNITY_MONOBEHAVIOUR_METHODS

    #region PRIVATE_METHODS


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

    // first funnel bar (bottom)
    private void CreateBar(Vector3 v1, Vector3 v2, Vector3 v3, CanvasRenderer barMeshCanvasRenderer, Material material)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[3];
        Vector2[] uv = new Vector2[3];
        int[] triangles = new int[3];

        vertices[0] = v1;
        vertices[1] = v2;
        vertices[2] = v3;

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        barMeshCanvasRenderer.SetMesh(mesh);
        barMeshCanvasRenderer.SetMaterial(material, null);
    }

    // other funnel bar
    private void CreateBar(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, CanvasRenderer barMeshCanvasRenderer, Material material)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[3 * 2];
        Vector2[] uv = new Vector2[3 * 2];
        int[] triangles = new int[3 * 2];

        vertices[0] = v1;
        vertices[1] = v2;
        vertices[2] = v3;
        vertices[3] = v4;

        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 3;

        triangles[3] = 0;
        triangles[4] = 3;
        triangles[5] = 1;

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        barMeshCanvasRenderer.SetMesh(mesh);
        barMeshCanvasRenderer.SetMaterial(material, null);
    }

    private void CreateBarOutline(LineRenderer renderer, Vector3 p1, Vector3 p2, float startWidth = 0.2f, float endWidth = 0.2f)
    {
        renderer.material = BarOutlineMaterial;
        renderer.startWidth = startWidth;
        renderer.endWidth = endWidth;
        renderer.loop = false;
        renderer.positionCount = 2;
        renderer.useWorldSpace = false;

        renderer.SetPosition(0, p1);
        renderer.SetPosition(1, p2);
    }

    private void CreateLabel(TextMeshPro labelTMPro, string text, float size, float distance, float y, TMP_FontAsset font, Material mat = null)
    {
        labelTMPro.text = text;
        labelTMPro.fontStyle = FontStyles.Italic;
        labelTMPro.fontSize = size;
        labelTMPro.alignment = TextAlignmentOptions.Left;
        labelTMPro.enableWordWrapping = false;
        labelTMPro.font = font;
        if(mat == null) labelTMPro.color = Color.white;
        else labelTMPro.color = mat.color;

        // location
        labelTMPro.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(distance, y, 0);
    }

    private void CreateLineToLabel(LineRenderer renderer, Vector3 p1, Vector3 p2, float startWidth = 0.1f, float endWidth = 0.1f, Material mat = null)
    {
        renderer.startWidth = startWidth;
        renderer.endWidth = endWidth;
        renderer.loop = false;
        renderer.positionCount = 2;
        renderer.useWorldSpace = false;
        if (mat == null) renderer.material.color = Color.white;
        else renderer.material = mat;

        renderer.SetPosition(0, p1);
        renderer.SetPosition(1, p2);
    }

    #endregion  // PRIVATE_METHODS

}
