Shader "Custom/MakeGash" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DamageTex("DamageTex", 2D) = "black"{}
		_GashMap("GashMap", 2D) = "black"{}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_GashEmission("GashEmission", Color) = (0,0,0,0)
		_DieEmission("DieEmission", Color) = (0,1,0,1)
		_EmissionRate("EmissionRate", Range(0,1))=0.0

		_BrightTex("BrightTex", 2D) = "white"{}
		_StencilTex("StencilTex", 2D) = "black"{}
		_BrightColor("BrightColor", Color) = (0,0.3,1,1)
		_ScrollSpeed("ScrollSpeed", Float) = 3
		_BrightRate("BrightRate", Float) = 10
	}

	SubShader {
		Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alphatest:_Cutoff

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _DamageTex;
		sampler2D _GashMap;
		sampler2D _BrightTex;
		sampler2D _StencilTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BrightTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _GashEmission;
		fixed4 _DieEmission;
		fixed4 _Color;
		fixed4 _BrightColor;
		float _EmissionRate;
		float _ScrollSpeed;
		float _BrightRate;


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color

			fixed2 scrolledUVx = IN.uv_BrightTex;
			fixed2 scrolledUVy = IN.uv_BrightTex;
			scrolledUVx.x += _Time * _ScrollSpeed;
			scrolledUVy.y += _Time * _ScrollSpeed;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 d = tex2D(_DamageTex, IN.uv_MainTex);
			fixed4 g = tex2D(_GashMap, IN.uv_MainTex);
			fixed4 s = tex2D(_StencilTex, IN.uv_MainTex);
			fixed4 bx = tex2D(_BrightTex, scrolledUVx);
			fixed4 by = tex2D(_BrightTex, scrolledUVy);

			fixed3 base = c.rgb * (1 - s) + (bx.rgb + by.rgb) * s;
			o.Albedo = base * (1 - g.r) + d.rgb * g.r;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Emission = _BrightColor.xyz * s * _BrightRate * (bx.xyz + by.xyz) + (_GashEmission.xyz * d.rgb * 10) * g.r * (1 - _EmissionRate) + _DieEmission.xyz * 3 * _EmissionRate;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Transparent/Cutout/Diffuse"
}
