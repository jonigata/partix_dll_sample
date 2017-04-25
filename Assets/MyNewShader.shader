// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MyNewShader"
{
	Properties
	{
		_TessValue( "Tessellation", Range( 1, 32 ) ) = 4
		_TessMin( "Tess Min Distance", Float ) = 10
		_TessMax( "Tess Max Distance", Float ) = 25
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Lambert keepalpha  noshadow vertex:vertexDataFunc tessellate:tessFunction nolightmap 
		struct Input
		{
			fixed filler;
		};

		struct appdata
		{
			float4 vertex : POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
			float4 texcoord1 : TEXCOORD1;
			float4 texcoord2 : TEXCOORD2;
			float4 texcoord3 : TEXCOORD3;
			fixed4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		uniform float4x4 _Deform;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;

		float4 tessFunction( appdata v0, appdata v1, appdata v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata v )
		{
			v.vertex.xyz += ( -v.vertex + mul( _Deform , float4( v.vertex , 0.0 ) ) );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=6001
3003;370;1066;939;365.9541;77.16686;1.3;True;True
Node;AmplifyShaderEditor.Matrix4X4Node;5;-65.29994,515.4001;Float;False;Property;_Deform;Deform;0;1;[HideInInspector];1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;FLOAT4x4
Node;AmplifyShaderEditor.PosVertexDataNode;2;-279.5,383.4999;Float;False;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.PosVertexDataNode;6;-241.5,667.4999;Float;False;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;265.5,614.4999;Float;False;0;FLOAT4x4;0.0;False;1;FLOAT3;0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0;False;FLOAT3
Node;AmplifyShaderEditor.NegateNode;3;77.5,408.4999;Float;False;0;FLOAT3;0.0;False;FLOAT3
Node;AmplifyShaderEditor.SimpleAddOpNode;4;419.5,338.4999;Float;False;0;FLOAT3;0.0;False;1;FLOAT3;0,0,0;False;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;732.4009,190.7;Float;False;True;6;Float;ASEMaterialInspector;Lambert;MyNewShader;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;False;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;True;0;4;10;25;False;0.5;False;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;13;OBJECT;0.0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False
WireConnection;7;0;5;0
WireConnection;7;1;6;0
WireConnection;3;0;2;0
WireConnection;4;0;3;0
WireConnection;4;1;7;0
WireConnection;0;11;4;0
ASEEND*/
//CHKSM=BBF5839638F385A49DACEEA26DAE0E348772E3C8