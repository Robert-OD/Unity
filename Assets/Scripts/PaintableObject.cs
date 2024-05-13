using UnityEngine;

public class PaintableObject : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject player;
    private Material paintMaterial; // To store the material created at runtime
    private bool materialCreated = false; // Flag to check if material has been created

    void Update()
    {
        if (Input.GetMouseButton(0)) // 0 is for left click
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject)
            {
                Debug.DrawRay(player.transform.position, player.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
                Debug.Log("Hit: " + hit.collider.name + " at point " + hit.point);
                CreateOrModifyMaterial(hit);
            }
        }

        UpdateTextureAges(); // Update the age of the paint every frame
    }

    private void CreateOrModifyMaterial(RaycastHit hit)
    {
        if (!materialCreated)
        {
            paintMaterial = new Material(Shader.Find("Custom/SimplePaintShader"));
            paintMaterial.SetTexture("_MainTex", GetComponent<Renderer>().material.mainTexture);
            paintMaterial.SetTexture("_PaintTex", GeneratePaintTexture());
            paintMaterial.SetTexture("_AgeTex", GenerateAgeTexture());
            GetComponent<Renderer>().material = paintMaterial;
            materialCreated = true;
        }

        PaintOnTexture(hit.textureCoord, hit.transform);
    }

    private Texture2D GeneratePaintTexture()
    {
        Texture2D paintTexture = new Texture2D(1024, 1024);
        Color[] clearColors = new Color[paintTexture.width * paintTexture.height];
        for (int i = 0; i < clearColors.Length; i++)
        {
            clearColors[i] = Color.clear; // Set all pixels to transparent
        }
        paintTexture.SetPixels(clearColors);
        paintTexture.Apply();
        return paintTexture;
    }

    private Texture2D GenerateAgeTexture()
    {
        Texture2D ageTexture = new Texture2D(1024, 1024, TextureFormat.RFloat, false);
        float[] ageValues = new float[ageTexture.width * ageTexture.height];
        for (int i = 0; i < ageValues.Length; i++)
        {
            ageValues[i] = 0; // Initialize ages to 0
        }
        ageTexture.SetPixelData(ageValues, 0);
        ageTexture.Apply();
        return ageTexture;
    }

    private void PaintOnTexture(Vector2 uv, Transform hitTransform)
    {
        Texture2D paintTexture = paintMaterial.GetTexture("_PaintTex") as Texture2D;
        Texture2D ageTexture = paintMaterial.GetTexture("_AgeTex") as Texture2D;
        int baseBrushSize = 10;  // Base size of the brush
        Vector3 lossyScale = hitTransform.lossyScale;
        float averageScale = (lossyScale.x + lossyScale.y + lossyScale.z) / 3f;
        int adjustedBrushSize = Mathf.Max(1, Mathf.RoundToInt(baseBrushSize / averageScale));

        for (int x = -adjustedBrushSize; x <= adjustedBrushSize; x++)
        {
            for (int y = -adjustedBrushSize; y <= adjustedBrushSize; y++)
            {
                int px = (int)(uv.x * paintTexture.width) + x;
                int py = (int)(uv.y * paintTexture.height) + y;
                if (px >= 0 && px < paintTexture.width && py >= 0 && py < paintTexture.height)
                {
                    paintTexture.SetPixel(px, py, Color.red);
                    ageTexture.SetPixel(px, py, new Color(0, 0, 0, 1)); // Reset age
                }
            }
        }
        paintTexture.Apply();
        ageTexture.Apply();
    }

    void UpdateTextureAges()
    {
        Texture2D ageTexture = paintMaterial.GetTexture("_AgeTex") as Texture2D;
        Color[] ageData = ageTexture.GetPixels();

        for (int i = 0; i < ageData.Length; i++)
        {
            Color agePixel = ageData[i];
            agePixel.r += Time.deltaTime * 0.25f; // Increment age
            ageData[i] = agePixel;
        }

        ageTexture.SetPixels(ageData);
        ageTexture.Apply();
    }
}
