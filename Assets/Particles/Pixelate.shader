Shader "Unlit/Pixelate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PixelCountU ("Pixel Count U", float) = 100
        _PixelCountV ("Pixel Count V", float) = 100
        _AlphaCutoff ("Alpha Cutoff", Range(0.0,10.0)) = 0.1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
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
                float4 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 custom : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 custom : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelCountU;
            float _PixelCountV;
            float _AlphaCutoff;
            float _UseTexture;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv.xy, _MainTex);
                o.color = v.color;
                o.custom = v.custom;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float pixelW = 1.0 / _PixelCountU;
                float pixelH = 1.0 / _PixelCountV;

                float2 uv = float2((int)(i.uv.x / pixelW) * pixelW, (int)(i.uv.y / pixelH) * pixelH);
                fixed4 col = tex2D(_MainTex, uv);
                if (i.custom.x <= 0.5)
                {
                    col = float4(1, 1, 1, 1);
                }

                col.a = 1-step(col.r+col.g+col.b, _AlphaCutoff);

                return col * i.color;
            }
            ENDCG
        }
    }
}
