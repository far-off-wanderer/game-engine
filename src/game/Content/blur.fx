
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

float4 kawaseBloom(sampler2D image, float2 uv, float2 texturesize, float iteration)
{
    float2 texelSize = 1.0 / texturesize;
    float2 texelSize05 = texelSize * 0.5;
    
    float2 uvOffset = texelSize.xy * float2(iteration, iteration) + texelSize05;

    const float2 upright = float2(1, 1);
    const float2 upleft = float2(-1, 1);
    const float2 downright = float2(1, -1);
    const float2 downleft = float2(-1, -1);
    
    return (
        tex2D(image, uv + upright * uvOffset)
        +
        tex2D(image, uv + upleft * uvOffset)
        +
        tex2D(image, uv + downright * uvOffset)
        +
        tex2D(image, uv + downleft * uvOffset)
    ) * 0.25;
}

float4 MainPS(PixelShaderInput input) : COLOR
{
    return kawaseBloom(source, input.TextureCoordinate.xy, resolution, iteration);
}

technique BasicColorDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};