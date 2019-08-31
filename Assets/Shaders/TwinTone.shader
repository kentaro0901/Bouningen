Shader "Custom/TwinTone" {
    Properties {
        _MainTex ("MainTex", 2D) = "" {}
		_LineColor("LineColor", Color) = (1,1,1,1)
		_BackColor("BackColor", Color) = (1,1,1,1)
    }
	SubShader{
		Tags { "Queue" = "Transparent" }
		Pass {
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert_img alpha:fade
			#pragma fragment frag

			sampler2D _MainTex;
			fixed4 _LineColor;
			fixed4 _BackColor;

			fixed4 frag(v2f_img i) : COLOR {
				fixed4 c = tex2D(_MainTex, i.uv);
				c.rgb = (c.r * 0.3 + c.g * 0.6 + c.b * 0.1 < 0.5) ? _LineColor.rgb : _BackColor.rgb;
				return c;
			}

			ENDCG
		}
	}
}
