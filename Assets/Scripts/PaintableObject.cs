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
    }

    private void CreateOrModifyMaterial(RaycastHit hit)
    {
        if (!materialCreated)
        {
            paintMaterial = new Material(Shader.Find("Custom/SimplePaintShader"));
            paintMaterial.SetTexture("_MainTex", GetComponent<Renderer>().material.mainTexture);
            paintMaterial.SetTexture("_PaintTex", GeneratePaintTexture());
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

    private void PaintOnTexture(Vector2 uv, Transform hitTransform)
    {
        Texture2D paintTexture = paintMaterial.GetTexture("_PaintTex") as Texture2D;
        int baseBrushSize = 10;  // Base size of the brush

        // Calculate average scale to adjust the brush size more uniformly
        Vector3 lossyScale = hitTransform.lossyScale;
        float averageScale = (lossyScale.x + lossyScale.y + lossyScale.z) / 3f;

        // Adjust the brush size based on the average scale of the object
        int adjustedBrushSize = Mathf.Max(1, Mathf.RoundToInt(baseBrushSize / averageScale));  // Ensure at least 1 pixel

        // Determine the area to paint
        for (int x = -adjustedBrushSize; x <= adjustedBrushSize; x++)
        {
            for (int y = -adjustedBrushSize; y <= adjustedBrushSize; y++)
            {
                int px = (int)(uv.x * paintTexture.width) + x;
                int py = (int)(uv.y * paintTexture.height) + y;
                if (px >= 0 && px < paintTexture.width && py >= 0 && py < paintTexture.height)
                {
                    paintTexture.SetPixel(px, py, Color.red);
                }
            }
        }
        paintTexture.Apply();
    }


}
