
Shader "Custom/BoxProj" {
	Properties{
		_Colour("Colour", Color) = (1,0,0,1)
		_BackColour("Background Colour", Color) = (0,0,0,0)
		_Thickness("Thickness", Range(0.0,0.5)) = 0.05
		_BoxWidth("Width", Float) = 0.4
		_BoxHeight("Height", Float) = 0.4
		_Dropoff("Dropoff", Range(0.01, 4)) = 0.1
		_FillAmount("Fill Amount", Range(0, 360)) = 360
	}
		Subshader{
		Tags{ "Queue" = "Transparent+1" "RenderType" = "Transparent" "DisableBatching" = "True" }
		Pass{
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

	v2f vert(float4 vertex : POSITION)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, vertex);
		o.uvShadow = mul(unity_Projector, vertex) - fixed4(0.5, 0.5, 0 ,0);
		o.uvFalloff = mul(unity_ProjectorClip, vertex);
		UNITY_TRANSFER_FOG(o,o.pos);
		return o;
	}

	fixed4 _Colour; // low precision type is usually enough for colors
	fixed4 _BackColour;
	float _Thickness;
	float _BoxWidth;
	float _BoxHeight;
	float _Dropoff;
	float _FillAmount;

	// r = radius
	// d = distance
	// t = thickness
	// p = % thickness used for dropoff
	float antialias(float x, float y, float t, float p) {
		if (x < ((-_BoxWidth / 2) - 0.5*t))
			return -pow((-_BoxWidth / 2) - x + 0.5*t, 2) / pow(p*t, 2) + 1.0;
		else if (x >((-_BoxWidth / 2) + 0.5*t))
			return -pow((-_BoxWidth / 2) - x - 0.5*t, 2) / pow(p*t, 2) + 1.0;

		else if (x < ((_BoxWidth / 2) - 0.5*t))
			return -pow((_BoxWidth / 2) - x + 0.5*t, 2) / pow(p*t, 2) + 1.0;
		else if (x >((_BoxWidth / 2) + 0.5*t))
			return -pow((_BoxWidth / 2) - x - 0.5*t, 2) / pow(p*t, 2) + 1.0;

		else if (y < ((-_BoxHeight / 2) - 0.5*t))
			return -pow((-_BoxHeight / 2) - x + 0.5*t, 2) / pow(p*t, 2) + 1.0;
		else if (y >((-_BoxHeight / 2) + 0.5*t))
			return -pow((-_BoxHeight / 2) - y - 0.5*t, 2) / pow(p*t, 2) + 1.0;

		else if (y < ((_BoxHeight / 2) - 0.5*t))
			return -pow((_BoxHeight / 2) - x + 0.5*t, 2) / pow(p*t, 2) + 1.0;
		else if (y >((_BoxHeight / 2) + 0.5*t))
			return -pow((_BoxHeight / 2) - y - 0.5*t, 2) / pow(p*t, 2) + 1.0;

		else
			return 1.0;
	}

	sampler2D _ShadowTex;
	sampler2D _FalloffTex;

	fixed4 frag(v2f i) : SV_Target
	{

		float distance = sqrt(pow(i.uvShadow.x, 2) + pow(i.uvShadow.y,2));
	float deltaX = 0 - i.uvShadow.x;
	float deltaY = 0 - i.uvShadow.y;

	fixed4 texS = _Colour;
	texS.a *= antialias(deltaX, deltaY, _Thickness, _Dropoff);

	UNITY_APPLY_FOG_COLOR(i.fogCoord, texS, fixed4(1,1,1,1));
	return texS;
	}
		ENDCG
	}
	}
}
