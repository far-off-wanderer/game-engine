#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

half brightness;

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
	half4 Position : SV_POSITION;
	half4 Color : COLOR0;
    half4 TextureCoordinate : TEXCOORD0;
};

half Epsilon = 1e-10;
 
half3 RGBtoHCV(half3 RGB)
{
  // Based on work by Sam Hocevar and Emil Persson
    half4 P = (RGB.g < RGB.b) ? half4(RGB.bg, -1.0, 2.0 / 3.0) : half4(RGB.gb, 0.0, -1.0 / 3.0);
    half4 Q = (RGB.r < P.x) ? half4(P.xyw, RGB.r) : half4(RGB.r, P.yzx);
    half C = Q.x - min(Q.w, Q.y);
    half H = abs((Q.w - Q.y) / (6.0 * C + Epsilon) + Q.z);
    return half3(H, C, Q.x);
}

half3 RGBtoHSL(half3 RGB)
{
    half3 HCV = RGBtoHCV(RGB);
    half L = HCV.z - HCV.y * 0.5;
    half S = HCV.y / (1.0 - abs(L * 2.0 - 1.0) + Epsilon);
    return half3(HCV.x, S, L);
}

half3 HUEtoRGB(half H)
{
    half R = abs(H * 6.0 - 3.0) - 1.0;
    half G = 2.0 - abs(H * 6.0 - 2.0);
    half B = 2.0 - abs(H * 6.0 - 4.0);
    return clamp(half3(R, G, B), 0.0, 1.0);
}

half3 HSLtoRGB(half3 HSL)
{

    half3 RGB = HUEtoRGB(HSL.x);
    half C = (1.0 - abs(2.0 * HSL.z - 1.0)) * HSL.y;
    return (RGB - 0.5) * C + HSL.z;
}

half3 exponentiate_hsl(half3 color_hsl, half factor)
{
    half luminance = 1.0 - exp(-color_hsl.z * factor);
    return half3(color_hsl.x, color_hsl.y, luminance);
}

half3 exponentiate(half3 color, half factor)
{
    return HSLtoRGB(exponentiate_hsl(RGBtoHSL(color / 10000.0), factor * 10000.0));
}

half3 gamma(half3 color)
{
    return pow(color, 1 / 2.2);
}

half4 MainPS(PixelShaderInput input) : COLOR
{
	half3 color = tex2D(source, input.TextureCoordinate.xy).xyz;
    half3 color_ldr = exponentiate(color, brightness);
    half3 color_ldr_gamma = gamma(color_ldr);
    return half4(color_ldr_gamma, 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
    }
};