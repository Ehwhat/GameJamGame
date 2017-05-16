Shader "Hidden/CameraLerp"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TransitionTex("Transition Texture", 2D) = "white" {}
		_Fade("Fade", Range(0, 1)) = 0
	}

		SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

	#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			float4 _MainTex_TexelSize;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				o.uv1 = v.uv;

		#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv1.y = 1 - o.uv1.y;
		#endif

				return o;
			}

			sampler2D _TransitionTex;
			float _Fade;
			sampler2D _MainTex;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 transit = tex2D(_TransitionTex, i.uv1);
				fixed4 screen = tex2D(_MainTex, i.uv);
				return lerp(screen, transit, _Fade*transit.a);

			}

			ENDCG
		}
	}
}
