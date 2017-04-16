#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

sampler2D non_blurred = sampler_state
{
    Texture = (non_blurred);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler2D half_blurred = sampler_state
{
    Texture = (half_blurred);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler2D _3quarter_blurred = sampler_state
{
    Texture = (_3quarter_blurred);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler2D blurred = sampler_state
{
    Texture = (blurred);
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
    return HSLtoRGB(exponentiate_hsl(RGBtoHSL(color / 100.0), factor * 100.0));
}

float3 gamma(float3 color)
{
    return pow(max(0, color), 1 / 2.2);
}

float A = 0.15;
float B = 0.50;
float C = 0.10;
float D = 0.20;
float E = 0.02;
float F = 0.30;
float W = 11.2;

float3 Uncharted2Tonemap(float3 x)
{
    return ((x * (A * x + C * B) + D * E) / (x * (A * x + B) + D * F)) - E / F;
}

float4 MainPS(PixelShaderInput input) : COLOR
{
    float3 color_non_blurred = tex2D(non_blurred, input.TextureCoordinate.xy).xyz;
	float3 color_blurred = tex2D(blurred, input.TextureCoordinate.xy).xyz;
    
    float3 color = color_non_blurred + color_blurred / 4; // blur shouldn't be here..

    float ExposureBias = 1.0f;
    float3 curr = Uncharted2Tonemap(ExposureBias * color);

    float3 whiteScale = 1.0f / Uncharted2Tonemap(W);
    color = curr * whiteScale;
      
    color = pow(color, 1 / 2.2); 
    color.g = 1;

    return float4(color, 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
    }
};