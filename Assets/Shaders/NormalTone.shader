Shader "Custom/NormalTone" {
    Properties {
        _MainTex ("MainTex", 2D) = "" {}
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
				return c;
			}

			ENDCG
		}
	}
}
