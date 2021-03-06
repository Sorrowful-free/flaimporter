﻿Shader "Fla/FillStyles/RadialGradient" {
	Properties {
		_Colors ("Colors", 2D) = "white" {} 
		_ColorWeight ("ColorWeight", 2D) = "white" {} 
		_GradientEntryCount("GradientEntryCount",Int) = 3
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
			"Queue"="Transparent"			
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
		}

		Cull Off
		Lighting Off  
		ZWrite Off 
		ZTest LEqual 
		Blend One OneMinusSrcAlpha 
		ColorMask [_ColorMask]


		LOD 200  

		 Pass {
			CGPROGRAM	 		
			#include "../FlaCG.cginc"
			#pragma vertex fla_vert_func 
			#pragma fragment frag 

			sampler2D _Colors;
			sampler2D _ColorWeight;
			int _GradientEntryCount; 
			
			
			
			fixed4 frag (fla_frag_data input) : SV_Target 
			{ 
				float radius = length(input.uv_0 - float2(0.5,0.5))/0.5; 
				radius = min(1,radius);
				bool checked = false; 
				int index1 = 0;
				int index2 = 0;
								
				float weight1 = 0;
				float weight2 = 0;

				float4 color1 = 0;
				float4 color2 = 0;

				int index = 0; 

				for(index = 0; index <= _GradientEntryCount; index += 1) 
				{
					if(!checked)
					{
						index1 = index;
						index2 = min(index1+1,_GradientEntryCount-1);	
					
						weight2 = extract_value_from_sampler2D(_ColorWeight,index2,_GradientEntryCount).r;
						if(weight2 >= radius)
						{
							weight1 = extract_value_from_sampler2D(_ColorWeight,index1,_GradientEntryCount).r;
							color1 = extract_value_from_sampler2D(_Colors,index1,_GradientEntryCount);
							color2 = extract_value_from_sampler2D(_Colors,index2,_GradientEntryCount);
							checked = true;
						}						
					}					
				}     
				if(!checked)
				{
					color1 = extract_value_from_sampler2D(_Colors,_GradientEntryCount,_GradientEntryCount);
					color2 = extract_value_from_sampler2D(_Colors,_GradientEntryCount,_GradientEntryCount);
				}
		
				float delta = (radius - weight1)/(weight2-weight1); 
				delta = max(0,min(1,delta));
				fixed4 color = lerp(color1,color2,delta); 
				fixed4 resultColor = apply_color_transform(color); 
				resultColor.a *= color.a;
				return resultColor;

			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
