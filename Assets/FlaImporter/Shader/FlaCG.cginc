#ifndef FLA_CG_INCLUDES
// Upgrade NOTE: excluded shader from DX11, Xbox360, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 xbox360 gles
// Upgrade NOTE: excluded shader from DX11 and Xbox360 because it uses wrong array syntax (type[size] name)
#pragma exclude_renderers d3d11 xbox360
#define FLA_CG_INCLUDES

#include "UnityCG.cginc"

#pragma multi_compile FLA_MASKED FLA_MASK FLA_IS_CLIPED
//#pragma FLA_IS_CLIPED

float _ShapeTweenDeltaKoef;
const float PI = 3.14159;

float4 _ColorMultipler = float4(1,0,0,1);
float4 _ColorOffset = float4(1,0,0,1);

float _TextureAspect = 1;
int _TextureIsCliped = 0;
int _MaskType = 0; //0 - masked, 1-mask

float4 _TextureMatrixABCD = float4(1,0,0,1);
float2 _TextureMatrixTXTY = float2(0,0); 

float2 _Skew = float2(0,0); 




struct fla_vert_data
{
	float4 position				:POSITION;
	float2 uv_0 				:TEXCOORD0;
	float2 uv_1 				:TEXCOORD1;
	float2 shape_tween_delta 	:TEXCOORD3; 
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

float2 get_transform_uv(float2 uv)
{
	float2 uvTemp = uv - float2(0.5,0.5);
	float2 uvResult = uv;

	uvResult.x = uvTemp.x*_TextureMatrixABCD.x + uvTemp.y*_TextureMatrixABCD.z + _TextureMatrixTXTY.x/100; 
	uvResult.y = uvTemp.x*_TextureMatrixABCD.y + uvTemp.y*_TextureMatrixABCD.w + _TextureMatrixTXTY.y/100;

	uvResult.y *= _TextureAspect;  
	uvResult += float2(0.5,0.5);
	
	return uvResult; 
}

fixed4 apply_color_transform(fixed4 color)
{
	fixed4 result = (color * _ColorMultipler)+_ColorOffset;
	if(result.a <0.01 && _MaskType>0)
	{
		discard;
	}
	return result;
}

float2 applySkew(float2 vertex)
{
	float2 v = vertex;
	float _SkewX = _Skew.x/180*PI;
	float _SkewY = _Skew.y/180*PI;
	return float2(v.x + v.y*tan(_SkewX), v.y  +v.x*-tan(_SkewY));
}

fla_frag_data fla_vert_func(fla_vert_data input)
{
	fla_frag_data output;
	float4 position = get_vertex_position(input.position,input.shape_tween_delta);
	position.xy = applySkew(position.xy);
	output.position = mul (UNITY_MATRIX_MVP, position);
	if(_TextureIsCliped > 0)
		output.uv_0 = input.uv_1;
	else
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