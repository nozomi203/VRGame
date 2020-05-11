Shader "Unlit/DetectBladeOrbit"
{
	Properties
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_PreBladePos("PreBladePos", Vector) = (0,0,0)
		_CurBladePos("CurBladePos", Vector) = (1,0,0)
		_PreBladeDir("PreBladeDir", Vector) = (0,0,1)
		_CurBladeDir("CurBladeDir", Vector) = (0,0,1)
		_BladeLength("BladeLength", float) = 1
		_BladeRadius("BladeRadius", float) = 0.02
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#pragma multi_compile _ BAKE_PAINT
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD2;
			};

			sampler2D _MainTex;

			float3 _PreBladePos;
			float3 _PreBladeDir;
			float3 _CurBladePos;
			float3 _CurBladeDir;
			float _BladeLength;
			float _BladeRadius;
			
			//三角形の内部にtがあるか判定
			//あるなら1，ないなら0
			int isInTriangle(float3 t, float3 p1, float3 p2, float3 p3){
				//周ベクトル
				float3 v1 = p2 - p1;
				float3 v2 = p3 - p2;
				float3 v3 = p1 - p3;
				//法線
				float3 normal = normalize(cross(v1, v2));
				//目的の点へのベクトル
				float3 u1 = t - p1;
				float3 u2 = t - p2;
				float3 u3 = t - p3;
				//外積
				float3 c1 = cross(v1, u1);
				float3 c2 = cross(v2, u2);
				float3 c3 = cross(v3, u3);
				//内積
				float d1 = dot(c1, normal);
				float d2 = dot(c2, normal);
				float d3 = dot(c3, normal);

				return step(0, d1 * d2) * step(0, d1 * d3);

			}

			float distFromTriangle(float3 t, float3 p1, float3 p2, float3 p3){
				
				float3 normal = normalize(cross(p2 - p1, p3 - p1));
				return abs(dot(normal, t - p1));
			}


			v2f vert (appdata v)
			{
				v2f o;
				
				#ifdef BAKE_PAINT
				float2 position = v.uv * 2.0 - 1.0;
				#if UNITY_UV_STARTS_AT_TOP
				position.y *= -1.0;
				#endif
				o.vertex = float4(position, 0.0, 1.0);
				#else
				o.vertex = UnityObjectToClipPos(v.vertex);
				#endif
				o.uv = v.uv;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				_PreBladeDir = normalize(_PreBladeDir);
				_CurBladeDir = normalize(_CurBladeDir);

				float3 p1 = _PreBladePos;
				float3 p2 = p1 + _BladeLength * _PreBladeDir;
				float3 p3 = _CurBladePos;
				float3 p4 = p3 + _BladeLength * _CurBladeDir;

				float dist1 = distFromTriangle(i.worldPos, p1, p2, p4);
				float dist2 = distFromTriangle(i.worldPos, p1, p4, p3);

				float depth = abs(dot(_CurBladeDir, i.worldPos - p3));
				float curBladeRad = _BladeRadius * (1 - depth / _BladeLength);

				int s1 = isInTriangle(i.worldPos, p1, p2, p4);
				s1 = s1 * step(dist1, curBladeRad);
				int s2 = isInTriangle(i.worldPos, p1, p4, p3);
				s2 = s2 * step(dist2, curBladeRad);
				int s = saturate(s1 + s2);
				// sample the texture
				fixed4 col = tex2D (_MainTex, i.uv);
				return col * (1 - s) + fixed4(1,1,1,1) * s;
			}
			ENDCG
		}
	}
}
