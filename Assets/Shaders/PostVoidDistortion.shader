Shader "Hidden/PostVoid/Distortion"
{
    Properties
    {
        _MaxDistance ("Max Distance (eye-space units, far end of quantization)", Float) = 30
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZTest Always Cull Off ZWrite Off

        Pass
        {
            Name "PostVoidDistortion"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma target 3.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // Driven from PostVoidDirector via Shader.SetGlobal*.
            float  _PV_Intensity;
            float  _PV_CA;
            float  _PV_Warp;
            float  _PV_Pulse;
            float  _PV_Time;
            float2 _PV_Shear;
            float  _PV_PixelSize;   // Edge length of one chunky pixel, in screen pixels. 1 = off.
            float  _PV_Enabled;     // Master gate. 0 = bypass (return source unchanged), 1 = full effect.

            float _MaxDistance;

            // 4x4 Bayer matrix.
            static const float Bayer4x4[16] = {
                 0.0/16.0,  8.0/16.0,  2.0/16.0, 10.0/16.0,
                12.0/16.0,  4.0/16.0, 14.0/16.0,  6.0/16.0,
                 3.0/16.0, 11.0/16.0,  1.0/16.0,  9.0/16.0,
                15.0/16.0,  7.0/16.0, 13.0/16.0,  5.0/16.0
            };

            float Hash21(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }

            float VNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = Hash21(i);
                float b = Hash21(i + float2(1.0, 0.0));
                float c = Hash21(i + float2(0.0, 1.0));
                float d = Hash21(i + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            // Snap a UV to the coarse pixel grid (sample at the centre of each chunky pixel).
            float2 SnapToPixelGrid(float2 uv, float2 grid)
            {
                return (floor(uv * grid) + 0.5) / grid;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Master bypass: return source pixel untouched (no warp, no CA, no quantization).
                if (_PV_Enabled < 0.5)
                {
                    return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, input.texcoord);
                }

                float2 uv = input.texcoord;
                float2 c  = uv - 0.5;
                float  r  = length(c);
                float2 dir = c / max(r, 1e-4);

                // Coarse pixel grid (in pixels): screen size / chunk edge length.
                float pxSize = max(_PV_PixelSize, 1.0);
                float2 grid  = max(_ScreenParams.xy / pxSize, 1.0);

                // 1. Noise-driven UV warp.
                float2 np  = uv * 4.0 + float2(_PV_Time * 0.7, _PV_Time * 0.41);
                float2 wrp = float2(VNoise(np), VNoise(np + 17.3)) - 0.5;
                uv += wrp * _PV_Warp * (0.6 + r) * (0.4 + _PV_Intensity);

                // 2. Radial shear that spikes on pulse.
                uv += dir * _PV_Pulse * 0.02 * sin(_PV_Time * 40.0);

                // 3. Per-frame jitter.
                uv += _PV_Shear * 0.004 * _PV_Intensity;

                // 4. Per-channel CA along the radial axis.
                float caAmt = _PV_CA * (0.4 + r * r) * 0.005;

                // 5. Sparse scanline tear during pulses (coarse-pixel rows).
                float row      = floor(uv.y * grid.y);
                float tearMask = step(0.985, Hash21(float2(row, floor(_PV_Time * 30.0))));
                float tear     = (Hash21(float2(row, _PV_Time)) - 0.5) * 0.04 * _PV_Pulse * tearMask;

                // Snap each channel's UV to the coarse grid, then point-sample.
                float2 uvR = SnapToPixelGrid(uv + dir * caAmt + float2(tear, 0.0), grid);
                float2 uvG = SnapToPixelGrid(uv,                                   grid);
                float2 uvB = SnapToPixelGrid(uv - dir * caAmt,                     grid);

                half3 col;
                col.r = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uvR).r;
                col.g = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uvG).g;
                col.b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_PointClamp, uvB).b;

                // 6. Depth-driven palette quantization, depth UV snapped to the same grid.
                float2 depthUv = SnapToPixelGrid(input.texcoord, grid);
                float  rawDepth = SampleSceneDepth(depthUv);
                float  eyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
                float  depth01  = saturate(eyeDepth / max(_MaxDistance, 0.0001));

                float levels  = lerp(32.0, 4.0, depth01);
                float divisor = max(levels - 1.0, 1.0);

                // Bayer index from the coarse-pixel coordinate (so all fragments inside the same
                // chunky pixel get the same threshold and the chunk stays a single solid colour).
                int2 cellIdx = (int2)(input.positionCS.xy / pxSize);
                int  bx = cellIdx.x & 3;
                int  by = cellIdx.y & 3;
                float dither = Bayer4x4[by * 4 + bx] - (7.5 / 16.0);

                col = (half3)(floor(col * levels + dither) / divisor);

                return half4(saturate(col), 1.0);
            }
            ENDHLSL
        }
    }
    Fallback Off
}
