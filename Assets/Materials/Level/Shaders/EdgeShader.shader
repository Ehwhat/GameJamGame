Shader "Custom/EdgeShader" {
	Properties {
		_BlendAColour("Color", Color) = (1,1,1,1)
		_BlendBColour("Color", Color) = (1,1,1,1)
		_BlendATex("Blend Texture A", 2D) = "white" {}
		_BlendBTex("Blend Texture B", 2D) = "black" {}

		_BlendCliffTint("Blend Cliff Colour", Color) = (1,1,1,1)
		_BlendCliff("Blend Cliff Texture", 2D) = "white" {}

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
		sampler2D _BlendCliff;
		sampler2D _NoiseTexture;

		struct Input {
			float2 uv_BlendATex;
			float2 uv2_BlendBTex;
			float2 uv_NoiseTexture;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color



			fixed4 blendA = tex2D(_BlendATex, IN.uv_BlendATex);
			fixed4 blendB = tex2D(_BlendBTex, IN.uv_BlendATex);

			float blend = tex2D(_NoiseTexture, IN.uv_NoiseTexture);

			float4 c = ((1 - blend)*blendA + blend*blendB);

			
			fixed4 blendCliff = tex2D(_BlendCliff, IN.uv_BlendATex);

			blend = IN.uv2_BlendBTex.y;
			c = ((1 - blend)*c + blend*blendCliff);
			

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
