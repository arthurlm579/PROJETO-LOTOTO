using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class VHSPostProcess : MonoBehaviour
{
    [Header("=== Configurações VHS ===")]
    [Range(0f, 1f)]
    public float intensity = 0.65f;

    [Range(0f, 1f)]
    public float glitchAmount = 0.45f;

    [Range(0f, 0.05f)]
    public float chromaticAberration = 0.014f;

    [Header("Scanlines")]
    public float scanlineSpeed = 9.5f;

    [Header("REC Indicator (piscando)")]
    public bool showREC = true;
    public float recBlinkSpeed = 1.2f;

    private Material vhsMaterial;
    private Shader vhsShader;

    // Textura de ruído (Unity já tem uma interna, mas podemos criar)
    private Texture2D noiseTexture;

    void OnEnable()
    {
        CreateMaterial();
        CreateNoiseTexture();
    }

    void CreateMaterial()
    {
        if (vhsShader == null)
            vhsShader = Shader.Find("Hidden/VHS_Effect");

        if (vhsShader == null)
        {
            Debug.LogError("Shader 'Hidden/VHS_Effect' não encontrado! Verifique o nome.");
            return;
        }

        if (vhsMaterial == null)
            vhsMaterial = new Material(vhsShader);
    }

    void CreateNoiseTexture()
    {
        noiseTexture = new Texture2D(256, 256, TextureFormat.RGB24, false);
        noiseTexture.filterMode = FilterMode.Point;
        noiseTexture.wrapMode = TextureWrapMode.Repeat;

        for (int x = 0; x < 256; x++)
            for (int y = 0; y < 256; y++)
            {
                float noise = Random.value;
                noiseTexture.SetPixel(x, y, new Color(noise, noise, noise));
            }
        noiseTexture.Apply();
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (vhsMaterial == null)
        {
            Graphics.Blit(source, destination);
            return;
        }

        vhsMaterial.SetFloat("_Intensity", intensity);
        vhsMaterial.SetFloat("_GlitchAmount", glitchAmount);
        vhsMaterial.SetFloat("_ChromaticAberration", chromaticAberration);
        vhsMaterial.SetFloat("_ScanlineSpeed", scanlineSpeed);

        if (noiseTexture != null)
            vhsMaterial.SetTexture("_NoiseTex", noiseTexture);

        Graphics.Blit(source, destination, vhsMaterial);
    }

    // Desenha o "REC" piscando no canto superior esquerdo
    void OnGUI()
    {
        if (!showREC) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 28;
        style.normal.textColor = Color.red;
        style.fontStyle = FontStyle.Bold;

        float blink = Mathf.Sin(Time.time * recBlinkSpeed * 6f) > 0 ? 1f : 0.3f;

        GUI.color = new Color(1, 0, 0, blink);
        GUI.Label(new Rect(35, 25, 200, 50), "● REC");

        GUI.color = Color.white;
    }

    void OnDisable()
    {
        if (vhsMaterial != null)
            DestroyImmediate(vhsMaterial);
    }
}