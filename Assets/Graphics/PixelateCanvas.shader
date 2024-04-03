Shader "Unlit/PixelateCanvas"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelSize("Pixel Size", float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        LOD 100
        ZTest Always
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
                float4 vertex : SV_POSITION;
                float2 screenUV : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenUV = ComputeScreenPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pixelSize = _PixelSize*(1/_ScreenParams.xy);
                float2 pixelUV = round(i.uv/pixelSize)*pixelSize;
                fixed4 col = tex2D(_MainTex, pixelUV);
                return col;
            }
            ENDCG
        }
    }
}
