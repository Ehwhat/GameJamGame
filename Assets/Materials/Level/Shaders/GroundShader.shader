Shader "Custom/GroundShader" {
	Properties {
		_BlendAColour ("Color", Color) = (1,1,1,1)
		_BlendBColour("Color", Color) = (1,1,1,1)
		_BlendATex ("Blend Texture A", 2D) = "white" {}
		_BlendBTex ("Blend Texture B", 2D) = "black" {}

		_NoiseTexture("Noise", 2D) = "white"{}

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

		sampler2D _BlendATex;
		sampler2D _BlendBTex;
		sampler2D _NoiseTexture;

		struct Input {
			float2 uv_BlendATex;
			float2 uv_BlendBTex;
			float2 uv_NoiseTexture;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _BlendAColour;
		fixed4 _BlendBColour;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 blendFrom = tex2D(_BlendATex, IN.uv_BlendATex);
			fixed4 blendTo = tex2D(_BlendBTex, IN.uv_BlendBTex);

			float blend = tex2D(_NoiseTexture, IN.uv_NoiseTexture);

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
