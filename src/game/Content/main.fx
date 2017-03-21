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
float brightness;

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
	output.Normal = mul(input.Normal, (float3x4)worldViewProjectionTransposed);

	return output;
}

float Epsilon = 1e-10;
 
float3 RGBtoHCV(float3 RGB)
{
  // Based on work by Sam Hocevar and Emil Persson
	float4 P = (RGB.g < RGB.b) ? float4(RGB.bg, -1.0, 2.0 / 3.0) : float4(RGB.gb, 0.0, -1.0 / 3.0);
	float4 Q = (RGB.r < P.x) ? float4(P.xyw, RGB.r) : float4(RGB.r, P.yzx);
	float C = Q.x - min(Q.w, Q.y);
	float H = abs((Q.w - Q.y) / (6.0 * C + Epsilon) + Q.z);
	return float3(H, C, Q.x);
}

float3 RGBtoHSL(float3 RGB)
{
	float3 HCV = RGBtoHCV(RGB);
	float L = HCV.z - HCV.y * 0.5;
	float S = HCV.y / (1.0 - abs(L * 2.0 - 1.0) + Epsilon);
	return float3(HCV.x, S, L);
}

float3 HUEtoRGB(float H)
{
	float R = abs(H * 6.0 - 3.0) - 1.0;
	float G = 2.0 - abs(H * 6.0 - 2.0);
	float B = 2.0 - abs(H * 6.0 - 4.0);
	return clamp(float3(R, G, B), 0.0, 1.0);
}

float3 HSLtoRGB(float3 HSL)
{

	float3 RGB = HUEtoRGB(HSL.x);
	float C = (1.0 - abs(2.0 * HSL.z - 1.0)) * HSL.y;
	return (RGB - 0.5) * C + HSL.z;
}

float3 exponentiate_hsl(float3 color_hsl, float factor)
{
	float luminance = 1.0 - exp(-color_hsl.z * factor);
	return float3(color_hsl.x, color_hsl.y, luminance);
}

float3 exponentiate(float3 color, float factor)
{
	return HSLtoRGB(exponentiate_hsl(RGBtoHSL(color / 10000.0), factor * 10000.0));
}	

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float shade = dot(normalize(input.Normal), float3(0, 1, 0)) * 0.5 + 0.5;
	float3 lighting = lerp(float3(.1, .1, .7), float3(1, .85, 0), 1 - pow(1 - shade, .25));
	float3 diffuse = tex2D(textureSampler, input.TextureCoodinates);
	float3 color = lighting * diffuse;
	return float4(exponentiate(color, brightness), 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};