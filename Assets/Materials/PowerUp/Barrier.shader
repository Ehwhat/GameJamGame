Shader "Custom/Dome"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Colour ("Colour", Color) = (0,0,0,0)
		_RimLevel ("Rim Level", Range(-1,1)) = 0
	}

	SubShader
	{
		Blend One One
		ZWrite Off
		Cull Off

		Tags
		{
			"RenderType"="Transparent"
			"Queue"="Transparent"
		}

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD2;
				float3 objectPos : TEXCOORD3;
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.objectPos = v.vertex.xyz;		
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.viewDir = normalize(UnityWorldSpaceViewDir(mul(unity_ObjectToWorld, v.vertex)));

				return o;
			}
		
			fixed4 _Colour;
			float _RimLevel;

			/*float triWave(float t, float offset, float yOffset)
			{
				return saturate(abs(frac(offset + t) * 2 - 1) + yOffset);
			}*/

			fixed4 texColour(v2f i, float rim)
			{
				fixed4 mainTex = tex2D(_MainTex, i.uv);
				mainTex.g *= (sin(_Time.z + mainTex.b * 5) + 1);
				return mainTex.r * _Colour + mainTex.g * _Colour;
			}

			fixed4 frag (v2f i) : SV_Target
			{

				float rim = 1 - abs(dot(i.normal, normalize(i.viewDir))) * 2;

				float rimVertTop = 1 - abs(dot(float3(0,1,0), normalize(i.normal)+float3(0,-_RimLevel,0))) * 3;

				float northPole = (i.objectPos.y - 0.45) * 20;
				float glow = max(max(rimVertTop, rim),northPole);

				fixed4 glowColour = fixed4(lerp(_Colour.rgb, fixed3(1, 1, 1), pow(glow, 4)), 1);
				
				fixed4 hexes = texColour(i, rim);

				fixed4 col = _Colour * _Colour.a + glowColour * glow + hexes;
				return col;
			}
			ENDCG
		}
	}
}
