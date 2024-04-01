Shader "Unlit/ScreenTransition"
{
    Properties
    {
        _MainTex ("Base (RGB), Alpha (A)", 2D) = "white" {}
        _TransitionTex("Transition Tex", 2D) = "white" {}
        _SpeedX ("_Speed X", Range(-1, 1)) = 0.1
        _SpeedY ("_Speed Y", Range(-1, 1)) = 0.1
        _Color ("Tint", Color) = (1,1,1,1)
        _transition("Transition", Range(0.99,2)) = 0.99
    }
 
        SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
 
        Cull Off
        Lighting Off
        ZWrite Off
        Blend DstColor OneMinusSrcAlpha
 
        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
 
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
 
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 uv : TEXCOORD0;
            };
            struct v2f
            {
                float4 pos   : SV_POSITION;
                float4 color    : COLOR;
                float2 uv  : TEXCOORD0;
            };
            fixed4 _Color;
            float4 _ClipRect;
            fixed _SpeedX;
            fixed _SpeedY;
            float _transition;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _power;
            sampler2D _TransitionTex;
            float4 _TransitionTex_ST;
            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
 
                // pan texture by offseting uv's over time
                //OUT.texcoord.xy = v.texcoord.xy + frac(_Time.y * float2(_SpeedX, _SpeedY));
                o.uv= TRANSFORM_TEX(v.uv, _TransitionTex);
 
                o.color = v.color * _Color;
                return o;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float4 texColorValue = tex2D(_TransitionTex, i.uv + frac(_Time.y * float2(_SpeedX, _SpeedY)));
                float4 col = float4(0,0,0,1);
                float cutoff = step(length(texColorValue), _transition);
                return col*cutoff;
            }
        ENDCG
        }
    }
    FallBack "UI/Default"
}
