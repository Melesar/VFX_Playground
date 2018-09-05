// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/Fresnel"
{
	Properties
	{
		_Power ("Fresnel power", Range(1, 5)) = 1
		_Exponent ("Fresnel exponent", Range(1, 5)) = 1
		_Color ("Fresnel color", Color) = (1, 1, 1, 1)
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
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half3 normal : TEXCOORD0;
				float4 color : COLOR;
				float3 viewDir : TEXCOORD1;
			};

			float _Power;
			float _Exponent;
			fixed4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = normalize(mul(unity_ObjectToWorld, v.normal));
				o.color = v.color;
				o.viewDir = WorldSpaceViewDir(v.vertex);
				return o;
			}

			float fresnel (fixed3 normal, fixed3 viewDir)
			{
				float d = dot(normal, viewDir);
				return 1 - pow(d, _Exponent);
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float f = fresnel(i.normal, i.viewDir);
				f = pow(f, _Power);

				return lerp(fixed4(0,0,0,1), _Color, f);
			}
			ENDCG
		}
	}
}
