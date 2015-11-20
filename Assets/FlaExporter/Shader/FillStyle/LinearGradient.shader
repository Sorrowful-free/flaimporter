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
						if(weigth.r >= input.uv_0.y)
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

				float delta = (input.uv_0.y - weight1.r)/(weight2.r-weight1.r);
				return lerp(color1,color2,delta);

			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
