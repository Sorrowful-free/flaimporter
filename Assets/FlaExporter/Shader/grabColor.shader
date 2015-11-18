Shader "Custom/grab" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)		
	}
	SubShader {
	//	Tags { "RenderType"="Opaque" }

		GrabPass{ "_OldColor" }
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
									
			fixed4 _Color;
			sampler2D _OldColor;
     
			struct appdata {
				float4 vertex : POSITION;
				float4 uv : TEXCOORD0;
			};
			  
			struct fragData
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};
      
			fragData vert (appdata v)
			{
				fragData o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				
				float4 p = o.pos;
				p.xy = 0.5*(p.xy+1.0);
				p.y = 1-p.y;
				o.uv = p;
				return o;
			}

			fixed4 frag (fragData i) : SV_Target 
			{ 
				return tex2D (_OldColor,i.uv) + _Color; 
			}
		ENDCG
	} 
	}
	FallBack "Diffuse"
}
