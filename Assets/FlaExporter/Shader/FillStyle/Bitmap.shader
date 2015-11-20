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
			"RenderType"="Transparent"  
			"PreviewType"="Plane"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

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
				return color;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
