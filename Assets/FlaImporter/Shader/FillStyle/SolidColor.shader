﻿Shader "Fla/FillStyles/SolidColor"
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)		
		_StencilId("StencilId",Int) = 0
		_StencilOp("StencilOp",Int) = 0
		_StencilComp("StencilComp",Int) = 0
		_ColorMask("ColorMask",Int) = 0  
	}
	  
	SubShader 
	{
		Stencil
		{
			Ref [_StencilId]
			Pass [_StencilOp]  
			Comp [_StencilComp]  
		}
		Tags 
		{
			"Queue" = "Transparent"			
			"IgnoreProjector"="True"  
			"RenderType"="Transparent" 
		} 

		Cull Off 
		Lighting Off 
		ZWrite Off
		ZTest LEqual  
		Blend SrcAlpha OneMinusSrcAlpha    
		ColorMask [_ColorMask]
		        
		LOD 200

		 Pass 
		 {
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
