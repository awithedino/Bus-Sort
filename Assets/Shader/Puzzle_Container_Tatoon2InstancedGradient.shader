Shader "Puzzle/Container/Tatoon2InstancedGradient" {
	Properties {
		_MainTexture ("MainTexture", 2D) = "white" {}
		_ColorB ("Color B", Vector) = (1,1,1,1)
		_ColorA ("Color A", Vector) = (1,1,1,1)
		[Toggle] _UseGradient ("Use Gradient", Float) = 0
		_GradientPosition ("Gradient Position", Float) = 0
		_GradientSize ("Gradient Size", Range(0, 10)) = 0
		_GradientRotation ("Gradient Rotation", Float) = 0
		[Toggle] _ChangeAxis ("Change Axis", Float) = 0
		_ShadowColor ("Shadow Color", Vector) = (0.66,0.66,0.66,1)
		_TextureRamp ("_TextureRamp", 2D) = "white" {}
		_ShadowSize ("ShadowSize", Range(0, 2)) = 0
		_ShadowBlend ("ShadowBlend", Range(0, 1)) = 0.529
		[Toggle] _UseRamp ("UseRamp", Float) = 0
		[Toggle] _UseShadow ("UseShadow", Float) = 1
		[NoScaleOffset] _SpecularMap ("Specular Map", 2D) = "gray" {}
		[Toggle] _UseSpecular ("UseSpecular", Float) = 1
		_SpecularTextureTiling ("Specular Texture Tiling", Float) = 0
		_SpecularTextureRotation ("Specular Texture Rotation", Float) = 0
		[Toggle] _SpecularTextureViewProjection ("Specular Texture View Projection", Float) = 0
		[Toggle] _SpecLightColor ("Spec Light Color", Float) = 0
		_SpecularColor ("Specular Color", Vector) = (1,1,1,0)
		_SpecularLightIntensity ("Specular Light Intensity", Range(0, 10)) = 1
		_SpecularSize ("Specular Size", Range(0, 1)) = 0.15
		_SpecularBlend ("Specular Blend", Range(0, 1)) = 0.364
		_EmissiveMap ("EmissiveMap", 2D) = "white" {}
		[Toggle] _UseEmissive ("UseEmissive", Float) = 0
		[HDR] _EmissiveColor ("EmissiveColor", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "VertexLit"
}