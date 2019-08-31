Shader "Custom/ReverseTone" {
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

			fixed4 frag(v2f_img i) : COLOR {
				fixed4 c = tex2D(_MainTex, i.uv);
				c.rgb = (1-c.r, 1-c.g, 1-c.b);
				return c;
			}

			ENDCG
		}
	}
}
