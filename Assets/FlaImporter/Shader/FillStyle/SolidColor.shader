Shader "Fla/FillStyles/SolidColor" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)		
	}
	SubShader {
		Tags 
		{
			"Queue"="Transparent"			
			"IgnoreProjector"="True" 
			"RenderType"="Transparent"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest LEqual  
		Blend SrcAlpha OneMinusSrcAlpha  
		 
		LOD 200

		 Pass {
			CGPROGRAM			
			#include "../FlaCG.cginc" 
			#pragma vertex fla_vert_func 
			#pragma fragment frag 

			fixed4 _Color;
			
			fixed4 frag (fla_frag_data input) : SV_Target 
			{ 
				fixed4 color = _Color*_Color.a; 
				fixed4 resultColor = apply_color_transform(color); 
				resultColor.a *= color.a;
				return resultColor;				
			}
			ENDCG 
		}
	} 
	FallBack "Diffuse"
}
