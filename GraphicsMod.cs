using System;
using MelonLoader;
using UnityEngine;
using BoneLib;
using BoneLib.BoneMenu;

[assembly: MelonInfo(typeof(GraphicsMod.Main), "Graphics", "1.0.0", "Nono")]
[assembly: MelonGame("Stress Level Zero", "BONELAB")]

namespace GraphicsMod
{
    public class Main : MelonMod
    {
        public static MelonPreferences_Category PrefCategory;
        public static MelonPreferences_Entry<float> PrefRenderScale;
        public static MelonPreferences_Entry<float> PrefBloom;
        public static MelonPreferences_Entry<float> PrefSaturation;
        public static MelonPreferences_Entry<float> PrefBrightness;
        public static MelonPreferences_Entry<float> PrefContrast;
        public static MelonPreferences_Entry<bool> PrefRTX;
        public static MelonPreferences_Entry<bool> PrefEnabled;

        private static float _renderScale = 1.0f;
        private static float _bloom = 0.0f;
        private static float _saturation = 1.0f;
        private static float _brightness = 1.0f;
        private static float _contrast = 1.0f;
        private static bool _rtx = false;
        private static bool _enabled = true;

        private static Page _mainPage;

        public override void OnInitializeMelon()
        {
            PrefCategory = MelonPreferences.CreateCategory("Graphics");
            PrefEnabled = PrefCategory.CreateEntry("Enabled", true);
            PrefRenderScale = PrefCategory.CreateEntry("RenderScale", 1.0f);
            PrefBloom = PrefCategory.CreateEntry("Bloom", 0.0f);
            PrefSaturation = PrefCategory.CreateEntry("Saturation", 1.0f);
            PrefBrightness = PrefCategory.CreateEntry("Brightness", 1.0f);
            PrefContrast = PrefCategory.CreateEntry("Contrast", 1.0f);
            PrefRTX = PrefCategory.CreateEntry("RTX", false);

            _enabled = PrefEnabled.Value;
            _renderScale = PrefRenderScale.Value;
            _bloom = PrefBloom.Value;
            _saturation = PrefSaturation.Value;
            _brightness = PrefBrightness.Value;
            _contrast = PrefContrast.Value;
            _rtx = PrefRTX.Value;

            CreateBoneMenu();
            MelonLogger.Msg("Graphics mod loaded. Menu ready. Survives Fusion.");
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            if (_enabled) ApplyAll();
        }

        public override void OnUpdate()
        {
            if (_enabled && Time.frameCount % 120 == 0)
                ApplyAll();
        }

        private void CreateBoneMenu()
        {
            _mainPage = Page.Root.CreatePage("Graphics", Color.cyan);

            _mainPage.CreateBool("Enabled", Color.green, _enabled, (v) =>
            {
                _enabled = v;
                PrefEnabled.Value = v;
                PrefCategory.SaveToFile();
                if (v) ApplyAll();
                else ResetToDefault();
            });

            _mainPage.CreateFloat("Render Scale", Color.white, _renderScale, 0.05f, 0.5f, 1.5f, (v) =>
            {
                _renderScale = v;
                PrefRenderScale.Value = v;
                PrefCategory.SaveToFile();
                ApplyRenderScale();
            });

            _mainPage.CreateFloat("Bloom", Color.magenta, _bloom, 0.1f, 0f, 5f, (v) =>
            {
                _bloom = v;
                PrefBloom.Value = v;
                PrefCategory.SaveToFile();
                ApplyPostProcess();
            });

            _mainPage.CreateFloat("Saturation", Color.yellow, _saturation, 0.05f, 0.2f, 2.5f, (v) =>
            {
                _saturation = v;
                PrefSaturation.Value = v;
                PrefCategory.SaveToFile();
                ApplyPostProcess();
            });

            _mainPage.CreateFloat("Brightness", Color.white, _brightness, 0.05f, 0.3f, 2.0f, (v) =>
            {
                _brightness = v;
                PrefBrightness.Value = v;
                PrefCategory.SaveToFile();
                ApplyPostProcess();
            });

            _mainPage.CreateFloat("Contrast", Color.gray, _contrast, 0.05f, 0.5f, 2.0f, (v) =>
            {
                _contrast = v;
                PrefContrast.Value = v;
                PrefCategory.SaveToFile();
                ApplyPostProcess();
            });

            _mainPage.CreateBool("RTX", Color.green, _rtx, (v) =>
            {
                _rtx = v;
                PrefRTX.Value = v;
                PrefCategory.SaveToFile();
                if (v)
                {
                    _bloom = 1.8f;
                    _saturation = 1.45f;
                    _brightness = 1.25f;
                    _contrast = 1.15f;
                    PrefBloom.Value = _bloom;
                    PrefSaturation.Value = _saturation;
                    PrefBrightness.Value = _brightness;
                    PrefContrast.Value = _contrast;
                    PrefCategory.SaveToFile();
                    ApplyAll();
                    MelonLogger.Msg("RTX preset ON");
                }
            });

            _mainPage.CreateFunction("Reset to Default", Color.red, () =>
            {
                ResetToDefault();
                MelonLogger.Msg("Graphics reset");
            });
        }

        private void ApplyAll()
        {
            ApplyRenderScale();
            ApplyPostProcess();
        }

        private void ApplyRenderScale()
        {
            try
            {
                float scale = Mathf.Clamp(_renderScale, 0.5f, 1.5f);
                if (UnityEngine.XR.XRSettings.enabled)
                    UnityEngine.XR.XRSettings.eyeTextureResolutionScale = scale;
            }
            catch (Exception e)
            {
                MelonLogger.Warning("RenderScale: " + e.Message);
            }
        }

        private void ApplyPostProcess()
        {
            try
            {
                Shader.SetGlobalFloat("_Saturation", _saturation);
                Shader.SetGlobalFloat("_Brightness", _brightness);
                Shader.SetGlobalFloat("_Contrast", _contrast);
                Shader.SetGlobalFloat("_BloomIntensity", _bloom);
            }
            catch (Exception e)
            {
                MelonLogger.Warning("PostProcess: " + e.Message);
            }
        }

        private void ResetToDefault()
        {
            _renderScale = 1.0f;
            _bloom = 0.0f;
            _saturation = 1.0f;
            _brightness = 1.0f;
            _contrast = 1.0f;
            _rtx = false;

            PrefRenderScale.Value = _renderScale;
            PrefBloom.Value = _bloom;
            PrefSaturation.Value = _saturation;
            PrefBrightness.Value = _brightness;
            PrefContrast.Value = _contrast;
            PrefRTX.Value = false;
            PrefCategory.SaveToFile();
            ApplyAll();
        }
    }
}
