Shader "Fla/FillStyles/LinearGradient" {
	Properties {
		_Colors ("Colors", 2D) = "white" {} 
		_ColorWeight ("ColorWeight", 2D) = "white" {} 
		_GradientEntryCount("GradientEntryCount",Int) = 3
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

			sampler2D _Colors;
			sampler2D _ColorWeight;
			int _GradientEntryCount;
			
			fixed4 frag (fla_frag_data input) : SV_Target 
			{ 
				float radius = input.uv_0.y; 
				//radius = min(1,radius);
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
				return lerp(color1,color2,delta); 

			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
