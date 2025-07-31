Shader "Unlit/FadeOutAndScrollX"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle] _Flip ("Flip", Float) = 0.0
        _AlphaScale ("AlphaScale", Float) = 0.5
        _TimeScale ("TimeScale", Vector) = (2.0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Fade" }
        LOD 100
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Flip;
            float _AlphaScale;
            float2 _TimeScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, (i.uv + _Time * _TimeScale.xy) % 1.0);
                col.a *= _AlphaScale * (_Flip - i.uv.x) * (-1 + _Flip * 2);
                return col;
            }
            ENDCG
        }
    }
}
