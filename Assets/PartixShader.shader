Shader "Custom/NewSurfaceShader" {
	SubShader{
		Pass {
			Tags { "RenderType" = "Opaque"}
			CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
        #include "UnityCG.cginc"
			struct v2f {
				float4 pos : SV_POSITION;
				float4 color : COLOR;
			};
			uniform float4x4 _Deform;
			v2f vert(appdata_full v) {
				v2f o;
				o.pos = mul(_Deform, v.vertex);
				o.pos = UnityObjectToClipPos(o.pos);
				o.color = v.color;
				return o;
			}
			fixed4 frag(v2f i) : SV_Target{ return i.color; }
			ENDCG
		}
	}
	Fallback "Diffuse"
}
