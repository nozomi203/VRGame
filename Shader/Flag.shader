Shader "Custom/Flag"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_Color("Color", Color) = (1,1,1,1)
		_Cutoff("Cutoff", float) = 0.5
		_Offset("Offset", float) = 0
		_PosParam("PosParam", float) = 1
		_TimeParam("TimeParam", float) = 1
		_AmpParam("AmpParam", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200
		Cull Off

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Lambert fullforwardshadow vertex:vert alphatest:_Cutoff

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		float _Offset;
		float _PosParam;
		float _TimeParam;
		float _AmpParam;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		void vert(inout appdata_full v){
			v.vertex.xyz += float3(0,1,0) * _AmpParam * sin(v.vertex.x * _PosParam + _Time.z * _TimeParam) * (v.vertex.x - _Offset);
			//v.normal = cross(float3(0,0,1), float3(1, cos(v.vertex.x + _Time.y), 0));
		}

        void surf (Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Transparent/Cutout/Diffuse"
}
