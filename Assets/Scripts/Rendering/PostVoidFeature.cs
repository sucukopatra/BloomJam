using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace BloomJam.Rendering
{
    /// <summary>
    /// Fullscreen post-process that applies the PostVoid distortion shader (per-channel UV
    /// shear, noise warp, scanline tear). Uses the URP 17 RenderGraph API: copy active color
    /// to a temp, then run the distortion shader writing back to active color.
    ///
    /// Drive intensity from C# via Shader.SetGlobalFloat on _PV_Intensity / _PV_CA / _PV_Warp /
    /// _PV_Pulse / _PV_Time / _PV_Shear (see PostVoidDirector).
    /// </summary>
    public sealed class PostVoidFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class Settings
        {
            [Tooltip("Hidden/PostVoid/Distortion shader. Auto-found by name if left null.")]
            public Shader shader;

            [Tooltip("Where in the URP frame to inject the distortion. After post-processing is normally what you want.")]
            public RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingPostProcessing;

            [Tooltip("Skip on Scene view / preview / reflection cameras.")]
            public bool gameCameraOnly = true;
        }

        public Settings settings = new();

        private Material _material;
        private PostVoidPass _pass;

        public override void Create()
        {
            if (settings.shader == null)
                settings.shader = Shader.Find("Hidden/PostVoid/Distortion");

            if (settings.shader == null)
            {
                Debug.LogWarning("[PostVoidFeature] Distortion shader not found. Feature disabled.");
                return;
            }

            _material = CoreUtils.CreateEngineMaterial(settings.shader);
            _pass = new PostVoidPass(_material)
            {
                renderPassEvent = settings.injectionPoint,
                requiresIntermediateTexture = true
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
        {
            if (_pass == null || _material == null) return;
            if (settings.gameCameraOnly && data.cameraData.cameraType != CameraType.Game) return;
            renderer.EnqueuePass(_pass);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(_material);
            _material = null;
            _pass = null;
        }

        private sealed class PostVoidPass : ScriptableRenderPass
        {
            private static readonly int BlitTextureId   = Shader.PropertyToID("_BlitTexture");
            private static readonly int BlitScaleBiasId = Shader.PropertyToID("_BlitScaleBias");
            private static readonly MaterialPropertyBlock s_Mpb = new();

            private readonly Material _material;

            private class PassData
            {
                public TextureHandle source;
                public Material material;
            }

            public PostVoidPass(Material mat)
            {
                _material = mat;
                profilingSampler = new ProfilingSampler("PostVoid");
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                var resources = frameData.Get<UniversalResourceData>();
                if (!resources.cameraColor.IsValid()) return;

                var desc = renderGraph.GetTextureDesc(resources.cameraColor);
                desc.name = "PostVoid_TmpColor";
                desc.clearBuffer = false;
                desc.depthBufferBits = 0;
                var tmp = renderGraph.CreateTexture(desc);

                // Vanilla copy: active color -> tmp (tmp is the read source for the distortion pass).
                renderGraph.AddBlitPass(
                    resources.activeColorTexture, tmp,
                    Vector2.one, Vector2.zero,
                    passName: "PostVoid Copy");

                // Distortion: tmp -> active color via _material.
                using (var builder = renderGraph.AddRasterRenderPass<PassData>("PostVoid Distortion", out var data, profilingSampler))
                {
                    data.source = tmp;
                    data.material = _material;

                    builder.UseTexture(tmp, AccessFlags.Read);
                    builder.UseTexture(resources.cameraDepthTexture, AccessFlags.Read);
                    builder.SetRenderAttachment(resources.activeColorTexture, 0, AccessFlags.Write);

                    builder.SetRenderFunc((PassData d, RasterGraphContext ctx) =>
                    {
                        s_Mpb.Clear();
                        s_Mpb.SetTexture(BlitTextureId, d.source);
                        s_Mpb.SetVector(BlitScaleBiasId, new Vector4(1f, 1f, 0f, 0f));
                        ctx.cmd.DrawProcedural(
                            Matrix4x4.identity, d.material, 0,
                            MeshTopology.Triangles, 3, 1, s_Mpb);
                    });
                }
            }
        }
    }
}
