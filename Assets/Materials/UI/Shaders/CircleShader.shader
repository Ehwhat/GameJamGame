// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Circle" {
	Properties{
		_Colour("Colour", Color) = (1,0,0,0)
		_Thickness("Thickness", Range(0.0,0.5)) = 0.05
		_Radius("Radius", Range(0.0, 0.5)) = 0.4
		_Dropoff("Dropoff", Range(0.01, 4)) = 0.1
		_FillAmount("Fill Amount", Range(0, 360)) = 360
	}
		SubShader{
		Tags{ "Queue" = "Transparent+1" "RenderType" = "Transparent"  "DisableBatching" = "True" }
		ZWrite Off
		Pass{
		Blend SrcAlpha OneMinusSrcAlpha // Alpha blending
		CGPROGRAM

#pragma vertex vert alpha
#pragma fragment frag
#include "UnityCG.cginc"
#define M_PI 3.1415926535897932384626433832795


	fixed4 _Colour; // low precision type is usually enough for colors
	float _Thickness;
	float _Radius;
	float _Dropoff;
	float _FillAmount;

	struct fragmentInput {
		float4 pos : SV_POSITION;
		float2 uv : TEXTCOORD0;
	};

	fragmentInput vert(appdata_base v)
	{
		fragmentInput o;

		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord.xy - fixed2(0.5,0.5);

		return o;
	}

	// r = radius
	// d = distance
	// t = thickness
	// p = % thickness used for dropoff
	float antialias(float r, float d, float t, float p) {
		if (d < (r - 0.5*t))
			return -pow(d - r + 0.5*t,2) / pow(p*t, 2) + 1.0;
		else if (d >(r + 0.5*t))
			return -pow(d - r - 0.5*t,2) / pow(p*t, 2) + 1.0;
		else
			return 1.0;
	}

	fixed4 frag(fragmentInput i) : SV_Target{
	float distance = sqrt(pow(i.uv.x, 2) + pow(i.uv.y,2));
	float deltaX = 0-i.uv.x;
	float deltaY = 0-i.uv.y;
	float angle = (atan2(deltaX, deltaY) * 180/ M_PI)+180;
	if (angle > 0 && angle < _FillAmount) {
		return fixed4(_Colour.r, _Colour.g, _Colour.b, _Colour.a*antialias(_Radius, distance, _Thickness, _Dropoff));
	}
	return 0;
	}


		ENDCG
	}
	}
}