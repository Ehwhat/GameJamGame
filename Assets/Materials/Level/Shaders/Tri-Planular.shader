Shader "Custom/Tri-Planular" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Scale("Scaling", Float) = 100
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

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 worldNormal;
		};

		half _Glossiness;
		half _Metallic;
		float _Scale;
		fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float3 projNormal = saturate(pow(IN.worldNormal * 1.4, 4));

			// SIDE X
			float3 x = tex2D(_MainTex, frac(IN.worldPos.zy*_Scale)) * abs(IN.worldNormal.x);
			// TOP / BOTTOM
			float3 y = tex2D(_MainTex, frac(IN.worldPos.zx*_Scale)) * abs(IN.worldNormal.y);
			// SIDE Z	
			float3 z = tex2D(_MainTex, frac(IN.worldPos.xy*_Scale)) * abs(IN.worldNormal.z);

			o.Albedo = z;
			o.Albedo = lerp(o.Albedo, x, projNormal.x);
			o.Albedo = lerp(o.Albedo, y, projNormal.y);

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
