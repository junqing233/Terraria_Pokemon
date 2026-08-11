sampler2D TextureSampler : register(s0);

float time;
float amplitude;
float frequency;

struct VS_OUTPUT
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
};

VS_OUTPUT VS(float4 Pos : POSITION, float2 TexCoord : TEXCOORD0)
{
    VS_OUTPUT output;
    output.Position = Pos;
    output.TexCoord = TexCoord;
    return output;
}

float4 PS(VS_OUTPUT input) : COLOR
{
    // 生成更复杂的波动效果
    float wave1 = sin(input.TexCoord.y * frequency + time) * amplitude;
    float wave2 = cos(input.TexCoord.y * frequency * 0.5 + time * 1.5) * amplitude * 0.5;
    float wave3 = sin(input.TexCoord.y * frequency * 2.0 + time * 0.5) * amplitude * 0.25;

    float wave = wave1 + wave2 + wave3;
    input.TexCoord.x += wave;

    float4 color = tex2D(TextureSampler, input.TexCoord);
    color.a = 1.0; // 确保 alpha 值为 1.0
    return color;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
    }
}
