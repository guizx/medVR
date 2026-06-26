using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Advanced Horror Post-Processing with Scene View Preview
/// Unity 6 URP - Attach to Main Camera
/// </summary>
[ExecuteAlways] // Runs in Edit Mode too!
public class AdvancedHorrorPostProcessing : MonoBehaviour
{
    [Header("=== POST-PROCESSING VOLUME ===")]
    public Volume postProcessVolume;
    
    [Header("=== VIGNETTE (Dark Edges) ===")]
    public bool enableVignette = true;
    [Range(0f, 1f)] public float vignetteIntensity = 0.65f;
    [Range(0f, 1f)] public float vignetteSmoothness = 0.4f;
    public Color vignetteColor = new Color(0.05f, 0.05f, 0.15f);
    
    [Header("=== FILM GRAIN (Old Camera) ===")]
    public bool enableFilmGrain = true;
    public FilmGrainLookup grainType = FilmGrainLookup.Medium1;
    [Range(0f, 1f)] public float grainIntensity = 0.5f;
    [Range(0f, 1f)] public float grainResponse = 0.8f;
    
    [Header("=== CHROMATIC ABERRATION (Color Split) ===")]
    public bool enableChromaticAberration = true;
    [Range(0f, 1f)] public float chromaticIntensity = 0.35f;
    
    [Header("=== COLOR GRADING ===")]
    public bool enableColorGrading = true;
    [Range(-100f, 100f)] public float saturation = -45f;
    [Range(-100f, 100f)] public float contrast = 20f;
    [Range(-100f, 100f)] public float brightness = -10f;
    public Color colorFilter = new Color(0.8f, 0.85f, 1f); // Cool blue tint
    
    [Header("=== LENS DISTORTION (Warping) ===")]
    public bool enableLensDistortion = true;
    [Range(-1f, 1f)] public float distortionIntensity = -0.2f;
    [Range(0.01f, 5f)] public float distortionScale = 1.0f;
    
    [Header("=== BLOOM (Glow) ===")]
    public bool enableBloom = true;
    [Range(0f, 10f)] public float bloomIntensity = 1.5f;
    [Range(0f, 10f)] public float bloomThreshold = 1.0f;
    public Color bloomTint = Color.white;
    
    [Header("=== MOTION BLUR (Disorientation) ===")]
    public bool enableMotionBlur = false;
    [Range(0f, 1f)] public float motionBlurIntensity = 0.3f;
    
    [Header("=== DEPTH OF FIELD (Focus Blur) ===")]
    public bool enableDepthOfField = false;
    [Range(0.1f, 50f)] public float focusDistance = 10f;
    [Range(0.1f, 32f)] public float aperture = 8f;
    [Range(1f, 300f)] public float focalLength = 50f;
    
    [Header("=== SHADOW/MIDTONE/HIGHLIGHT ===")]
    public bool enableSplitToning = true;
    public Color shadowColor = new Color(0.2f, 0.25f, 0.4f);
    public Color highlightColor = new Color(1f, 0.95f, 0.9f);
    [Range(-100f, 100f)] public float splitBalance = 0f;
    
    [Header("=== TONEMAPPING ===")]
    public bool enableTonemapping = true;
    public TonemappingMode tonemapMode = TonemappingMode.ACES;
    
    [Header("=== DYNAMIC EFFECTS ===")]
    public bool enableBreathing = true;
    [Range(0.1f, 2f)] public float breathingSpeed = 0.5f;
    [Range(0f, 0.3f)] public float breathingIntensity = 0.12f;
    
    public bool enableFlicker = false;
    [Range(0.1f, 5f)] public float flickerSpeed = 2f;
    [Range(0f, 0.5f)] public float flickerAmount = 0.15f;
    
    [Header("=== SCREEN DISTORTION ===")]
    public bool enableScreenDistortion = false;
    [Range(0f, 1f)] public float distortionAmount = 0.1f;
    [Range(0.5f, 10f)] public float distortionFrequency = 3f;
    
    // Component references
    private Vignette vignette;
    private FilmGrain filmGrain;
    private ChromaticAberration chromatic;
    private ColorAdjustments colorAdjustments;
    private LensDistortion lensDistortion;
    private Bloom bloom;
    private MotionBlur motionBlur;
    private DepthOfField depthOfField;
    private SplitToning splitToning;
    private Tonemapping tonemapping;
    
    private float breathingTime = 0f;
    private float flickerTime = 0f;
    private float distortionTime = 0f;
    
    private float baseVignetteIntensity;
    private float baseBrightness;

    void OnEnable()
    {
        SetupPostProcessing();
        baseVignetteIntensity = vignetteIntensity;
        baseBrightness = brightness;
    }

    void Update()
    {
        // Works in both Play and Edit mode!
        ApplyDynamicEffects();
        
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Force update in Scene view
            ApplyAllSettings();
        }
#endif
    }
    
    void OnValidate()
    {
        // Apply immediately when values change in Inspector
        if (postProcessVolume != null)
        {
            ApplyAllSettings();
        }
    }

    void SetupPostProcessing()
    {
        // Auto-create volume if missing
        if (postProcessVolume == null)
        {
            Volume[] volumes = FindObjectsOfType<Volume>();
            if (volumes.Length > 0)
            {
                postProcessVolume = volumes[0];
            }
            else
            {
                GameObject volumeGO = new GameObject("Horror Post-Process Volume");
                postProcessVolume = volumeGO.AddComponent<Volume>();
                postProcessVolume.isGlobal = true;
                postProcessVolume.priority = 10;
            }
        }

        // Create profile if missing
        if (postProcessVolume.profile == null)
        {
            postProcessVolume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
        }

        // Get or add all effects
        GetOrAddEffect(ref vignette);
        GetOrAddEffect(ref filmGrain);
        GetOrAddEffect(ref chromatic);
        GetOrAddEffect(ref colorAdjustments);
        GetOrAddEffect(ref lensDistortion);
        GetOrAddEffect(ref bloom);
        GetOrAddEffect(ref motionBlur);
        GetOrAddEffect(ref depthOfField);
        GetOrAddEffect(ref splitToning);
        GetOrAddEffect(ref tonemapping);

        ApplyAllSettings();
        
        Debug.Log("✓ Horror Post-Processing Setup Complete!");
    }
    
    void GetOrAddEffect<T>(ref T effect) where T : VolumeComponent
    {
        if (!postProcessVolume.profile.TryGet(out effect))
        {
            effect = postProcessVolume.profile.Add<T>(false);
        }
    }

    void ApplyAllSettings()
    {
        if (postProcessVolume == null) return;
        
        // VIGNETTE
        if (vignette != null)
        {
            vignette.active = enableVignette;
            vignette.intensity.Override(vignetteIntensity);
            vignette.smoothness.Override(vignetteSmoothness);
            vignette.color.Override(vignetteColor);
        }

        // FILM GRAIN
        if (filmGrain != null)
        {
            filmGrain.active = enableFilmGrain;
            filmGrain.type.Override(grainType);
            filmGrain.intensity.Override(grainIntensity);
            filmGrain.response.Override(grainResponse);
        }

        // CHROMATIC ABERRATION
        if (chromatic != null)
        {
            chromatic.active = enableChromaticAberration;
            chromatic.intensity.Override(chromaticIntensity);
        }

        // COLOR ADJUSTMENTS
        if (colorAdjustments != null)
        {
            colorAdjustments.active = enableColorGrading;
            colorAdjustments.saturation.Override(saturation);
            colorAdjustments.contrast.Override(contrast);
            colorAdjustments.postExposure.Override(brightness / 20f);
            colorAdjustments.colorFilter.Override(colorFilter);
        }

        // LENS DISTORTION
        if (lensDistortion != null)
        {
            lensDistortion.active = enableLensDistortion;
            lensDistortion.intensity.Override(distortionIntensity);
            lensDistortion.scale.Override(distortionScale);
        }

        // BLOOM
        if (bloom != null)
        {
            bloom.active = enableBloom;
            bloom.intensity.Override(bloomIntensity);
            bloom.threshold.Override(bloomThreshold);
            bloom.tint.Override(bloomTint);
        }

        // MOTION BLUR
        if (motionBlur != null)
        {
            motionBlur.active = enableMotionBlur;
            motionBlur.intensity.Override(motionBlurIntensity);
        }

        // DEPTH OF FIELD
        if (depthOfField != null)
        {
            depthOfField.active = enableDepthOfField;
            depthOfField.mode.Override(DepthOfFieldMode.Gaussian);
            depthOfField.gaussianStart.Override(focusDistance);
            depthOfField.gaussianEnd.Override(focusDistance + 10f);
            depthOfField.gaussianMaxRadius.Override(1f);
        }

        // SPLIT TONING
        if (splitToning != null)
        {
            splitToning.active = enableSplitToning;
            splitToning.shadows.Override(shadowColor);
            splitToning.highlights.Override(highlightColor);
            splitToning.balance.Override(splitBalance);
        }

        // TONEMAPPING
        if (tonemapping != null)
        {
            tonemapping.active = enableTonemapping;
            tonemapping.mode.Override(tonemapMode);
        }
    }

    void ApplyDynamicEffects()
    {
        float deltaTime = Application.isPlaying ? Time.deltaTime : Time.realtimeSinceStartup * 0.016f;
        
        // BREATHING EFFECT
        if (enableBreathing && vignette != null)
        {
            breathingTime += deltaTime * breathingSpeed;
            float breathe = Mathf.Sin(breathingTime) * breathingIntensity;
            vignette.intensity.value = Mathf.Clamp01(baseVignetteIntensity + breathe);
        }

        // FLICKER EFFECT
        if (enableFlicker && colorAdjustments != null)
        {
            flickerTime += deltaTime * flickerSpeed;
            float flicker = Mathf.PerlinNoise(flickerTime, 0f) * flickerAmount;
            colorAdjustments.postExposure.value = (baseBrightness / 20f) - flicker;
        }

        // SCREEN DISTORTION
        if (enableScreenDistortion && lensDistortion != null)
        {
            distortionTime += deltaTime * distortionFrequency;
            float distort = Mathf.Sin(distortionTime) * distortionAmount;
            lensDistortion.intensity.value = distortionIntensity + distort;
        }
    }

    // PUBLIC METHODS FOR RUNTIME CONTROL
    
    public void SetIntensity(float intensity)
    {
        vignetteIntensity = Mathf.Lerp(0.3f, 0.9f, intensity);
        grainIntensity = Mathf.Lerp(0.2f, 0.7f, intensity);
        chromaticIntensity = Mathf.Lerp(0.1f, 0.6f, intensity);
        saturation = Mathf.Lerp(-20f, -60f, intensity);
        ApplyAllSettings();
    }

    public void TriggerJumpScare(float duration = 0.5f)
    {
        if (Application.isPlaying)
            StartCoroutine(JumpScareRoutine(duration));
    }

    System.Collections.IEnumerator JumpScareRoutine(float duration)
    {
        float elapsed = 0f;
        float startVig = vignetteIntensity;
        float startChrom = chromaticIntensity;
        float startDist = distortionIntensity;

        // Spike
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            vignetteIntensity = Mathf.Lerp(startVig, 1f, t);
            chromaticIntensity = Mathf.Lerp(startChrom, 1f, t);
            distortionIntensity = Mathf.Lerp(startDist, -0.8f, t);
            ApplyAllSettings();
            yield return null;
        }

        // Recovery
        elapsed = 0f;
        while (elapsed < duration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration * 0.5f);
            vignetteIntensity = Mathf.Lerp(1f, startVig, t);
            chromaticIntensity = Mathf.Lerp(1f, startChrom, t);
            distortionIntensity = Mathf.Lerp(-0.8f, startDist, t);
            ApplyAllSettings();
            yield return null;
        }
    }

    public void TriggerGlitch(float duration = 0.2f)
    {
        if (Application.isPlaying)
            StartCoroutine(GlitchRoutine(duration));
    }

    System.Collections.IEnumerator GlitchRoutine(float duration)
    {
        float elapsed = 0f;
        bool originalFlicker = enableFlicker;
        enableFlicker = true;
        flickerSpeed = 20f;

        while (elapsed < duration)
        {
            chromaticIntensity = Random.Range(0.5f, 1f);
            distortionIntensity = Random.Range(-0.5f, 0.5f);
            ApplyAllSettings();
            elapsed += Time.deltaTime;
            yield return null;
        }

        enableFlicker = originalFlicker;
        flickerSpeed = 2f;
        chromaticIntensity = 0.35f;
        distortionIntensity = -0.2f;
        ApplyAllSettings();
    }
}

