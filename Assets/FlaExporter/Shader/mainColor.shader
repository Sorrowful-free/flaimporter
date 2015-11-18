Shader "Custom/main" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		 Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
									
			fixed4 _Color;
			
     
			  struct v2f {
				  float4 pos : SV_POSITION;
				  fixed4 color : COLOR;
			  };
      
			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.color.xyz = v.normal * 0.5 + 0.5;
				o.color.w = 1.0;
				return o;
			}

			fixed4 frag (v2f i) : SV_Target 
			{ 
				return _Color; 
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
