Shader "Fla/FillStyles/Bitmap" 
{
	Properties 
	{
		_Bitmap ("Bitmap", 2D) = "white" {} 
	}

	SubShader 
	{
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
		 
		 Pass 
		 {
			CGPROGRAM 
			#include "../FlaCG.cginc"  
			#pragma vertex fla_vert_func      
			#pragma fragment frag 
			    
			sampler2D _Bitmap;       
				
			fixed4 frag (fla_frag_data input) : SV_Target 
			{ 
				fixed4 color = tex2D(_Bitmap,input.uv_0); 
				color.rgb *=color.a;
				color = apply_color_transform(color); 			
				return color; 
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
