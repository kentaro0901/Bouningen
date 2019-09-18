Shader "Custom/SpriteTwinTone" {
	Properties {
		_MainTex("Texture", 2D) = "white" {}
		_LineColor("LineColor", Color) = (1,1,1,1)
		_BackColor("BackColor", Color) = (1,1,1,1)
	}
	SubShader {
		Tags {"Queue" = "Transparent" "RenderType" = "Transparent"}
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass	{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _LineColor;
			fixed4 _BackColor;

			fixed4 frag(v2f_img i) : SV_Target {
				fixed4 c = tex2D(_MainTex, i.uv);
				c.rgb = (c.r * 0.3 + c.g * 0.6 + c.b * 0.1 < 0.5) ? _LineColor.rgb : _BackColor.rgb;
				return c;
			}
			ENDCG
		}
	}
}
