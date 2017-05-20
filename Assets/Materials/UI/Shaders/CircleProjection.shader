// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Projector' with 'unity_Projector'
// Upgrade NOTE: replaced '_ProjectorClip' with 'unity_ProjectorClip'

Shader "Custom/CircleProj" {
	Properties {
		_Colour("Colour", Color) = (1,0,0,1)
		_BackColour("Background Colour", Color) = (0,0,0,0)
		_Thickness("Thickness", Range(0.0,0.5)) = 0.05
		_Radius("Radius", Range(0.0, 2.5)) = 0.4
		_Dropoff("Dropoff", Range(0.01, 4)) = 0.1
		_FillAmount("Fill Amount", Range(0, 360)) = 360
	}
	Subshader {
		Tags { "Queue" = "Geometry+1" "RenderType" = "Transparent" "DisableBatching" = "True" }
		Pass {
			ZWrite Off
			//ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
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
				o.pos = UnityObjectToClipPos (vertex);
				o.uvShadow = mul (unity_Projector, vertex) - fixed4(0.5, 0.5, 0 ,0);
				o.uvFalloff = mul (unity_ProjectorClip, vertex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}
			
			fixed4 _Colour; // low precision type is usually enough for colors
			fixed4 _BackColour;
			float _Thickness;
			float _Radius;
			float _Dropoff;
			float _FillAmount;

			// r = radius
			// d = distance
			// t = thickness
			// p = % thickness used for dropoff
			float antialias(float r, float d, float t, float p) {
				if (d < (r - 0.5*t))
					return -pow(d - r + 0.5*t, 2) / pow(p*t, 2) + 1.0;
				else if (d >(r + 0.5*t))
					return -pow(d - r - 0.5*t, 2) / pow(p*t, 2) + 1.0;
				else
					return 1.0;
			}

			sampler2D _ShadowTex;
			sampler2D _FalloffTex;
			
			fixed4 frag (v2f i) : SV_Target
			{

				float distance = sqrt(pow(i.uvShadow.x, 2) + pow(i.uvShadow.y,2));
				float deltaX = 0 - i.uvShadow.x;
				float deltaY = 0 - i.uvShadow.y;
				float angle = (atan2(deltaX, deltaY) * 180 / M_PI) + 180;
				
				fixed4 texS = _Colour;
				if (!(angle > 0 && angle < _FillAmount)) {
					texS = _BackColour;
				}
				texS.a *= antialias(_Radius, distance, _Thickness, _Dropoff);

				UNITY_APPLY_FOG_COLOR(i.fogCoord, texS, fixed4(1,1,1,1));
				return texS;
			}
			ENDCG
		}
	}
}
