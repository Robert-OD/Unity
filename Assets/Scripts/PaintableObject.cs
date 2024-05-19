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
        int baseBrushSize = 1;  // Base size for each spot of the burst
        int numberOfShots = 5;  // Number of paint spots in the burst
        float spreadRadius = 50f;  // Radius around the UV click point where shots can land

        // Loop over the number of shots to simulate a shotgun burst
        for (int i = 0; i < numberOfShots; i++)
        {
            // Generate random angle and radius for the spread
            float angle = Random.Range(0, 2 * Mathf.PI);
            float radius = Random.Range(0, spreadRadius);

            // Calculate the offset from the center point based on the random angle and radius
            int offsetX = Mathf.RoundToInt(radius * Mathf.Cos(angle));
            int offsetY = Mathf.RoundToInt(radius * Mathf.Sin(angle));

            // Determine the area to paint for each individual shot
            for (int x = -baseBrushSize; x <= baseBrushSize; x++)
            {
                for (int y = -baseBrushSize; y <= baseBrushSize; y++)
                {
                    int px = (int)(uv.x * paintTexture.width) + offsetX + x;
                    int py = (int)(uv.y * paintTexture.height) + offsetY + y;
                    if (px >= 0 && px < paintTexture.width && py >= 0 && py < paintTexture.height)
                    {
                        paintTexture.SetPixel(px, py, Color.red);
                        // Initialize age to a very small value but non-zero
                        ageTexture.SetPixel(px, py, new Color(0.01f, 0, 0, 1)); // Start age slightly above zero
                    }
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
            // Increment age for all pixels, but only if they haven't fully aged yet
            if (ageData[i].r < 1.0f)
            {
                float incrementedAge = ageData[i].r + Time.deltaTime * 0.75f; // Adjust aging rate here
                ageData[i].r = Mathf.Clamp(incrementedAge, 0.0f, 1.0f); // Ensure age does not exceed 1
            }
        }

        ageTexture.SetPixels(ageData);
        ageTexture.Apply();
    }



}
