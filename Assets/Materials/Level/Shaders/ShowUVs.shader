Shader "Unlit/WorldSpaceNormals"
{
	Properties
	{
		_BlendFrom("Blend From Texture", 2D) = "white" {}
		_BlendTo("Blend To Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		// include file that contains UnityObjectToWorldNormal helper function
#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION; // vertex position
		float2 uv : TEXCOORD0; // texture coordinate
		float3 normal : TEXCOORD1;
	};

		struct v2f {
		// we'll output world space normal as one of regular ("texcoord") interpolators
		half3 objNormal : TEXCOORD1;
		float3 coords : TXCOORD2;
		float2 uv : TEXCOORD0;
		float4 pos : SV_POSITION;
	};

	v2f vert(float4 pos : POSITION, float3 normal : NORMAL, float2 uv : TEXCOORD0)
	{
		v2f o;
		o.pos = UnityObjectToClipPos(pos);
		o.uv = uv;
		o.objNormal = normal;
		o.coords = pos.xyz;
		return o;
	}

	sampler2D _BlendFrom;
	sampler2D _BlendTo;

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 c = 0;

		fixed4 blendFrom = tex2D(_BlendFrom, i.coords.xz);
		fixed4 blendTo = tex2D(_BlendTo, i.coords.xz);

		c.rgb = (blendFrom*1-i.uv.y)+(blendTo*i.uv.y);

		return c;
	}
		ENDCG
	}
	}
}