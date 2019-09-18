Shader "Custom/SpriteTwinTone" {
	Properties{
		_MainTex("Texture", 2D) = "white"{}
		_LineColor("LineColor", Color) = (1,1,1,1)
		_BackColor("BackColor", Color) = (1,1,1,1)
	}
		SubShader{
			Tags{ "RenderType" = "Fade" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard alpha:fade
			#pragma target 3.0

			struct Input {
				float2 uv_MainTex; //座標
			};

			sampler2D _MainTex; //画像
			fixed4 _LineColor;
			fixed4 _BackColor;

			void surf(Input IN, inout SurfaceOutputStandard o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
				o.Albedo = c.rgb = (c.r * 0.3 + c.g * 0.6 + c.b * 0.1 < 0.5) ? _LineColor.rgb : _BackColor.rgb;
				o.Alpha = c.a;
			}
			ENDCG
	}
		FallBack "Diffuse"
}

