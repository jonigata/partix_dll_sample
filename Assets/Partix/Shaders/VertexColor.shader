Shader "Partix/VertexColorShader" {
	SubShader{
		Pass {
			Tags { "RenderType" = "Opaque"}
			CGPROGRAM

                        #pragma vertex vert
                        #pragma fragment frag
                        #pragma multi_compile EDITOR_MODE PLAY_MODE

                        #include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};
			uniform float4x4 _Deform;
			v2f vert(appdata_full v) {
				v2f o;
                                o.pos = v.vertex;
                        #ifdef EDITOR_MODE
                                o.pos = mul(UNITY_MATRIX_MVP, o.pos);
                        #elif PLAY_MODE
				o.pos = mul(_Deform, o.pos);
				o.pos = mul(UNITY_MATRIX_VP, o.pos);
                        #endif
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target{ return i.color; }
			ENDCG
		}
		Pass {
			Tags { "LightMode" = "ShadowCaster"}
			CGPROGRAM

                        #pragma vertex vert
                        #pragma fragment frag
                        #pragma multi_compile EDITOR_MODE PLAY_MODE
                        #pragma multi_compile_shadowcaster

                        #include "UnityCG.cginc"

			struct v2f {
                            V2F_SHADOW_CASTER;
			};
			uniform float4x4 _ShadowDeform;
			v2f vert(appdata_base v) {
				v2f o;
                                o.pos = v.vertex;
                        #ifdef EDITOR_MODE
                                o.pos = mul(UNITY_MATRIX_MVP, o.pos);
                        #elif PLAY_MODE
				o.pos = mul(_ShadowDeform, o.pos);
				o.pos = mul(UNITY_MATRIX_VP, o.pos);
                        #endif
                                o.pos = UnityApplyLinearShadowBias(o.pos);
				return o;
			}
			fixed4 frag(v2f i) : SV_Target{
                             SHADOW_CASTER_FRAGMENT(i)
                        }
			ENDCG
		}
	}
	Fallback "Diffuse"
}
