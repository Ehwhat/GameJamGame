// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/ScreenTransition"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TransitionTex("Transition Texture", 2D) = "white" {}
		_TransitionOverlay("Transition Overlay", 2d) = "white" {}
		_Color("Screen Color", Color) = (1,1,1,1)
		_Cutoff("Cutoff", Range(0, 1)) = 0
		_CutoffEdge("Cutoff Edge", Range(0,1)) = 0.2
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
					o.vertex = UnityObjectToClipPos(v.vertex);
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
				sampler2D _TransitionOverlay;
				float _Cutoff;
				float _CutoffEdge;
				fixed4 _Color;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 transit = tex2D(_TransitionTex, i.uv1);

					fixed4 screen = tex2D(_MainTex, i.uv);

					fixed4 transitionColour = tex2D(_TransitionOverlay,i.uv1) * _Color;

					fixed4 result = screen;

					float realCutoff = (_Cutoff*(1 + _CutoffEdge));

					if (transit.b < (realCutoff)-_CutoffEdge) {
						return result = lerp(screen, transitionColour, _Fade);
					}
					else if (transit.b < realCutoff) {
						return result = lerp(lerp(screen, transitionColour, _Fade), screen, smoothstep(realCutoff - _CutoffEdge, realCutoff, transit.b) );
					}
					return result;
				}					
				ENDCG
			}
		}
}
