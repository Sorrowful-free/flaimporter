Shader "Fla/ColorTransform/ColorTransform" {
	Properties {
		[Enum(None,0,Brightness,1,Tine,2,Alpha,3)] _Mode ("Mode", Int) = 0
		
		_Color ("Color", Color) = (1,1,1,1)
		
		_Brightness ("Brightness", Range(-1,1)) = 0
		_Tine ("Tine", Range(0,1)) = 0		
		_Alpha ("Alpha", Range(0,1)) = 0
	}
	SubShader {
		Tags 
		{
			"IgnoreProjector"="True"
			"Queue"="Transparent" 
			"RenderType"="Transparent" 			
			"PreviewType"="Plane"
		}	
		ZTest Off
		Blend One OneMinusSrcAlpha
		//BlendOp LogicalClear

		GrabPass{"_OldColor"}		
		 			
		 Pass {
						
			CGPROGRAM			
			#include "../FlaCG.cginc"
			#pragma vertex fla_grab_vert_func 
			#pragma fragment frag 
			
			fixed4 _Color;
			int _Mode;
			float _Tine;
			float _Brightness;
			float _Alpha;			
			sampler2D _OldColor;
			
			fixed4 frag (fla_frag_data input) : SV_Target 
			{ 
				fixed4 resultColor = tex2D(_OldColor,input.uv_0) ;
				float alpha = resultColor.a;
				if(_Mode == 1)
				{
					if(_Brightness >0)
						resultColor.rgb = resultColor.rgb + (1-resultColor.rgb) * _Brightness;
					else
						resultColor.rgb = resultColor.rgb + resultColor.rgb * _Brightness;
				}else if(_Mode == 2)
				{
					resultColor.rgb = resultColor.rgb - (resultColor.rgb - _Color.rgb) * _Tine;
				}else if(_Mode == 3)
				{
					resultColor *= _Alpha;
				}

				resultColor *= alpha;
				return resultColor ; 
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
