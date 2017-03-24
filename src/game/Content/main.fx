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
	half4 Position : POSITION0;
	half3 Normal : NORMAL0;
	half2 TextureCoodinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	half4 Position : SV_POSITION;
	half3 Normal : NORMAL0;
	half2 TextureCoodinates : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, worldViewProjection);
	output.TextureCoodinates = input.TextureCoodinates;
	output.Normal = mul(input.Normal, (half3x4)worldViewProjectionTransposed).xyz;

	return output;
}

half4 MainPS(VertexShaderOutput input) : COLOR
{
	half shade = dot(normalize(input.Normal), half3(0, 1, 0)) * 0.5 + 0.5;
	half3 lighting = lerp(half3(.1, .1, .7), half3(1, .85, 0), 1 - pow(max(0, 1 - shade), .25));
	half3 diffuse = tex2D(textureSampler, input.TextureCoodinates).xyz;
	half3 color = lighting * diffuse;
	return shade > .9 ? half4(1000, 1000, 1000, 1) : half4(color, 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};