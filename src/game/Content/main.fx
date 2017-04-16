#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

float brightness;

matrix worldViewProjection;
matrix worldViewProjectionTransposed;

sampler2D albedo_sampler = sampler_state
{
	Texture = (albedo);
	MagFilter = Linear;
	MinFilter = Linear;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 Normal : NORMAL0;
	float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = mul(input.Position, worldViewProjection);
	output.TextureCoordinates = input.TextureCoordinates;
	output.Normal = mul(input.Normal, (float3x4)worldViewProjectionTransposed).xyz;

	return output;
}

#define PI 3.14159265

float orenNayarDiffuse(float3 lightDirection, float3 viewDirection, float3 surfaceNormal, float roughness, float albedo)
{
  
    float LdotV = dot(lightDirection, viewDirection);
    float NdotL = dot(lightDirection, surfaceNormal);
    float NdotV = dot(surfaceNormal, viewDirection);

    float s = LdotV - NdotL * NdotV;
    float t = lerp(1.0, max(NdotL, NdotV), step(0.0, s));

    float sigma2 = roughness * roughness;
    float A = 1.0 + sigma2 * (albedo / (sigma2 + 0.13) + 0.5 / (sigma2 + 0.33));
    float B = 0.45 * sigma2 / (sigma2 + 0.09);

    return albedo * max(0.0, NdotL) * (A + B * s / t) / PI;
}

float beckmannDistribution(float x, float roughness)
{
    float NdotH = max(x, 0.0001);
    float cos2Alpha = NdotH * NdotH;
    float tan2Alpha = (cos2Alpha - 1.0) / cos2Alpha;
    float roughness2 = roughness * roughness;
    float denom = 3.141592653589793 * roughness2 * cos2Alpha * cos2Alpha;
    return exp(tan2Alpha / roughness2) / denom;
}

float cookTorranceSpecular(float3 lightDirection, float3 viewDirection, float3 surfaceNormal, float roughness, float fresnel)
{
    float VdotN = max(dot(viewDirection, surfaceNormal), 0.0);
    float LdotN = max(dot(lightDirection, surfaceNormal), 0.0);

    //Half angle floattor
    float3 H = normalize(lightDirection + viewDirection);

    //Geometric term
    float NdotH = max(dot(surfaceNormal, H), 0.0);
    float VdotH = max(dot(viewDirection, H), 0.000001);
    float x = 2.0 * NdotH / VdotH;
    float G = min(1.0, min(x * VdotN, x * LdotN));
  
    //Distribution term
    float D = beckmannDistribution(NdotH, roughness);

    //Fresnel term
    float F = pow(1.0 - VdotN, fresnel);

    //Multiply terms and done
    return G * F * D / max(3.14159265 * VdotN * LdotN, 0.000001);
}



float4 MainPS(VertexShaderOutput input) : COLOR
{
    float3 albedo = pow(tex2D(albedo_sampler, input.TextureCoordinates).xyz, 2.2);
    float3 normal = normalize(input.Normal);
    float3 light = float3(0.707, 0.707, 0);
    float3 view = float3(0, 0, -1);

    float roughness = 16;
    float diffuse = orenNayarDiffuse(light, view, normal, roughness, 1);
    float3 specular = cookTorranceSpecular(light, view, normal, roughness, 10);

    float3 color = diffuse * albedo + specular;

	return float4(color * brightness, 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};