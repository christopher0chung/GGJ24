// Made with Amplify Shader Editor v1.9.2.2
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "FaceShader"
{
	Properties
	{
		_Albedo("Albedo", Color) = (1,1,1,1)
		_FaceSheet("FaceSheet", 2D) = "white" {}
		_Rows("Rows", Int) = 1
		_Columns("Columns", Int) = 1
		_Frame("Frame", Range( 0 , 24)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform float4 _Albedo;
		uniform sampler2D _FaceSheet;
		uniform int _Columns;
		uniform int _Rows;
		uniform float _Frame;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 break135_g1 = i.uv_texcoord;
			float2 appendResult136_g1 = (float2(break135_g1.x , ( 1.0 - break135_g1.y )));
			float2 FlipBookUV141_g1 = appendResult136_g1;
			float FlipBookColumns143_g1 = (float)_Columns;
			float FlipBookRows144_g1 = (float)_Rows;
			float2 appendResult83_g1 = (float2(FlipBookColumns143_g1 , FlipBookRows144_g1));
			float temp_output_104_0_g1 = ( FlipBookColumns143_g1 * FlipBookRows144_g1 );
			float2 appendResult95_g1 = (float2(temp_output_104_0_g1 , FlipBookRows144_g1));
			float FlipBookSpeed145_g1 = ( _Time.y * 0.0 );
			float FlipBookID142_g1 = _Frame;
			float clampResult108_g1 = clamp( FlipBookID142_g1 , 0.0001 , ( temp_output_104_0_g1 - 1.0 ) );
			float temp_output_92_0_g1 = frac( ( ( FlipBookSpeed145_g1 + clampResult108_g1 ) / temp_output_104_0_g1 ) );
			float2 appendResult93_g1 = (float2(temp_output_92_0_g1 , ( temp_output_92_0_g1 - -0.01 )));
			float2 break105_g1 = ( ( FlipBookUV141_g1 / appendResult83_g1 ) + ( floor( ( appendResult95_g1 * appendResult93_g1 ) ) / appendResult83_g1 ) );
			float2 appendResult139_g1 = (float2(break105_g1.x , ( 1.0 - break105_g1.y )));
			float4 tex2DNode3 = tex2D( _FaceSheet, appendResult139_g1 );
			float4 lerpResult2 = lerp( _Albedo , tex2DNode3 , tex2DNode3.a);
			o.Emission = lerpResult2.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=19202
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;FaceShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;;0;False;;False;0;False;;0;False;;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;12;all;True;True;True;True;0;False;;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;False;2;15;10;25;False;0.5;True;0;0;False;;0;False;;0;0;False;;0;False;;0;False;;0;False;;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;;-1;0;False;;0;0;0;False;0.1;False;;0;False;;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;16;FLOAT4;0,0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.LerpOp;2;-224,138.5;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;1;-699,-22.5;Inherit;False;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,0.764706,0.1450978,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1375,56.5;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-768,175.5;Inherit;True;Property;_FaceSheet;FaceSheet;1;0;Create;True;0;0;0;False;0;False;-1;None;413b59d13ae77314c83ec83a36f76189;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.IntNode;8;-1373,244.5;Inherit;False;Property;_Columns;Columns;3;0;Create;True;0;0;0;False;0;False;1;5;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;7;-1373,175.5;Inherit;False;Property;_Rows;Rows;2;0;Create;True;0;0;0;False;0;False;1;5;False;0;1;INT;0
Node;AmplifyShaderEditor.FunctionNode;5;-1079,55.5;Inherit;False;Flipbook;-1;;1;53c2488c220f6564ca6c90721ee16673;6,161,0,71,0,68,0,164,1,162,1,163,1;10;51;SAMPLER2D;0.0;False;167;SAMPLERSTATE;0;False;13;FLOAT2;0,0;False;24;FLOAT;0;False;4;FLOAT;3;False;5;FLOAT;3;False;130;FLOAT;0;False;2;FLOAT;0;False;55;FLOAT;0;False;70;FLOAT;0;False;5;COLOR;53;FLOAT2;0;FLOAT;47;FLOAT;48;FLOAT;62
Node;AmplifyShaderEditor.RangedFloatNode;9;-1501,315.5;Inherit;False;Property;_Frame;Frame;4;0;Create;True;0;0;0;False;0;False;0;0;0;24;0;1;FLOAT;0
WireConnection;0;2;2;0
WireConnection;2;0;1;0
WireConnection;2;1;3;0
WireConnection;2;2;3;4
WireConnection;3;1;5;0
WireConnection;5;13;4;0
WireConnection;5;24;9;0
WireConnection;5;4;8;0
WireConnection;5;5;7;0
ASEEND*/
//CHKSM=5CE66A48857DD3FEC1E162D25A2E1B4D045DEB24