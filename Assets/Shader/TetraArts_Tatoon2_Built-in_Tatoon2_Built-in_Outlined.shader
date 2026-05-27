Shader "TetraArts/Tatoon2/Built-in/Tatoon2_Built-in_Outlined" {
	Properties {
		_Cutoff ("Mask Clip Value", Float) = 0.5
		_ShadowSize ("ShadowSize", Range(0, 2)) = 0
		_ShadowBlend ("ShadowBlend", Range(0, 1)) = 0.529
		_DiffuseColor ("Diffuse Color", Vector) = (0,0.4941176,1,1)
		[Toggle] _UseRim ("UseRim", Float) = 0
		[Toggle] _UseNormalMap ("UseNormalMap", Float) = 0
		_RimColor ("Rim Color", Vector) = (0.5377358,0.5377358,0.5377358,1)
		_MainTexture ("MainTexture", 2D) = "white" {}
		[NoScaleOffset] _RimTexture ("Rim Texture", 2D) = "white" {}
		[Toggle] _RimTextureViewProjection ("Rim Texture View Projection", Float) = 0
		_NormalStrength ("Normal Strength", Float) = 1
		_RimTextureTiling ("Rim Texture Tiling", Float) = 0
		_ShadowColor ("Shadow Color", Vector) = (0.6603774,0.6603774,0.6603774,1)
		_RimTextureRotation ("Rim Texture Rotation", Float) = 0
		[Toggle] _RimLightColor ("Rim Light Color", Float) = 0
		_RimLightIntensity ("Rim Light Intensity", Range(0, 10)) = 0
		[NoScaleOffset] _ShadowTexture ("Shadow Texture", 2D) = "white" {}
		_RimSize ("Rim Size", Range(-1, 2)) = 0.74
		_RimBlend ("Rim Blend", Range(0.1, 10)) = 2.21
		[Toggle] _ShadowTextureViewProjection ("Shadow Texture View Projection", Float) = 0
		[Toggle] _UseSpecular ("UseSpecular", Float) = 1
		_ShadowTextureTiling ("Shadow Texture Tiling", Float) = 0
		[Toggle] _UseGradient ("Use Gradient", Float) = 0
		[NoScaleOffset] _SpecularMap ("Specular Map", 2D) = "gray" {}
		_ShadowTextureRotation ("Shadow Texture Rotation", Float) = 0
		[Toggle] _SpecularTextureViewProjection ("Specular Texture View Projection", Float) = 0
		_ColorB ("Color B", Vector) = (0,0.1264467,1,0)
		_ColorA ("Color A", Vector) = (1,0,0,0)
		[Toggle] _UseShadow ("UseShadow", Float) = 1
		_SpecularTextureTiling ("Specular Texture Tiling", Float) = 0
		_SpecularTextureRotation ("Specular Texture Rotation", Float) = 0
		[Toggle] _UseOutline ("UseOutline", Float) = 0
		_GradientSize ("Gradient Size", Range(0, 10)) = 0
		_SpecularColor ("Specular Color", Vector) = (1,1,1,1)
		[Toggle] _SpecLightColor ("Spec Light Color", Float) = 0
		_GradientPosition ("Gradient Position", Float) = 0
		_SpecularLightIntensity ("Specular Light Intensity", Range(0, 10)) = 1
		_GradientRotation ("Gradient Rotation", Float) = 0
		[Toggle] _OutlineNoise ("OutlineNoise", Float) = 0
		_SpecularSize ("Specular Size", Range(0, 1)) = 0.15
		_SpecularBlend ("Specular Blend", Range(0, 1)) = 0.364
		_NormalMap ("NormalMap", 2D) = "bump" {}
		[HDR] _OutlineColor ("Outline Color", Vector) = (0.1504539,0.1647517,0.1981132,1)
		_OutlineSize ("Outline Size", Float) = 0.1
		_OutlineNoiseScale ("OutlineNoiseScale", Float) = 1
		_Smoothness ("Smoothness", Range(0, 1)) = 0.5
		[Toggle] _Level2 ("Level2", Float) = 0
		[Toggle] _Level3 ("Level3", Float) = 0
		[Toggle] _Animate ("Animate", Float) = 0
		[Toggle] _UseShadowTexture ("UseShadowTexture", Float) = 0
		_XDirectionSpeed ("XDirectionSpeed", Float) = 0
		_YDirectionSpeed ("YDirectionSpeed", Float) = 0
		[Toggle] _ChangeAxis ("ChangeAxis", Float) = 0
		_XSpeed ("XSpeed", Float) = 0
		_YSpeed ("YSpeed", Float) = 0
		_EmissiveMap ("EmissiveMap", 2D) = "white" {}
		[Toggle] _UseEmissive ("UseEmissive", Float) = 0
		[HDR] _EmissiveColor ("EmissiveColor", Vector) = (2.996078,0.0611825,0,0)
		_TextureRamp ("TextureRamp", 2D) = "white" {}
		[Toggle] _UseRamp ("UseRamp", Float) = 0
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
	Fallback "Diffuse"
	//CustomEditor "TatoonEditor_OutlineIncluded"
}