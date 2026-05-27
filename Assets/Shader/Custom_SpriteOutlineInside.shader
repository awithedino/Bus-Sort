Shader "Custom/SpriteOutlineInside" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Vector) = (1,1,1,1)
		_OutlineDarken ("Outline Darken", Range(0, 1)) = 0.65
		_OutlineWidth ("Outline Width (px)", Range(0, 4)) = 1
		_AlphaThreshold ("Alpha Threshold", Range(0, 1)) = 0.01
		_ShadowDeepen ("Shadow Deepen", Range(1, 8)) = 3
		_ShadowDarken ("Shadow Darken", Range(0, 1)) = 0.8
		_ShadowStrength ("Shadow Strength", Range(0, 1)) = 0.8
		_ShadowAngle ("Shadow Angle (Deg)", Range(0, 360)) = 315
		_ShadowStart ("Shadow Start", Range(0, 1)) = 0.3
		_ShadowSoftness ("Shadow Softness", Range(0.001, 1)) = 0.2
		_ShadowOffset ("Shadow Offset", Range(0, 1)) = 0.35
		_ShadowWidth ("Shadow Width", Range(0, 2)) = 1
		[MaterialToggle] PixelSnap ("Pixel Snap", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy) * _Color;
			}

			ENDHLSL
		}
	}
	Fallback "Sprites/Default"
}