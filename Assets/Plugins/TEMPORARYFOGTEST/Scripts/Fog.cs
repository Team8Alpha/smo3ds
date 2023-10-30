using UnityEngine;

public class Fog : MonoBehaviour 
{
    [SerializeField, Tooltip("Resolution of the displacement map")]
    int resolution = 10;

    [SerializeField, Tooltip("Size of the fog space, width and height")]
    float size = 50;

    [SerializeField, Tooltip("Maximum depth the fog can depress")]
    float depth = 10;

    [SerializeField, Tooltip("Multiplies the radius objects affect the fog")]
    float affectorMultiplier = 2;

    [SerializeField]
    float lerpSpeed = 1;

    [SerializeField]
    Renderer fogRenderer;

    [SerializeField, Tooltip("Sets this Renderer's main texture to be the generated heightmap")]
    Renderer debugRenderer;

    private Color[] heightMap;
    private Texture2D texture;

    private float halfSize;

    private void Awake()
    {
        halfSize = size * 0.5f;

        heightMap = new Color[resolution * resolution];

        ClearHeightmap();

        texture = new Texture2D(resolution, resolution, TextureFormat.RHalf, false);
        texture.wrapMode = TextureWrapMode.Clamp;

        fogRenderer.material.SetTexture("_DisplacementTex", texture);
        fogRenderer.material.SetFloat("_Size", size);
        fogRenderer.material.SetFloat("_MaxDisplacement", depth);

        if (debugRenderer != null)
            debugRenderer.material.mainTexture = texture;
    }

    public void Impact(Vector3 position, float radius)
    {
        float affectorSize = radius * affectorMultiplier;

        Vector3 local = transform.InverseTransformPoint(position);

        float penetrationDepth = Mathf.Abs(local.y - radius);

        Vector2 resPoint = LocalToResolutionSpace(local);
        float resSize = LocalToResolutionScalar(affectorSize);
        float scaledDepth = penetrationDepth / depth;

        AffectInRadius(resPoint, resSize, scaledDepth);
    }

    private void Update()
    {
        LerpHeightmap();
    }

    private void LateUpdate()
    {
        texture.SetPixels(heightMap);
        texture.Apply();
    }

    private void ClearHeightmap()
    {
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                SetHeightmapValue(x, y, 0);
            }
        }
    }

    /// <summary>
    /// Returns the heightmap to the default state over time
    /// </summary>
    private void LerpHeightmap()
    {
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                float current = GetHeightmapValue(x, y);
                float target = Mathf.Lerp(current, 0, lerpSpeed * Time.deltaTime);
                SetHeightmapValue(x, y, target);
            }
        }
    }

    /// <summary>
    /// Gets the value of the heightmap, from 0 to 1.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private float GetHeightmapValue(int x, int y)
    {
        return heightMap[ToFlatIndex(x, y)].r;
    }

    /// <summary>
    /// Sets the value of the heightmap, from 0 to 1.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="value"></param>
    private void SetHeightmapValue(int x, int y, float value)
    {
        heightMap[ToFlatIndex(x, y)] = new Color(value, 0, 0);
    }

    /// <summary>
    /// Sets the heightmap value to the max of either this value or
    /// its current value.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="value"></param>
    private void SetHeightmapValueMax(int x, int y, float value)
    {
        heightMap[ToFlatIndex(x, y)] = new Color(Mathf.Max(GetHeightmapValue(x, y), value), 0, 0);
    }

    /// <summary>
    /// Pressures the heightmap at the specific position, radius and depth.
    /// </summary>
    /// <param name="resPosition"></param>
    /// <param name="resRadius"></param>
    /// <param name="scaledDepth"></param>
    private void AffectInRadius(Vector2 resPosition, float resRadius, float scaledDepth)
    {
        float adjustedRadius = resRadius + 0.5f;

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                // Center the pixel coordinate
                Vector2 p = new Vector2(x, y) + Vector2.one * 0.5f;

                float distance = Vector2.Distance(resPosition, p);

                if (distance <= adjustedRadius)
                {
                    float t = 1 - (distance / adjustedRadius);

                    SetHeightmapValueMax(x, y, t * scaledDepth);
                }
            }
        }
    }

    /// <summary>
    /// Converts from 2D coordinates to 1D.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private int ToFlatIndex(int x, int y)
    {
        return y * resolution + x;
    }

    /// <summary>
    /// Converts from 1D coordinates to 2D.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void To2DIndex(int i, out int x, out int y)
    {
        x = i % resolution;
        y = i % resolution;
    }

    private Vector2 LocalToResolutionSpace(Vector3 local)
    {
        Vector2 uv = new Vector2(local.x / size, local.z / size) + Vector2.one * 0.5f;
        return uv * resolution;
    }

    private float LocalToResolutionScalar(float localScalar)
    {
        return (localScalar / size) * resolution;
    }
}
