// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/WaterSurfaceShader" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_BumpMap("BumpMap", 2D) = "bump" {}
		_BumpMag("Bump Magnitude", float) = 1

		_BumpMapDisplacementX("BumpMap Displacement X", float) = 0
		_BumpMapDisplacementY("BumpMap Displacement Y", float) = 0

		_BumpMap2("BumpMap2", 2D) = "bump" {}
		_BumpMag2("Bump2 Magnitude", float) = 1

		_BumpMap2DisplacementX("BumpMap Displacement X", float) = 0
		_BumpMap2DisplacementY("BumpMap Displacement Y", float) = 0

		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_NoiseTex("Noise Texture", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		//Physically based Standard lighting model, and enable shadows on all light types
		//- Standard means standard lightning
		//- vertex:vert to be able to modify the vertices
		//- addshadow to make the shadows look correct after modifying the vertices
		#pragma surface surf Standard  vertex:vert addshadow

		//Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#pragma glsl

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _BumpMap2;

		float _BumpMapDisplacementX;
		float _BumpMapDisplacementY;
		float _BumpMap2DisplacementX;
		float _BumpMap2DisplacementY;

		half _BumpMag;
		half _BumpMag2;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _NoiseTex;
		sampler2D _CameraDepthNormalsTexture;

		//Water parameters
		float _WaterScale;
		float _WaterSpeed;
		float _WaterDistance;
		float _WaterTime;
		float _WaterNoiseStrength;
		float _WaterNoiseWalk;

		float _WaterSize;


		struct Input 
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_BumpMap2;

			float2 screenuv;
			float depth;

			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		//The wave function
		float3 getWavePos(float3 pos)
		{			

			float waveType = pos.z;

			pos.y += sin((_WaterTime * _WaterSpeed + waveType) / _WaterDistance) * _WaterScale;
			pos.y += tex2Dlod(_NoiseTex, float4(pos.x, pos.z + sin(_WaterTime * 0.1), 0.0, 0.0) * _WaterNoiseWalk).a*_WaterNoiseStrength;

			return pos;
		}

		float rand(float3 myVector) {
             return frac(sin(_Time[0] * dot(myVector ,float3(12.9898,78.233,45.5432))) * 43758.5453);
         }

        float rotateFloat2(float2 f, float d){
        	float sinX = sin(d);
			float cosX = cos(d);
			float sinY = sin(d);
			float2x2 rotateMatrix = float2x2(cosX, -sinX, sinY, cosX);
			return mul(f, rotateMatrix);
        }

		void vert(inout appdata_full IN, out Input o) 
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			o.depth = -mul(UNITY_MATRIX_MV, IN.vertex).z * _ProjectionParams.w;
			o.screenuv = ((IN.vertex.xy / IN.vertex.w) + 1 / 2);
			o.screenuv = 1 - o.screenuv;

			//Get the global position of the vertice
			float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);
			float3 v1 = worldPos.xyz + float3(5,0,0);
			float3 v2 = worldPos.xyz + float3(0,0,5);

			v1 = getWavePos(v1);
			v2 = getWavePos(v2);
			//Manipulate the position
			float3 withWave = getWavePos(worldPos.xyz);

			//Convert the position back to local
			float4 localPos = mul(unity_WorldToObject, float4(withWave, worldPos.w));

			//Assign the modified vertice
			IN.vertex = localPos;
			

			//float3 vn = mul(unity_WorldToObject, vna);

		}

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			//Albedo comes from a texture tinted by color

			float screenDepth = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, IN.screenuv).zw);
			float diff = screenDepth - IN.depth;
			float intersect = 0;

			if (diff > 0)
				intersect = 1 - smoothstep(0, _ProjectionParams.w, diff);

			float2 displacedBumpUV = float2(IN.uv_BumpMap.x+_Time.x*_BumpMapDisplacementX, IN.uv_BumpMap.y+_Time.x*_BumpMapDisplacementY);
			float2 displacedBump2UV = float2(IN.uv_BumpMap2.x+_Time.x*_BumpMap2DisplacementX, IN.uv_BumpMap2.y+_Time.x*_BumpMap2DisplacementY);

			float3 norm = IN.worldNormal;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;// + intersect;
			//Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Normal = norm + normalize((UnpackNormal(tex2D(_BumpMap, displacedBumpUV*(_BumpMag / _WaterSize)))) + (UnpackNormal(tex2D(_BumpMap2, -displacedBump2UV*(_BumpMag2 / _WaterSize)))));
			//normalize((UnpackNormal(tex2D(_BumpMap, displacedBumpUV*(_BumpMag/_WaterSize))))+(UnpackNormal(tex2D(_BumpMap2, -displacedBump2UV*(_BumpMag2/_WaterSize)))));

		}
		
		ENDCG
	}
	FallBack "Diffuse"
}