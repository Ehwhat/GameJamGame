// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Custom/CircleCutProj" {
	Properties {
		_DitherImage("DitherImage", 2d) = "white"{}
		_DitherImageScale("Dither Image Scale", Float) = 2
		_Radius("Radius", Range(0.0, 2.5)) = 0.4
	}
	Subshader {
		Tags { "Queue" = "Geometry+10" "DisableBatching" = "True" }

		Pass {
			ZWrite On
			ColorMask 0
			Blend SrcAlpha SrcAlpha
			Offset -1, -1
		//
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
#define M_PI 3.1415926535897932384626433832795
			
			struct v2f {
				float4 uvShadow : TEXCOORD0;
				float4 uvFalloff : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float4 pos : SV_POSITION;
			};
			
			float4x4 unity_Projector;
			float4x4 unity_ProjectorClip;
			
			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, vertex);
				o.uvShadow = mul(unity_Projector, vertex) - fixed4(0.5, 0.5, 0 ,0);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			float _Radius;
			float _DitherImageScale;
			sampler2D _DitherImage;
			sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			
			fixed4 frag (v2f i) : SV_Target
			{

				float distance = sqrt(pow(i.uvShadow.x, 2) + pow(i.uvShadow.y,2));
				if (distance > _Radius) {
					fixed4 ditherColor = tex2D(_DitherImage, (i.uvShadow*_DitherImageScale + fixed4(0.5, 0.5, 0, 0)));
					if (ditherColor.b > 0.1) {
						clip(-1);
					}
				}
				return 1;
			}
			ENDCG
		}
	}
}
