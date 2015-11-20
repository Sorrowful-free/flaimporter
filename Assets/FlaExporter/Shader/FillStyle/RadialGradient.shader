Shader "Fla/FillStyles/RadialGradient" {
	Properties {
		_Colors ("Colors", 2D) = "white" {} 
		_ColorWeight ("ColorWeight", 2D) = "white" {} 
		_GradientEntryCount("GradientEntryCount",Int) = 3
		_range("GradientEntryCount",Range(-1,1)) = 0
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
				float pos = length(input.uv_0 - float2(0.5,0.5))/0.5; 
				pos = max(0,min(1,pos)); 
				int index1 =0;
				int index2 =0;
				float4 weigth ;
				int index = 0;
				bool cheked = false; 
				for(index = 0; index < _GradientEntryCount; index +=1) 
				{
					if(!cheked)
					{
						weigth = extract_value_from_sampler2D(_ColorWeight,index,_GradientEntryCount);
						if(weigth.r >= pos)
						{
							index1 = max(0,index-1); 
							index2 = index1+1;	
							cheked = true;		 			
						}
					}					
				}

				
				fixed4 weight1 = extract_value_from_sampler2D(_ColorWeight,index1,_GradientEntryCount);
				fixed4 weight2 = extract_value_from_sampler2D(_ColorWeight,index2,_GradientEntryCount);
				fixed4 color1 = extract_value_from_sampler2D(_Colors,index1,_GradientEntryCount);
				fixed4 color2 = extract_value_from_sampler2D(_Colors,index2,_GradientEntryCount);
				
				
				float delta = (pos - weight1.r)/(weight2.r-weight1.r); //todo fix this shader
				return fixed4(weight1.r,0,0,1);
				//delta = max(0,min(1,delta));
				//return delta
				if(!cheked)
				{
					return 0;
				}
				
				//float delta2 = 1-abs(delta-0.5)/0.5;
				if(delta > 1)
				{
					return fixed4(1,0,0,1);  
				}
				if(delta < 0)
				{
					return fixed4(0,1,0,1); 
				}
				//return delta;
				//return color1 + (color2-color1)*delta*(1+_range);
				return lerp(color1,color2,delta*(1+_range));

			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
