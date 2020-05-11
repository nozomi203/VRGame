Shader "Unlit/MeshBreak"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TimeOffset("TimeOffset", float) = 0
		_Emission("Emission", Color) = (0,0,0,0)
		_MaxRotSpeed("MaxRotSpeed", float) = 720
		_MaxVerSpeed("MaxVerSpeed", float) = 1
		_MaxHorSpeed("MaxHorSpeed", float) = 1.5
		_RandomSeed("RandomSeed", int) = 10
		_Alpha("Alpha", Range(0,1)) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2g 
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			struct g2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _TimeOffset;
			fixed4 _Emission;
			float _MaxRotSpeed;
			float _MaxVerSpeed;
			float _MaxHorSpeed;
			int _RandomSeed;
			fixed _Alpha;

			#define _RadFactor 0.0174533


			//ベクトルの回転
			float3 rotate(float3 pos, float theta, float3 n, float3 pivot)
			{
				float angleSin = sin(_RadFactor * theta);
				float angleCos = cos(_RadFactor * theta);

				float4x4 r;
				r._11 = angleCos + n.x * n.x * (1 - angleCos);
				r._12 = n.x * n.y * (1 - angleCos) - n.z * angleSin;
				r._13 = n.x * n.z * (1 - angleCos) + n.y * angleSin;
				r._14 = 0.0;
				r._21 = n.x * n.y * (1 - angleCos) + n.z * angleSin;
				r._22 = angleCos + n.y * n.y * (1 - angleCos);
				r._23 = n.y * n.z * (1 - angleCos) - n.x * angleSin;
				r._24 = 0.0;
				r._31 = n.x * n.z * (1 - angleCos) - n.y * angleSin;
				r._32 = n.y * n.z * (1 - angleCos) + n.x * angleSin;
				r._33 = angleCos + n.z * n.z * (1 - angleCos);
				r._34 = 0.0;
				r._41_42_43_44 = float4(0, 0, 0, 1);


				float3 relativePos = pos - pivot;
				return pivot + mul(r, float4(relativePos, 1.0)).xyz;
			}

			//乱数生成
			float GetRandomNumber(float2 texCoord, int Seed)
			{
				return frac(sin(dot(texCoord.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
			}

			//tanh
			float tanh(float x) {
				return (exp(x) - exp(-x)) / (exp(x) + exp(-x));
			}


			
			v2g vert (appdata v)
			{
				v2g o;
				o.vertex = v.vertex;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2g input[3], inout TriangleStream<g2f> stream) {

				float3 v1 = input[0].vertex.xyz - input[2].vertex.xyz;
				float3 v2 = input[1].vertex.xyz - input[2].vertex.xyz;
				float3 normal = normalize(cross(v1, v2));
				float3 center = (input[0].vertex.xyz + input[1].vertex.xyz + input[2].vertex.xyz) / 3;

				float3 _Axis = rotate(normal, 360 * GetRandomNumber(input[0].uv, _RandomSeed), float3(1, 0, 0), float3(0, 0, 0));
				_Axis = rotate(_Axis, 360 * GetRandomNumber(input[1].uv, _RandomSeed), float3(0, 1, 0), float3(0, 0, 0));
				_Axis = rotate(_Axis,360 * GetRandomNumber(input[2].uv, _RandomSeed), float3(0, 0, 1), float3(0, 0, 0));

				float _RotSpeed = _MaxRotSpeed * 2 * (GetRandomNumber(input[0].uv, _RandomSeed + 1) - 0.5);
				float _HorSpeed = _MaxHorSpeed * (1 + 2 * (GetRandomNumber(input[1].uv, _RandomSeed + 1) - 0.5));
				float _VerSpeed = _MaxVerSpeed * (1 + GetRandomNumber(input[2].uv, _RandomSeed + 1));
				float _ElapsedTime = _Time.y - _TimeOffset;

				[unroll]
				for (int x = 0; x < 3; x++) {
					v2g i = input[x];
					g2f o;
					//回転
					i.vertex.xyz = rotate(i.vertex.xyz, _RotSpeed * _ElapsedTime, _Axis, center);
					//水平移動
					i.vertex.xyz += _HorSpeed * tanh(4 * _ElapsedTime) * float4(normal, 0);
					//垂直移動
					i.vertex.xyz += 0.5 * _VerSpeed * _ElapsedTime * _ElapsedTime * float4(0,1,0,0);

					o.vertex = UnityObjectToClipPos(i.vertex);
					o.uv = i.uv;
					stream.Append(o);
				}
			}


			
			fixed4 frag (g2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				col += _Emission * 2;
				col.w = _Alpha;
				return col;
			}
			ENDCG
		}
	}
}
