Shader "Unlit/LightShaft"
{
Properties
    {
       _noise1("noise1", 2D) = "white" {}
       _noise1AlphaCutoff("alpha1Cutoff", float) = 0.1
       _noise2("noise2", 2D) = "white" {}
       _noise2AlphaCutoff("alpha2Cutoff", float) = 0.1
       _noise1ScrollSpeed("noise1Scroll", float) = 1.0
       _noise2ScrollSpeed("noise2Scroll", float) = 2.0
       _frontFaceColor("Front face color", color) = (0,0,0,0)
       _backFaceColor("back face color", color) = (1,1,1,1)
       [MaterialToggle] _onlyNoise("only noise", float) = 0.0
       _originPosX("originPosX", float) = 0.0
       _originPosY("originPosY", float) = 0.0
       _originPosZ("originPosZ", float) = 0.0
       _distanceFromOriginCutoff("distanceFromOriginCutoff", float) = 0.0
       _fadeOffStrength("fade strength", float) = 1.0
       _maximumDistance("maximum distance", float) = 50.0
    }
   SubShader {
      Tags { "Queue" = "Transparent" } 
         // draw after all opaque geometry has been drawn
      Pass {
         Cull Front // first pass renders only back faces 
             // (the "inside")
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
         #include "UnityCG.cginc"
         sampler2D _noise1 ,_noise2;
         float _noise1ScrollSpeed,_noise2ScrollSpeed,_noise1AlphaCutoff,_noise2AlphaCutoff, _onlyNoise,_originPosX,_originPosY,_originPosZ,_distanceFromOriginCutoff,_fadeOffStrength,_maximumDistance;
         float4 _backFaceColor;
         struct v2f
         {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            float4 color : COLOR;
            float3 worldPos : TEXCOORD1;
            float3 worldNorm : TEXCOORD2;
         };
         v2f vert (appdata_full v)
         {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.color = v.color;
            o.uv = v.texcoord;
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            o.worldNorm = UnityObjectToWorldNormal(v.vertex);
            return o;
         }

         fixed4 frag(v2f i) : SV_Target
         {
            float4 noise1 = tex2D(_noise1, i.uv+_noise1ScrollSpeed*_Time.x);
            float4 noise2 = tex2D(_noise2, i.uv+_noise2ScrollSpeed*_Time.x);
            float4 finalCol = _backFaceColor;
            //float3 dirToCam = saturate(_WorldSpaceCameraPos - i.worldPos);
            //float dirToCamDotNormal = dot(dirToCam, i.worldNorm);

            float distanceFromOrigin = length(i.worldPos-float3(_originPosX,_originPosY,_originPosZ));
            float fadeOut = _distanceFromOriginCutoff/(pow(distanceFromOrigin, _fadeOffStrength));
            if( distanceFromOrigin > _distanceFromOriginCutoff)
            {
               if(distanceFromOrigin > _maximumDistance)
               {
                  discard;
               }
               return finalCol*fadeOut;
            }

            if(length(noise1) > _noise1AlphaCutoff)
            {
               finalCol *= noise1;
            }
            else
            {
               discard;
            }
            if(length(noise2) > _noise2AlphaCutoff)
            {
               finalCol *= noise2;
            }
            if(_onlyNoise > 0.0)
            {
               finalCol = noise1*_backFaceColor;
            }
            return finalCol*fadeOut;
         }
         ENDCG  
      }

      Pass {
         Cull Back // second pass renders only front faces 
             // (the "outside")
         ZWrite Off // don't write to depth buffer 
            // in order not to occlude other objects
         Blend SrcAlpha OneMinusSrcAlpha // use alpha blending

         CGPROGRAM 
 
         #pragma vertex vert 
         #pragma fragment frag
         #include "UnityCG.cginc"
         sampler2D _noise1,_noise2;
         float _noise1ScrollSpeed,_noise2ScrollSpeed,_noise1AlphaCutoff,_noise2AlphaCutoff,_originPosX,_originPosY,_originPosZ,_distanceFromOriginCutoff,_fadeOffStrength,_maximumDistance;
         float4 _frontFaceColor;

         struct v2f
         {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
            float4 color : COLOR;
            float3 worldPos : TEXCOORD1;
            float3 worldNorm : TEXCOORD2;
         };
         v2f vert (appdata_full v)
         {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
            o.color = v.color;
            o.uv = v.texcoord;
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            o.worldNorm = UnityObjectToWorldNormal(v.vertex);
            return o;
         }

         fixed4 frag(v2f i) : SV_Target
         {
            float4 noise1 = tex2D(_noise1, i.uv-_noise1ScrollSpeed*_Time.x/10);
            float4 noise2 = tex2D(_noise2, i.uv-_noise2ScrollSpeed*_Time.x/10);
            float4 finalCol = _frontFaceColor;
            //float3 dirToCam = saturate(_WorldSpaceCameraPos - i.worldPos);
            //float dirToCamDotNormal = dot(dirToCam, i.worldNorm);
            float distanceFromOrigin = length(i.worldPos-float3(_originPosX,_originPosY,_originPosZ));
            float fadeOut = _distanceFromOriginCutoff/(pow(distanceFromOrigin, _fadeOffStrength));
            if( distanceFromOrigin > _distanceFromOriginCutoff)
            {
               if(distanceFromOrigin > _maximumDistance)
               {
                  discard;
               }
               return _frontFaceColor*fadeOut;
            }
            
            if(length(noise1) > _noise1AlphaCutoff)
            {
               finalCol *= noise1;
            }
            if(length(noise2) > _noise2AlphaCutoff)
            {
               finalCol *= noise2;
            }
            return _frontFaceColor*fadeOut;
         }
         ENDCG  
      }
   }
}
