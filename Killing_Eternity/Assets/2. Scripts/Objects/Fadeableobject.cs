using UnityEngine;
using UnityEngine.Rendering;

public class FadeableObject : MonoBehaviour
{
    public enum Mode { Dither, Transparent }

    [SerializeField] Mode mode = Mode.Dither;
    [SerializeField, Range(0f, 1f)] float fadedAlpha = 0.25f;
    [SerializeField] float fadeSpeed = 5f;
    [SerializeField] string ditherProperty = "_Fade";
    [SerializeField] string colorProperty = "_BaseColor";

    Renderer[] renderers;
    MaterialPropertyBlock mpb;
    int ditherId, colorId;

    Material[][] originalMats;
    Material[][] fadeMats;
    bool usingFadeMats;

    float alpha = 1f;
    bool occluding;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
        ditherId = Shader.PropertyToID(ditherProperty);
        colorId = Shader.PropertyToID(colorProperty);

        if (mode == Mode.Transparent)
            BuildFadeMaterials();

        enabled = false;
    }

    public void SetOccluding(bool value)
    {
        if (occluding == value) return;
        occluding = value;
        enabled = true;
    }

    void Update()
    {
        float goal = occluding ? fadedAlpha : 1f;
        alpha = Mathf.MoveTowards(alpha, goal, fadeSpeed * Time.deltaTime);
        Apply();
        if (Mathf.Approximately(alpha, goal))
            enabled = false;
    }

    void Apply()
    {
        if (mode == Mode.Dither)
        {
            foreach (Renderer r in renderers)
            {
                r.GetPropertyBlock(mpb);
                mpb.SetFloat(ditherId, alpha);
                r.SetPropertyBlock(mpb);
            }
            return;
        }

        // Transparent mode
        bool needFade = alpha < 0.999f;
        if (needFade != usingFadeMats)
        {
            usingFadeMats = needFade;
            for (int i = 0; i < renderers.Length; i++)
                renderers[i].sharedMaterials = needFade ? fadeMats[i] : originalMats[i];
        }

        if (!needFade) return;
        foreach (Material[] mats in fadeMats)
            foreach (Material m in mats)
            {
                if (!m || !m.HasProperty(colorId)) continue;
                Color c = m.GetColor(colorId);
                c.a = alpha;
                m.SetColor(colorId, c);
            }
    }

    void BuildFadeMaterials()
    {
        originalMats = new Material[renderers.Length][];
        fadeMats = new Material[renderers.Length][];
        for (int i = 0; i < renderers.Length; i++)
        {
            Material[] src = renderers[i].sharedMaterials;
            originalMats[i] = src;
            fadeMats[i] = new Material[src.Length];
            for (int j = 0; j < src.Length; j++)
            {
                if (!src[j]) continue;
                Material m = new Material(src[j]) { name = src[j].name + " (Fade)" };
                MakeTransparentURP(m);
                fadeMats[i][j] = m;
            }
        }
    }

    // Flips a URP/Lit material to Transparent/Alpha at runtime.
    static void MakeTransparentURP(Material m)
    {
        m.SetOverrideTag("RenderType", "Transparent");
        if (m.HasProperty("_Surface")) m.SetFloat("_Surface", 1f);
        if (m.HasProperty("_Blend")) m.SetFloat("_Blend", 0f);
        m.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        if (m.HasProperty("_SrcBlendAlpha")) m.SetInt("_SrcBlendAlpha", (int)BlendMode.One);
        if (m.HasProperty("_DstBlendAlpha")) m.SetInt("_DstBlendAlpha", (int)BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = (int)RenderQueue.Transparent;
    }

    void OnDestroy()
    {
        if (fadeMats == null) return;
        foreach (Material[] mats in fadeMats)
            foreach (Material m in mats)
                if (m) Destroy(m);
    }
}