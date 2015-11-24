Shader "Fla/FillStyles/SolidColor" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)		
	}
	SubShader {
		Tags 
		{
			"Queue"="Transparent" 						
			"RenderType"="Transparent" 			
			"PreviewType"="Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off 
		Blend One OneMinusSrcAlpha 
		 
		LOD 200

		 Pass {
			CGPROGRAM			
			#include "../FlaCG.cginc"
			#pragma vertex fla_vert_func 
			#pragma fragment frag 

			fixed4 _Color;
			
			fixed4 frag (fla_frag_data input) : SV_Target 
			{ 
				return _Color*_Color.a; 
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
