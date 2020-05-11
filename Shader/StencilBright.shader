Shader "Custom/StencilBright"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		_BrightColor("Color", Color) = (0.5,0,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_StencilTex("StencilTex", 2D) = "black"{}
		_BrightTex("BrightTex", 2D) = "white"{}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		_ScrollSpeed("ScrollSpeed", float) = 1
		_EmissionRate("EmissionRate", float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _StencilTex;
		sampler2D _BrightTex;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_BrightTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
		fixed4 _BrightColor;
		float _ScrollSpeed;
		float _EmissionRate;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			fixed2 scrolledUVx = IN.uv_BrightTex;
			fixed2 scrolledUVy = IN.uv_BrightTex;
			scrolledUVx.x += _Time * _ScrollSpeed;
			scrolledUVy.y += _Time * _ScrollSpeed;
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 s = tex2D(_StencilTex, IN.uv_MainTex);
			fixed4 bx = tex2D(_BrightTex, scrolledUVx);
			fixed4 by = tex2D(_BrightTex, scrolledUVy);

            o.Albedo = c.rgb * (1 - s) + (bx.rgb + by.rgb) * s;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
			o.Emission = _BrightColor.xyz * s * _EmissionRate * (bx.xyz + by.xyz);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
