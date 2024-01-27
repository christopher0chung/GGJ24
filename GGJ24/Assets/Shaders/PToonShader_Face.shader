// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PToonShader_Face"
{
	Properties
	{
		_FaceSheet("FaceSheet", 2D) = "white" {}
		_Albedo("Albedo", Color) = (0,0,0,0)
		_Normals("Normals", 2D) = "bump" {}
		_NormalPower("Normal Power", Range( 0 , 5)) = 0
		_LightColor("Light Color", Color) = (0,0,0,0)
		_Shininess("Shininess", Range( 0.01 , 10)) = 1
		_SpecularIntensity("Specular Intensity", Range( 0 , 1)) = 0
		_MaxLitValue("Max Lit Value", Range( 0 , 1)) = 0
		_MinLitValue("Min Lit Value", Range( 0 , 1)) = 0
		_ShadowTint("Shadow Tint", Color) = (0,0,0,0)
		_ShadowTintStrength("ShadowTintStrength", Range( 0 , 1)) = 0
		_MaxShadowValue("Max Shadow Value", Range( 0 , 1)) = 0
		_MinShadowValue("Min Shadow Value", Range( 0 , 1)) = 0
		_LightClamp("Light Clamp", Range( 0 , 1)) = 0
		_ShadowBoost("Shadow Boost", Range( 0 , 1)) = 0
		_RowsAndColums("Rows And Colums", Vector) = (5,5,0,0)
		_FrameIndex("Frame Index", Range( 0 , 15)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha , One One
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _Albedo;
		uniform sampler2D _FaceSheet;
		uniform float2 _RowsAndColums;
		uniform float _FrameIndex;
		uniform float4 _ShadowTint;
		uniform float _ShadowTintStrength;
		uniform sampler2D _Normals;
		uniform float4 _Normals_ST;
		uniform float _NormalPower;
		uniform float _ShadowBoost;
		uniform float _LightClamp;
		uniform float _MinShadowValue;
		uniform float _MaxShadowValue;
		uniform float4 _LightColor;
		uniform float _Shininess;
		uniform float _SpecularIntensity;
		uniform float _MinLitValue;
		uniform float _MaxLitValue;


		float3 ASESafeNormalize(float3 inVec)
		{
			float dp3 = max(1.175494351e-38, dot(inVec, inVec));
			return inVec* rsqrt(dp3);
		}


		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 break135_g33 = i.uv_texcoord;
			float2 appendResult136_g33 = (float2(break135_g33.x , ( 1.0 - break135_g33.y )));
			float2 FlipBookUV141_g33 = appendResult136_g33;
			float FlipBookColumns143_g33 = _RowsAndColums.y;
			float FlipBookRows144_g33 = _RowsAndColums.x;
			float2 appendResult83_g33 = (float2(FlipBookColumns143_g33 , FlipBookRows144_g33));
			float temp_output_104_0_g33 = ( FlipBookColumns143_g33 * FlipBookRows144_g33 );
			float2 appendResult95_g33 = (float2(temp_output_104_0_g33 , FlipBookRows144_g33));
			float FlipBookSpeed145_g33 = ( _Time.y * 0.0 );
			float FlipBookID142_g33 = _FrameIndex;
			float clampResult108_g33 = clamp( FlipBookID142_g33 , 0.0001 , ( temp_output_104_0_g33 - 1.0 ) );
			float temp_output_92_0_g33 = frac( ( ( FlipBookSpeed145_g33 + clampResult108_g33 ) / temp_output_104_0_g33 ) );
			float2 appendResult93_g33 = (float2(temp_output_92_0_g33 , ( temp_output_92_0_g33 - -0.01 )));
			float2 break105_g33 = ( ( FlipBookUV141_g33 / appendResult83_g33 ) + ( floor( ( appendResult95_g33 * appendResult93_g33 ) ) / appendResult83_g33 ) );
			float2 appendResult139_g33 = (float2(break105_g33.x , ( 1.0 - break105_g33.y )));
			float4 tex2DNode30 = tex2D( _FaceSheet, appendResult139_g33 );
			float4 lerpResult250 = lerp( _Albedo , tex2DNode30 , tex2DNode30.a);
			float4 FlatColor61 = lerpResult250;
			float4 temp_output_35_0 = ( FlatColor61 * _ShadowTint );
			float4 lerpResult33 = lerp( FlatColor61 , temp_output_35_0 , _ShadowTintStrength);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float2 uv_Normals = i.uv_texcoord * _Normals_ST.xy + _Normals_ST.zw;
			float3 tex2DNode156 = UnpackNormal( tex2D( _Normals, uv_Normals ) );
			float3 normalizeResult17_g15 = ASESafeNormalize( tex2DNode156 );
			float3 break13_g15 = normalizeResult17_g15;
			float3 appendResult2_g15 = (float3(break13_g15.x , break13_g15.y , ( break13_g15.z / max( _NormalPower , 1E-06 ) )));
			float3 normalizeResult6_g15 = ASESafeNormalize( appendResult2_g15 );
			float3 temp_output_174_0 = normalizeResult6_g15;
			float3 normalizeResult3_g29 = ASESafeNormalize( normalize( (WorldNormalVector( i , temp_output_174_0 )) ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult8_g29 = dot( normalizeResult3_g29 , ase_worldlightDir );
			float grayscale12_g29 = Luminance(( ( ase_lightColor * (ase_lightAtten).xxxx ) * max( dotResult8_g29 , 0.0 ) ).rgb);
			float clampResult45_g29 = clamp( grayscale12_g29 , 0.0 , 1.0 );
			float clampResult82 = clamp( (0.0 + (clampResult45_g29 - _ShadowBoost) * (1.0 - 0.0) / (( _LightClamp + 1.0 ) - _ShadowBoost)) , 0.0 , 1.0 );
			float clampResult72 = clamp( (0.0 + (clampResult82 - _MinShadowValue) * (1.0 - 0.0) / (_MaxShadowValue - _MinShadowValue)) , 0.0 , 1.0 );
			float4 lerpResult36 = lerp( lerpResult33 , FlatColor61 , clampResult72);
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 appendResult17_g32 = (float3(ase_worldViewDir.x , 0.0 , ase_worldViewDir.z));
			float3 normalizeResult18_g32 = normalize( appendResult17_g32 );
			float3 newWorldNormal9_g32 = (WorldNormalVector( i , temp_output_174_0 ));
			float3 appendResult19_g32 = (float3(newWorldNormal9_g32.x , 0.0 , newWorldNormal9_g32.z));
			float3 normalizeResult10_g32 = normalize( appendResult19_g32 );
			float dotResult8_g32 = dot( normalizeResult18_g32 , normalizeResult10_g32 );
			float clampResult20_g32 = clamp( pow( max( dotResult8_g32 , 0.0 ) , ( _Shininess * 128.0 ) ) , 0.0 , _SpecularIntensity );
			float4 lerpResult101 = lerp( lerpResult36 , _LightColor , clampResult20_g32);
			float4 lerpResult58 = lerp( lerpResult101 , FlatColor61 , max( (0.0 + (clampResult82 - _MinLitValue) * (1.0 - 0.0) / (_MaxLitValue - _MinLitValue)) , 0.0 ));
			c.rgb = lerpResult58.rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.CommentaryNode;108;-3022.353,-283.4401;Inherit;False;1139.153;766.9884;Comment;8;30;84;61;250;251;253;254;257;Base Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;71;-704.9181,585.8954;Inherit;False;816.6672;436.7116;Comment;7;224;116;55;56;54;80;66;Lit Color Calc;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;65;-1760.324,-279.3956;Inherit;False;1635.569;502.6198;Comment;9;195;34;33;35;62;64;133;36;31;Unlit Color Calc;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;47;-1758.208,327.4812;Inherit;False;943.0303;333.4733;Comment;6;72;45;39;40;150;149;Shadow Region;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;48;-1218.195,1125.246;Inherit;False;1005.347;341.9011;Comment;4;50;74;51;49;Lit Region;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;31;-1724.724,-117.0601;Inherit;False;Property;_ShadowTint;Shadow Tint;12;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.1981131,0.1758725,0.1616678,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;72;-1058.896,376.289;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;66;-682.307,639.7967;Inherit;False;61;FlatColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;80;-447.7306,714.3457;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;54;-279.0484,644.1314;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-1170.078,1251.886;Inherit;False;Property;_MinLitValue;Min Lit Value;11;0;Create;True;0;0;0;False;0;False;0;0.708;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-680.9178,920.832;Inherit;False;Property;_DiffuseLightColorStrength;Diffuse Light Color Strength;9;0;Create;True;0;0;0;False;0;False;0;0.744;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;55;-683.2233,732.7641;Inherit;False;Property;_LightColor;Light Color;5;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0.9576786,0.8915094,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WireNode;116;-479.1422,819.4448;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;94;-3287.964,1430.584;Inherit;False;Property;_ShadowBoost;Shadow Boost;17;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;96;-2979.053,1550.542;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-3291.875,1545.626;Inherit;False;Property;_LightClamp;Light Clamp;16;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;36;-405.8456,-228.2469;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;133;-524.6801,117.1831;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;92;-2726.747,1182.014;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;82;-2455.374,1182.341;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;164;-3794.854,1373.012;Inherit;False;Property;_NormalPower;Normal Power;4;0;Create;True;0;0;0;False;0;False;0;0;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;174;-3421.854,1183.012;Inherit;False;CDC_NormalPower;-1;;15;a8f8e18f2afc2a34a80e603a8c15e09b;0;2;7;FLOAT3;0,0,0;False;10;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;62;-1685.292,-230.2576;Inherit;False;61;FlatColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;-1418.724,-134.0596;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;33;-1107.551,-223.5896;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;40;-1707.208,454.1213;Inherit;False;Property;_MinShadowValue;Min Shadow Value;15;0;Create;True;0;0;0;False;0;False;0;0.425;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-1708.208,535.1208;Inherit;False;Property;_MaxShadowValue;Max Shadow Value;14;0;Create;True;0;0;0;False;0;False;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;45;-1360.12,377.4813;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;150;-1728.032,420.9355;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;34;-1420.086,88.49373;Inherit;False;Property;_ShadowTintStrength;ShadowTintStrength;13;0;Create;True;0;0;0;False;0;False;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;195;-1109.717,-0.8984439;Inherit;False;AOTint;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;149;-1750.984,500.0871;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;117;221.7097,740.9579;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;101;1122.24,-252.2398;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;51;-808.5756,1173.805;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMaxOpNode;74;-485.5291,1173.615;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-1168.196,1332.884;Inherit;False;Property;_MaxLitValue;Max Lit Value;10;0;Create;True;0;0;0;False;0;False;0;0.754;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;219;540.4216,924.9684;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;220;-70.90347,1040.236;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;198;-2996.451,1066.85;Inherit;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;226;478.3921,1046.814;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;224;-101.1551,638.9587;Inherit;False;LitColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2793.16,-494.2621;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;PToonShader_Face;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;;0;False;;False;0;False;;0;False;;False;0;Custom;0.93;True;True;0;True;TransparentCutout;;Geometry;ForwardOnly;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;2;5;False;;10;False;;4;1;False;;1;False;;0;False;;0;False;;0;False;0;0.05555532,0,1,0;VertexOffset;False;False;Cylindrical;False;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.WireNode;227;1979.921,710.1989;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;235;-3031.19,1182.77;Inherit;True;CDC_DiffuseValue;-1;;29;32dbebd37f0b9b548b8b2e229c8ad01c;0;1;4;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;58;2169.407,-252.0099;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;156;-3813.291,1178.179;Inherit;True;Property;_Normals;Normals;3;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;249;-3532.94,900.4503;Inherit;True;PerturbNormal;-1;;30;c8b64dd82fb09f542943a895dffb6c06;1,26,1;1;6;FLOAT3;0,0,0;False;4;FLOAT3;9;FLOAT;28;FLOAT;29;FLOAT;30
Node;AmplifyShaderEditor.GetLocalVarNode;64;-736.5804,-54.29221;Inherit;False;61;FlatColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.WireNode;118;422.6008,389.2842;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;210;525.3478,389.5412;Inherit;True;CDC_CameraSpecularValue;6;;32;52df86ff9f0ffab418a306f878c698d9;0;1;11;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;225;1802.367,-159.6557;Inherit;False;61;FlatColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;250;-2263.115,-82.32129;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;61;-2090.2,-82.44;Inherit;False;FlatColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;251;-2876.115,-57.32129;Inherit;False;Flipbook;-1;;33;53c2488c220f6564ca6c90721ee16673;6,161,0,71,0,68,0,164,1,162,1,163,1;10;51;SAMPLER2D;0.0;False;167;SAMPLERSTATE;0;False;13;FLOAT2;0,0;False;24;FLOAT;0;False;4;FLOAT;3;False;5;FLOAT;3;False;130;FLOAT;0;False;2;FLOAT;0;False;55;FLOAT;0;False;70;FLOAT;0;False;5;COLOR;53;FLOAT2;0;FLOAT;47;FLOAT;48;FLOAT;62
Node;AmplifyShaderEditor.Vector2Node;253;-2997.115,344.6787;Inherit;False;Property;_RowsAndColums;Rows And Colums;18;0;Create;True;0;0;0;False;0;False;5,5;5,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;254;-2974.115,-202.3213;Inherit;False;Property;_FrameIndex;Frame Index;19;0;Create;True;0;0;0;False;0;False;0;0;0;15;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;84;-2525.783,-232.4514;Inherit;False;Property;_Albedo;Albedo;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.4467365,0.5754716,0.3881719,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;30;-2593.353,-57.54962;Inherit;True;Property;_FaceSheet;FaceSheet;1;0;Create;True;0;0;0;False;0;False;-1;None;4cc73acd0602e4445ae3324810328a5d;True;0;False;white;LockedToTexture2D;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;257;-2559.337,214.6527;Inherit;False;Constant;_Color0;Color 0;18;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
WireConnection;72;0;45;0
WireConnection;80;0;66;0
WireConnection;80;1;55;0
WireConnection;54;0;66;0
WireConnection;54;1;80;0
WireConnection;54;2;56;0
WireConnection;116;0;55;0
WireConnection;96;0;76;0
WireConnection;36;0;33;0
WireConnection;36;1;64;0
WireConnection;36;2;133;0
WireConnection;133;0;72;0
WireConnection;92;0;235;0
WireConnection;92;1;94;0
WireConnection;92;2;96;0
WireConnection;82;0;92;0
WireConnection;174;7;156;0
WireConnection;174;10;164;0
WireConnection;35;0;62;0
WireConnection;35;1;31;0
WireConnection;33;0;62;0
WireConnection;33;1;35;0
WireConnection;33;2;34;0
WireConnection;45;0;150;0
WireConnection;45;1;40;0
WireConnection;45;2;39;0
WireConnection;150;0;149;0
WireConnection;195;0;35;0
WireConnection;149;0;82;0
WireConnection;117;0;116;0
WireConnection;101;0;36;0
WireConnection;101;1;118;0
WireConnection;101;2;210;0
WireConnection;51;0;82;0
WireConnection;51;1;49;0
WireConnection;51;2;50;0
WireConnection;74;0;51;0
WireConnection;219;0;220;0
WireConnection;220;0;198;0
WireConnection;198;0;174;0
WireConnection;226;0;74;0
WireConnection;224;0;54;0
WireConnection;0;13;58;0
WireConnection;227;0;226;0
WireConnection;235;4;174;0
WireConnection;58;0;101;0
WireConnection;58;1;225;0
WireConnection;58;2;227;0
WireConnection;249;6;156;0
WireConnection;118;0;117;0
WireConnection;210;11;219;0
WireConnection;250;0;84;0
WireConnection;250;1;30;0
WireConnection;250;2;30;4
WireConnection;61;0;250;0
WireConnection;251;24;254;0
WireConnection;251;4;253;2
WireConnection;251;5;253;1
WireConnection;30;1;251;0
ASEEND*/
//CHKSM=0244D0B0CF6DC32735446105588155520DDE37F8