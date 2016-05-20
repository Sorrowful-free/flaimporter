Shader "Fla/Test/RasterShader" 
{
	Properties 
	{
		_Bitmap ("Bitmap", 2D) = "white" {} 
		_Relef ("Relef", 2D) = "white" {}
		_StencilId("StencilId",Int) = 0
		_StencilOp("StencilOp",Int) = 0
		_StencilComp("StencilComp",Int) = 0
		_ColorMask("ColorMask",Int) = 0
		_Tile("Tile",float) = 0
		_RelefCount("RelefCount",Int) = 1 
		_RelefSize("RelefSize",float) = 0
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
		Blend SrcAlpha OneMinusSrcAlpha   
		ColorMask [_ColorMask]

		LOD 200
		 
		 Pass 
		 { 
			CGPROGRAM 
			#include "../FlaCG.cginc"  
			#pragma vertex vert_func      
			#pragma fragment frag 
						    
			sampler2D _Bitmap;       
			sampler2D _Relef;      
			float _Tile; 
			int _RelefCount; 
			float _RelefSize;

			fla_frag_data vert_func(fla_vert_data input)
			{
				fla_frag_data output;
				output.position = mul (UNITY_MATRIX_MVP, input.position);
				output.uv_0 = (input.uv_0); 	
				return output;
			}

				
			fixed4 frag (fla_frag_data input) : SV_Target 
			{ 
				
				//fixed4 resultColor = apply_color_transform(color); 			 
			//	resultColor.a *= color.a;
				fixed4 color = tex2D(_Bitmap,input.uv_0); 
				fixed4 relef =  tex2D(_Bitmap,input.uv_0*_Tile);
				color += (tex2D(_Bitmap,input.uv_0+(relef- float4(0.5,0.5,0,0))*_RelefSize))*0.6; 

				return color; 
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
