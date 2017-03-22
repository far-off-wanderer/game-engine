#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix worldViewProjection;
matrix worldViewProjectionTransposed;


sampler2D textureSampler = sampler_state
{
	Texture = (modelTexture);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TextureCoodinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL0;
	float2 TextureCoodinates : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, worldViewProjection);
	output.TextureCoodinates = input.TextureCoodinates;
	output.Normal = mul(input.Normal, (float3x4)worldViewProjectionTransposed).xyz;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float shade = dot(normalize(input.Normal), float3(0, 1, 0)) * 0.5 + 0.5;
	float3 lighting = lerp(float3(.1, .1, .7), float3(1, .85, 0), 1 - pow(max(0, 1 - shade), .25));
	float3 diffuse = tex2D(textureSampler, input.TextureCoodinates).xyz;
	float3 color = lighting * diffuse;
	return float4(color, 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};