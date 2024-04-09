Shader "Unlit/Eyeball"
{
Properties
    {
        [Header(Main)]
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}
      
        [Header(Iris)]
        _IrisTex("Iris Texture (RGB)", 2D) = "black" {}
        _IrisTexColor("Iris Texture Tint", Color) = (1,0,0,1)
        _Radius("Iris Radius", Range(0,1)) = 0.4
        _IrisColor("Iris Color", Color) = (0,1,1,1)
        _IrisColorOut("Iris Color Out", Color) = (0,1,0,1)
        _IrisScaleX("Iris Scale X", Range(0,2)) = 1
        _IrisScaleY("Iris Scale Y", Range(0,2)) = 1
        _Speed("Iris Scroll Speed", Range(-10,10)) = 0
        _Scale("Iris Texture Scale", Range(0.1,10)) = 10
        [Toggle(TEXTURE)] _TEXTURE("Circlular Texture", Float) = 0
        _Distort("Iris Texture Distortion", Range(0,1)) = 0.5
        _Brightness("Iris Texture Brigthness", Range(0,5)) = 1
 
        [Header(Pupil)]     
        _PupilTex("Pupil Texture (RGB)", 2D) = "white" {}
        _PupilScale("Pupil Tex Radius", Range(0,1)) = 0.3
        _RadiusPupil("Pupil Radius", Range(0,0.5)) = 0.1
        _PupilColor("Pupil Color", Color) = (0,0,0,1)
        _PupilColorOut("Pupil Color Out", Color) = (0,0,1,1)
        _PupilScaleX("Pupil Scale X", Range(0,1)) = 0.5
        _PupilScaleY("Pupil Scale Y", Range(0,1)) = 0.5
 
        [Header(Highlight and Iris Edge)]
        _GlintTex("Glint Texture (RGB)", 2D) = "black" {}
        _GlintScale("Glint Scale", Range(0,1)) = 0.3
        _Edgewidth("Iris Edge Width", Range(0,2)) = 0.1
        _IrisEdgeColor("Iris Edge Color", Color) = (0,0,0,1)
                    
        
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 200
          CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf  Standard fullforwardshadows vertex:vert
        #pragma target 3.5
      
        #pragma shader_feature TEXTURE
 
        sampler2D _MainTex, _IrisTex, _PupilTex, _GlintTex;
        struct Input
        {
            float2 uv_MainTex;
            float4 objPos;
            float3 viewDir;
            float3 worldPos;
            float3 worldNormal;
            float3 normal;
        };
 
 
        float _Radius, _RadiusPupil;
        fixed4 _Color, _IrisColor, _PupilColor, _PupilColorOut, _IrisColorOut, _IrisTexColor, _IrisEdgeColor;
        float _PupilScaleX, _PupilScaleY, _Edgewidth, _IrisScaleX, _IrisScaleY, _Scale, _Speed, _Distort, _Brightness;
        float _PupilScale;
        float  _GlintScale;
 
        void vert(inout appdata_full v, out Input o) {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.objPos = v.vertex;
            o.worldPos = UnityObjectToWorldDir(v.vertex);
            o.worldNormal = UnityObjectToWorldNormal(v.vertex);
            o.normal = v.normal;
        }
 
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
 
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            const float PI = 3.14159265359;
            // Puff out the direction to compensate for interpolation.
            float3 direction = normalize(IN.normal);

            // Get a longitude wrapping eastward from x-, in the range 0-1.
            float longitude = 0.5 - atan2(direction.z, direction.x) / (2.0f * PI);
            // Get a latitude wrapping northward from y-, in the range 0-1.
            float latitude = 0.5 + asin(direction.y) / PI;

            // Combine these into our own sampling coordinate pair.
            float2 customUV = float2(longitude, latitude);
            // circles
            
            float dis= distance(0, float3(IN.objPos.x * _IrisScaleX, IN.objPos.y * _IrisScaleY, IN.objPos.z - 0.5)); 
            float disPup = (distance(0, float3(IN.objPos.x * _PupilScaleX, IN.objPos.y * _PupilScaleY , IN.objPos.z - 0.5))); 
            float irisRadius = 1- saturate(dis / _Radius);
            float pupilRadius = 1 - saturate(disPup / _RadiusPupil);
            float irisEdge = 1 - saturate(dis / _Radius - _Edgewidth);
        
            // point in center of eye, flipped
            float2 uv = float2(-IN.objPos.x , IN.objPos.y );
            // uv for the pupil, adjusted for a sphere
            float2 uvPup  = uv / (_PupilScale * 2);
            uvPup += 0.5;
            
            // uv for the glint, adjusted for a sphere
            float2 uvGlint = uv / (_GlintScale * 2);
            uvGlint.x += 0.5;
            uvGlint.y += 0.5;
            //uvGlint -= IN.viewDir*0.2;
        
            
            // Iris texture
            float4 i = tex2D(_IrisTex, IN.uv_MainTex);
 
 
            // glint and pupil texture
            float4 glint = tex2D(_GlintTex, uvGlint);
            float4 pup = tex2D(_PupilTex, uvPup);
 
            // add extra tint
            i *= _IrisTexColor;
            i *= _Brightness;
 
            // increase strength then clamp it for a smooth circle
            float irisCircle = saturate(irisRadius * 20);
            float pupilCircle = saturate(pupilRadius * 20);
            pupilCircle *= pup.r;
            float irisEdgeCircle = saturate(irisEdge * 10);
            
            // eyewhite is everything but the iris 
            float4 eyeWhite = _Color * (1 - irisEdgeCircle);
            glint *= irisCircle;
            // subract to avoid bleeding through of colors
            irisEdgeCircle -=irisCircle ;
        
            irisCircle -= pupilCircle;
 
            // lerp colors
            float4 irisLerp = lerp(_IrisColorOut,_IrisColor, irisRadius ) + i;
            float4 irisColored = irisCircle * irisLerp;
        
            float4 pupilLerp = lerp(_PupilColorOut,_PupilColor, pupilRadius);
            float4 pupilColored = pupilCircle * pupilLerp;
 
 
            float4 irisEdgeColored = irisEdgeCircle * _IrisEdgeColor;


            float4 mainCol = tex2D(_MainTex, customUV);
            // all together
            o.Albedo = eyeWhite + irisColored + pupilColored + irisEdgeColored + mainCol;
            // glint in emission
            o.Emission =  glint;
            o.Smoothness = 0.75;
 
 
        }
        ENDCG
    }
    FallBack "Diffuse"
}
