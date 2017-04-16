matrix worldViewProjection;
matrix worldViewProjectionTransposed;

sampler albedoSampler = sampler_state
{
    Texture = (Albedo);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

struct SceneToGBufferVSInput
{
	float3 Position : POSITION0;
    float3 Normal : NORMAL0;
	float2 TextureCoordinates : TEXCOORD0;
};

struct SceneToGBufferPSInput
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct SceneToGBufferPSOutput
{
    float4 Albedo : COLOR0;
    float4 Normal : COLOR1;
};

SceneToGBufferPSInput SceneToGBufferVS(in SceneToGBufferVSInput input)
{
    SceneToGBufferPSInput output = (SceneToGBufferPSInput)0;

    output.Position = mul(float4(input.Position, 1), worldViewProjection);
    output.Normal = mul(input.Normal, (float3x4) worldViewProjectionTransposed).xyz;
    output.TextureCoordinates = input.TextureCoordinates;

	return output;
}

SceneToGBufferPSOutput SceneToGBufferPS(SceneToGBufferPSInput input)
{
    SceneToGBufferPSOutput output = (SceneToGBufferPSOutput)0;

    output.Albedo = tex2D(albedoSampler, input.TextureCoordinates);
    output.Normal = float4(input.Normal, 0);

    return output;
}


sampler gbufferAlbedoSampler = sampler_state
{
    Texture = (GBufferAlbedo);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};
sampler gbufferNormalSampler = sampler_state
{
    Texture = (GBufferNormal);
    MagFilter = Linear;
    MinFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};
struct GBufferToScreenVSInput
{
    float4 Position : POSITION0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct GBufferToScreenPSInput
{
    float4 Position : SV_POSITION;
    float3 Normal : NORMAL0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct GBufferToScreenPSOutput
{
    float4 ScreenColor : COLOR0;
};

GBufferToScreenPSInput GBufferToScreenVS(in GBufferToScreenVSInput input)
{
    GBufferToScreenPSInput output = (GBufferToScreenPSInput) 0;

    output.Position = input.Position;
    output.Position.x = input.Position.x * 2 - 1;
    output.Position.y = 1 - input.Position.y * 2;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float4 GBufferToScreenPS(GBufferToScreenPSInput input) : COLOR0
{
    float4 Albedo = tex2D(gbufferAlbedoSampler, input.TextureCoordinates);
    float4 Normal = normalize(tex2D(gbufferNormalSampler, input.TextureCoordinates));

    float4 ScreenColor = Albedo;
    ScreenColor.a = Normal.y;

    return ScreenColor;
}

technique Shaders
{
	pass SceneToGBuffer
	{
        VertexShader = compile vs_4_0_level_9_1 SceneToGBufferVS();
        PixelShader = compile ps_4_0_level_9_1 SceneToGBufferPS();
    }

    pass GBufferToScreen
    {
        VertexShader = compile vs_4_0_level_9_1 GBufferToScreenVS();
        PixelShader = compile ps_4_0_level_9_1 GBufferToScreenPS();
    }
};