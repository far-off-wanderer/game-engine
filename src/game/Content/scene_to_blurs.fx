#if OPENGL
#define SV_POSITION POSITION
#define PS_SHADERMODEL ps_3_0
#else
#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

float2 resolution;
float iteration;

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
    float4 color = tex2D(source, input.TextureCoordinate.xy);

    
    float3 whiteScale = 1.0f / Uncharted2Tonemap(W);
    whiteScale = 0;
    //return max(0, (exp2(color) - 1) / 2);
    return float4(max(0, color.xyz - whiteScale), color.w);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};