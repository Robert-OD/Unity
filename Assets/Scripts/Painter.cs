using UnityEngine;

public class Painter : MonoBehaviour
{
    private Camera cam;
    public Texture2D paintTexture;
    private Renderer rend;


    void Start()
    {
        cam = Camera.main;
        InitializeTexture();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // 0 is for left click
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit: " + hit.collider.name + " at point " + hit.point);
                Paint(hit);
            }
        }
    }

    private void Paint(RaycastHit hit)
    {
        rend = hit.collider.GetComponent<Renderer>();

        rend.material = rend.material;
        rend.material.mainTexture = paintTexture;
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= paintTexture.width;
        pixelUV.y *= paintTexture.height;

        // Paint a 10x10 pixel dot at the click location
        for (int x = -5; x < 5; x++)
        {
            for (int y = -5; y < 5; y++)
            {
                paintTexture.SetPixel((int)(pixelUV.x + x), (int)(pixelUV.y + y), Color.red);
            }
        }
        paintTexture.Apply();
    }


    void InitializeTexture()
    {
        // Adjust the size based on your needs (e.g., 1024x1024)
        paintTexture = new Texture2D(1024, 1024);
        // Fill the texture with transparent color initially if needed
        for (int x = 0; x < paintTexture.width; x++)
        {
            for (int y = 0; y < paintTexture.height; y++)
            {
                paintTexture.SetPixel(x, y, Color.clear);
            }
        }
        paintTexture.Apply();
    }
}
