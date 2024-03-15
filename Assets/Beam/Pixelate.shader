Shader "Unlit/Pixelate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelX ("PixelX", Float) = 0.0
        _PixelY ("PixelY", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Cutout" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelX;
            float _PixelY;
            int _Resolution;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float dx = 1.0 / _PixelX;
                float dy = 1.0 / _PixelY;
                float2 uv = float2(dx * floor(i.uv.x / dx), dy * floor(i.uv.y / dy));
                fixed4 textureCol = tex2D(_MainTex, uv);
                return textureCol;
            }
            ENDCG
        }
    }
}
