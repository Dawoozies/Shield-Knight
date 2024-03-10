Shader "Unlit/Beam"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WhiteCutoffFactor ("White Cutoff Factor (Higher this is the smaller the white part of the beam)", Range(2, 50)) = 2.0
        _Fade ("Fade Distance From Edges", Range(0,1)) = 0.0
        _Resolution ("Resolution", Range(1,255)) = 0.0
        _TextureCutoff("Texture Cutoff Value", Range(0,1)) = 0.0
        _Speed ("Beam Speed", Range(0,500)) = 0.0
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
            float _Fade;
            float _Resolution;
            float _TextureCutoff;
            float _Speed;
            float _WhiteCutoffFactor;
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
                // sample the texture
                float2 uv = i.uv;
                float shift = (_Time.x * _Speed) % 1;
                uv = float2(i.uv.x - shift, i.uv.y);
                float distFromMiddle = length(uv - float2(uv.x, 0.5))*_WhiteCutoffFactor;
                float distFromBottom = length(uv - float2(uv.x, 1))*2;
                float distFromTop = length(uv - float2(uv.x, 0))*2;
                fixed4 white = fixed4(1, 1, 1, 1);
                fixed4 beamColor = i.color;
                float alpha = lerp(0, 1, saturate(distFromTop-_Fade))*step(0.5, 1-uv.y)+lerp(0, 1, saturate(distFromBottom-_Fade))*step(0.5, uv.y);
                alpha = saturate(alpha);
                //toon shading not smooth, or at least pixelate it
                fixed4 col = lerp(white, beamColor, distFromMiddle)*fixed4(1,1,1,alpha);
                col = col*_Resolution;
                col = round(col);
                col = col/_Resolution;
                //col = col*col;
                fixed4 textureCol = tex2D(_MainTex, uv);
                float textureLightValue = step(_TextureCutoff, textureCol.r+textureCol.g+textureCol.b);
                col = fixed4(col.rgb, textureLightValue);
                return col;
            }
            ENDCG
        }
    }
}
