// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/FrostedGlass"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Radius("Radius", Range(1, 51)) = 1
        _Scale("Scale", Range(0, 10)) = 1
    }

    Category
    {
        Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }

        SubShader
        {
            GrabPass
            {
                Tags{ "LightMode" = "Always" }
            }

            Pass
            {
                Tags{ "LightMode" = "Always" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    return o;
                }

                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _Radius;
                float _Scale;

                inline float calculateGaussWeight(float sigma,float index){
                    return (1 / sqrt(2 * 3.14159265359 * (sigma * sigma)) * pow(2.71828182846, -((index * index) / (2 * (sigma * sigma)))));
                }

                half4 frag(v2f i) : COLOR
                {
                    #define GRABXYPIXEL(kernelx, kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely, i.uvgrab.z, i.uvgrab.w)))

                    half4 sum = GRABXYPIXEL(0, 0) * calculateGaussWeight(_Radius, 0);

                    for(int index = 1; index < _Radius * 5; index++){
                        sum += GRABXYPIXEL(index * _Scale, 0) * calculateGaussWeight(_Radius, index);
                        sum += GRABXYPIXEL(-index * _Scale, 0) * calculateGaussWeight(_Radius, index);
                    }

                    return sum;
                }
                ENDCG
            }
            GrabPass
            {
                Tags{ "LightMode" = "Always" }
            }

            Pass
            {
                Tags{ "LightMode" = "Always" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                    float scale = -1.0;
                    #else
                    float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    return o;
                }

                sampler2D _GrabTexture;
                float4 _GrabTexture_TexelSize;
                float _Radius;
                float _Scale;

                inline float calculateGaussWeight(float sigma,float index){
                    return (1 / sqrt(2 * 3.14159265359 * (sigma * sigma)) * pow(2.71828182846, -((index * index) / (2 * (sigma * sigma)))));
                }

                half4 frag(v2f i) : COLOR
                {
                    #define GRABXYPIXEL(kernelx, kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely, i.uvgrab.z, i.uvgrab.w)))

                    half4 sum = GRABXYPIXEL(0, 0) * calculateGaussWeight(_Radius, 0);

                    for(int index = 1; index < _Radius * 5; index++){
                        sum += GRABXYPIXEL(0, index * _Scale) * calculateGaussWeight(_Radius, index);
                        sum += GRABXYPIXEL(0, -index * _Scale) * calculateGaussWeight(_Radius, index);
                    }

                    return sum;
                }
                ENDCG
            }
        }
    }
}