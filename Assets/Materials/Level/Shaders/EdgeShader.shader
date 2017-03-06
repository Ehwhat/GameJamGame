Shader "Custom/EdgeShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_BlendFrom("Blend From Texture", 2D) = "white" {}
		_BlendTo("Blend To Texture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _BlendFrom;
		sampler2D _BlendTo;


		struct Input {
			float2 uv_BlendFrom;
			float2 uv2_BlendTo;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 blendFrom = tex2D(_BlendFrom, IN.uv_BlendFrom);
			fixed4 blendTo = tex2D(_BlendTo, IN.uv_BlendFrom);

			float blend = IN.uv2_BlendTo.y;

			float4 c = ((1 - blend)*blendFrom + blend*blendTo);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
