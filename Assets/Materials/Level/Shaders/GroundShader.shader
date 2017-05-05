Shader "Custom/GroundShader" {
	Properties {
		_BlendAColour ("Color", Color) = (1,1,1,1)
		_BlendBColour("Color", Color) = (1,1,1,1)
		_BlendARock("Blend A Rock", 2D) = "white" {}
		_BlendATex ("Blend Texture A", 2D) = "white" {}
		_BlendBTex ("Blend Texture B", 2D) = "black" {}

		_BlendANormal("Blend Normal Texture", 2D) = "white"{}
		_BlendBNormal("Blend Normal Texture", 2D) = "white"{}

		_GroundNormal("Overall Ground Normal", 2D) = "white"{}

		_NoiseTexture("Noise", 2D) = "white"{}

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
#pragma glsl
		#pragma target 3.0

		sampler2D _BlendATex;
		sampler2D _BlendBTex;
		sampler2D _BlendARock;

		sampler2D _BlendANormal;
		sampler2D _BlendBNormal;

		sampler2D _NoiseTexture;
		sampler2D _GroundNormal;

		struct Input {
			float2 uv_BlendATex;
			float2 uv_BlendBTex;
			float2 uv_BlendARock;
			float2 uv_BlendANormal;
			float2 uv_BlendBNormal;
			float2 uv_NoiseTexture;
		};

		void vert(inout appdata_full v) {
			half3 groundNormal = UnpackNormal(tex2Dlod(_GroundNormal, v.texcoord1));
			v.vertex.xyz += v.normal * groundNormal.x;
		}

		half _Glossiness;
		half _Metallic;
		fixed4 _BlendAColour;
		fixed4 _BlendBColour;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 blendFrom = (tex2D(_BlendATex, IN.uv_BlendATex) * _BlendAColour)* tex2D(_BlendARock, IN.uv_BlendARock);
			fixed4 blendTo = tex2D(_BlendBTex, IN.uv_BlendBTex) * _BlendBColour;

			half3 blendFromNormal = UnpackNormal(tex2D(_BlendANormal, IN.uv_BlendANormal));
			half3 blendToNormal = UnpackNormal(tex2D(_BlendBNormal, IN.uv_BlendBNormal));
			
			float4 blend = tex2D(_NoiseTexture, IN.uv_NoiseTexture);

			float4 c = ((1 - blend)*blendFrom + blend*blendTo);
			half3 normal = ((1 - blend)*blendFromNormal + blend*blendToNormal);

			o.Albedo = c.rgb;
			o.Normal = normal;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
