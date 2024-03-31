Shader "Unlit/PaletteSwap"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DisplacementTex("Displacement Texture", 2D) = "white" {}
        _displacement("Displacement Value", Range(0.0,2.0)) = 0.0
        _speed ("Speed", float) = 0.0
        [MaterialToggle] _RecolorActive("Recolor Active", Float) = 0
        _alphaThreshold("Alpha Threshold", Range(0.0,1.0)) = 0.2
        _threshold ("col dist threshold", Range(0.0, 1.0)) = 0.1
        _target1 ("target", Color) = (0.0,0.0,0.0,1.0)
        _target2 ("target", Color) = (0.0,0.0,0.0,1.0)
        _replace1 ("replace", Color) = (0.0,0.0,0.0,0.0)
        _replace2 ("replace", Color) = (0.0,0.0,0.0,0.0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
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
                float2 uvDisplacement : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _threshold;
            float4 _target1;
            float4 _target2;
            float4 _replace1;
            float4 _replace2;
            float _RecolorActive;
            float _alphaThreshold;

            sampler2D _DisplacementTex;
            float4 _DisplacementTex_ST;
            float _displacement;
            float _speed;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvDisplacement = TRANSFORM_TEX(v.uv, _DisplacementTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 dispCol = tex2D(_DisplacementTex, i.uvDisplacement + frac(_Time.y*float2(_speed, 0)));
                float2 uv = i.uv;
                float cutoff = step(length(dispCol), _displacement);
                fixed4 col = tex2D(_MainTex, uv)*cutoff;
                //if distance between texcol and target col less than threshold
                if(col.a < _alphaThreshold)
                {
                    discard;
                }
                
                if(_RecolorActive >= 0.5)
                {
                col = all(distance(col,_target1) <= _threshold) ? _replace1 : col;
                col = all(distance(col,_target2) <= _threshold) ? _replace2 : col;
                }
                //i mean any of these multipliers are either 0 or 1
                return col * i.color;
            }
            ENDCG
        }
    }
}
