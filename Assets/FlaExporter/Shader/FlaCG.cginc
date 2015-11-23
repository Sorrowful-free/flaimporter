#ifndef FLA_CG_INCLUDES
#define FLA_CG_INCLUDES

#include "UnityCG.cginc"

uniform float _ShapeTweenDeltaKoef;


uniform float _TextureRotation;
uniform float2 _TexturePosition;
uniform float2 _TextureScale;
uniform float2 _TextureTransformPosition;

struct fla_vert_data
{
	float4 position				:POSITION;
	float2 uv_0 				:TEXCOORD0;
	float2 shape_tween_delta 	:TEXCOORD1; 
};

struct fla_frag_data
{
	float4 position		:SV_POSITION;
	float2 uv_0 		:TEXCOORD0; 
};

float4 extract_value_from_sampler2D(sampler2D tex2d,int index,int count)
{
	float floatIndex = index;
	float floatCount = count;
	float offset = 1/floatCount/2;
	
	if(floatIndex >= floatCount-1)
	{
		return tex2D(tex2d,float2(1.0-offset,0.5));
	}

	if(floatIndex < 0)
	{
		return tex2D(tex2d,float2(offset,0.5));
	}
	
	float position = floatIndex/floatCount + offset;
	float4 color = tex2D(tex2d,float2(position,0.5));
	return color;
}

float4 get_screen_uv(float4 position)
{
	float4 screenPosition = position; 
	screenPosition.xy = 0.5*(screenPosition.xy+1.0);
	screenPosition.y = 1-screenPosition.y;
	return screenPosition;
}

float4 get_vertex_position(float4 position, float2 delta)
{
	float2 deltaPosition = delta*_ShapeTweenDeltaKoef;
	float4 resultPosition = position;
	resultPosition.xy += deltaPosition.xy; 
	return resultPosition;
}

float2 get_transform_uv(float2 uv)//todo need write
{
	return uv;
}

fla_frag_data fla_vert_func(fla_vert_data input)
{
	fla_frag_data output;
	float4 position = get_vertex_position(input.position,input.shape_tween_delta);
	output.position = mul (UNITY_MATRIX_MVP, position);
	output.uv_0 = get_transform_uv(input.uv_0);
	return output;
}

fla_frag_data fla_grab_vert_func(fla_vert_data input)
{
	fla_frag_data output;
	float4 position = get_vertex_position(input.position,input.shape_tween_delta);
	output.position = mul (UNITY_MATRIX_MVP, position);
	output.uv_0 = get_screen_uv(output.position);	
	return output;
}


#endif