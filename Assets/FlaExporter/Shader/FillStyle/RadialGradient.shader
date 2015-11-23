Shader "Fla/FillStyles/RadialGradient" {
	Properties {
		_Colors ("Colors", 2D) = "white" {} 
		_ColorWeight ("ColorWeight", 2D) = "white" {} 
		_GradientEntryCount("GradientEntryCount",Int) = 3
		_range("GradientEntryCountasd",Range(-1,1)) = 0
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
			float _range;
			
			fixed4 frag (fla_frag_data input) : SV_Target 
			{ 
				float radius = length(input.uv_0 - float2(0.5,0.5))/0.5; 
				int index1 = 0;
				int index2 = 0;
				
				bool cheked = false; 
				float weight1 = 0;
				float weight2 = 0;

				float4 color1 = 0;
				float4 color2 = 0;

				int index = 0; 
				for(index = 0; index <= _GradientEntryCount; index += 1) 
				{
					if(!cheked)
					{
						index1 = index;// max(0,index-1);	
						index2 = index1+1;   
						
						if(radius >= 1)
						{
						//	index1 = _GradientEntryCount-1;	  
						//	index2 = index1+1;   

							weight1 = extract_value_from_sampler2D(_ColorWeight,index1,_GradientEntryCount).r;
							weight2 = extract_value_from_sampler2D(_ColorWeight,index2,_GradientEntryCount).r;
							color1 = extract_value_from_sampler2D(_Colors,index1,_GradientEntryCount);
							color2 = extract_value_from_sampler2D(_Colors,index2,_GradientEntryCount);
							cheked = true;     
						}  
											
						if(!cheked)  
						{  
							weight1 = extract_value_from_sampler2D(_ColorWeight,index1,_GradientEntryCount).r;
							weight2 = extract_value_from_sampler2D(_ColorWeight,index2,_GradientEntryCount).r; 
							if(weight2 > radius && weight1 < radius) 
							{
								cheked = true;		
								color1 = extract_value_from_sampler2D(_Colors,index1,_GradientEntryCount);
								color2 = extract_value_from_sampler2D(_Colors,index2,_GradientEntryCount);						
							}		
							if(!cheked)									
							{
							//	index1 = _GradientEntryCount-1;	   
							//	index2 = index1+1; 
							//	color1 = extract_value_from_sampler2D(_Colors,index1,_GradientEntryCount);
							//	color2 = extract_value_from_sampler2D(_Colors,index2,_GradientEntryCount);									
								//cheked = true;     
							}
						}					
					}
				}     
				
			//	weight1 = extract_value_from_sampler2D(_ColorWeight,index1,_GradientEntryCount).r; 
				
				
			//	if(input.uv_0.x<0.5)
			//		return 	(weight2-weight1);
				//return 	(radius - weight1); 
								
				float delta = (radius - weight1)/(weight2-weight1); //todo fix this shader 
				delta = max(0,min(1,delta));
			//	return delta;
			//	return lerp(weight1,weight2,delta);
			//	return delta;
				//return delta;
						
				return lerp(color1,color2,delta);

			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
