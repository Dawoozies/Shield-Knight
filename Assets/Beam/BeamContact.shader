Shader "Unlit/BeamContact"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TextureCutoff("Texture Cutoff Value", Range(0,1)) = 0.0
        _CutoffLineGradient("Gradient of cutoff line", Range(-8,8)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

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
            float _TextureCutoff;
            float _CutoffLineGradient;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float sinTime = sin(_Time.x*1000);
                float cosTime = cos(_Time.x*1000 + 3.14159265/2);
                float2 sampleUV = step(0, sinTime)*i.uv + step(0, cosTime)*(1-i.uv);
                fixed4 textureCol = tex2D(_MainTex, sampleUV);
                float textureLightValue = step(_TextureCutoff, textureCol.r+textureCol.g+textureCol.b);
                fixed4 col = fixed4(i.color.rgb, textureLightValue);
                float xCutoff = step(0.5, 1-i.uv.x);
                float yValueTop = (i.uv.x-0.5)*_CutoffLineGradient+0.5;
                float yValueBottom = -(i.uv.x-0.5)*_CutoffLineGradient+0.5;
                float lineCutoff = step(i.uv.y, yValueTop)*step(yValueBottom, i.uv.y);
                return col * lineCutoff;
            }
            ENDCG
        }
    }
}
