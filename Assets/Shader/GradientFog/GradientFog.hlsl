#ifndef GRADIENTFOG_INCLUDED
#define GRADIENTFOG_INCLUDED

    float4 noFog = float4(0, 0, 0, 1);

    void GradientFog_float(float4 a, float4 b, float4 c, float fog_start, float fog_max, float first_color_threshold, float second_color_threshold, float t, out float4 d)
    {
        float fogStartReciprocal = 1 / fog_start;
        float fogMaxReciprocal = 1 / fog_max;
        
        float alpha = step(t, fog_start) * lerp(0, fog_max, t * fogStartReciprocal);
        alpha += step(fog_start, t) * lerp(fog_max, 1, (t - fog_start) * fogMaxReciprocal);
        
        float firstColorThresholdReciprocal = 1 / first_color_threshold;
        float secondColorThresholdReciprocal = 1 / (1 - second_color_threshold);
        float middleColorThresholdReciprocal = 1 / (second_color_threshold - first_color_threshold);
    
        float4 color = step(t, first_color_threshold) * lerp(noFog, a, t * firstColorThresholdReciprocal);
        color += step(first_color_threshold, t) * step(t, second_color_threshold) * lerp(a, b, (t - first_color_threshold) * middleColorThresholdReciprocal);
        color += step(second_color_threshold, t) * lerp(b, c, saturate((t - second_color_threshold) * secondColorThresholdReciprocal));
    
        d = float4(color.r, color.g, color.b, saturate(alpha));
    }

#endif