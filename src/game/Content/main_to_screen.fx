#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float brightness;

sampler2D source = sampler_state
{
    Texture = (source);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct PixelShaderInput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
    float4 TextureCoordinate : TEXCOORD0;
};

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

float4 MainPS(PixelShaderInput input) : COLOR
{
	float3 color = tex2D(source, input.TextureCoordinate.xy).xyz;
    return float4(exponentiate(color, brightness), 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};