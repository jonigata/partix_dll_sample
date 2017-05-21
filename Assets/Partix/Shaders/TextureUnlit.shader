Shader "Partix/TextureUnlit"
{
        Properties  {
            _MainTex("Base (RGB)",2D) = "white" {}
        }
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
                        Cull Off

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
                        #pragma multi_compile EDITOR_MODE PLAY_MODE
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

                        sampler2D _MainTex;
                        float4 _MainTex_ST;
			uniform float4x4 _Deform;
			
			v2f vert (appdata v)
			{
				v2f o;
                                o.vertex = v.vertex;
                        #ifdef EDITOR_MODE
                                o.vertex = mul(UNITY_MATRIX_MVP, o.vertex);
                        #elif PLAY_MODE
				o.vertex = mul(_Deform, v.vertex);
				o.vertex = mul(UNITY_MATRIX_VP, o.vertex);
                        #endif
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                                fixed4 col = tex2D(_MainTex, i.uv);
                                return col;
			}
			ENDCG
		}
	}
}