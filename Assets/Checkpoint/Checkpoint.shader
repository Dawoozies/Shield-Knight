Shader "Unlit/Checkpoint"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,0.5)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        ZWrite Off
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
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _PixelX;
            float _PixelY;
            float4 _Color;
            v2f vert (appdata v)
            {
                v2f o;
                float4x4 modelMatrix = unity_ObjectToWorld;
                float4x4 modelMatrixInverse = unity_WorldToObject;

                o.normal = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
                o.viewDir = normalize(_WorldSpaceCameraPos - mul(modelMatrix, v.vertex).xyz);

                o.pos = UnityObjectToClipPos(v.vertex);

                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                float3 normalDir = normalize(i.normal);
                float3 viewDir = normalize(i.viewDir);

                float newOpacity = min(1.0, _Color.a / abs(dot(viewDir, normalDir)));
                return float4(_Color.rgb, newOpacity);
            }
            ENDCG
        }
    }
}
