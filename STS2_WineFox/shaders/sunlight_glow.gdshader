shader_type canvas_item;
render_mode blend_disabled;

// 太阳光晕透明叠加效果（强烈、动态、可调整）

// 光源位置（默认中上方）
uniform vec2 light_position = vec2(0.5, 0.25);

// 光晕强度与范围
uniform float glow_intensity : hint_range(0.0, 3.0) = 1.2;
uniform float glow_radius : hint_range(0.1, 2.0) = 0.6;
uniform float glow_falloff : hint_range(0.5, 5.0) = 2.0;

// 色散与彩虹
uniform float chromatic_aberration : hint_range(0.0, 0.1) = 0.03;
uniform float rainbow_intensity : hint_range(0.0, 1.5) = 0.6;
uniform float rainbow_scale : hint_range(1.0, 20.0) = 8.0;
uniform float rainbow_speed : hint_range(0.0, 2.0) = 0.3;

// 动态光晕层
uniform float flare_intensity : hint_range(0.0, 2.0) = 0.8;
uniform float flare_count : hint_range(3.0, 12.0) = 6.0;
uniform float flare_speed : hint_range(0.0, 1.0) = 0.2;

// 光束 (God Rays)
uniform float ray_intensity : hint_range(0.0, 1.0) = 0.4;
uniform float ray_count : hint_range(3.0, 24.0) = 12.0;
uniform float ray_speed : hint_range(0.0, 1.0) = 0.15;
uniform float ray_width : hint_range(0.01, 0.2) = 0.05;

// 全局动态
uniform float time_scale : hint_range(0.0, 2.0) = 0.5;
uniform float pulse_intensity : hint_range(0.0, 0.5) = 0.15;
uniform float pulse_speed : hint_range(0.0, 3.0) = 0.8;

// 辅助函数：HSV 转 RGB
vec3 hsv2rgb(vec3 c) {
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

// 辅助函数：彩虹色（基于距离和角度）
vec3 rainbow_color(float dist, float angle, float time) {
    float hue = fract(dist * rainbow_scale - angle * 0.5 + time * rainbow_speed);
    return hsv2rgb(vec3(hue, 0.8, 1.0));
}

// 辅助函数：星形光斑
float star_flare(vec2 uv, vec2 center, float arms, float rotation, float sharpness) {
    vec2 d = uv - center;
    float dist = length(d);
    float angle = atan(d.y, d.x) + rotation;
    float star = pow(abs(cos(angle * arms)), sharpness);
    return star * smoothstep(0.5, 0.0, dist);
}

void fragment() {
    vec2 uv = UV;
    
    // 计算到光源的距离和方向
    vec2 to_light = uv - light_position;
    float dist = length(to_light);
    float angle = atan(to_light.y, to_light.x);
    
    // 时间变量
    float t = TIME * time_scale;
    float pulse = 1.0 + sin(t * pulse_speed) * pulse_intensity;
    
    // ============================================
    // 1. 基础光晕 (主太阳)
    // ============================================
    float core = smoothstep(glow_radius * 0.1, 0.0, dist);
    float glow = pow(smoothstep(glow_radius, 0.0, dist), glow_falloff) * glow_intensity * pulse;
    
    // ============================================
    // 2. 彩虹圈 (Rainbow Ring)
    // ============================================
    float rainbow_ring = smoothstep(glow_radius * 0.8, glow_radius * 0.6, dist) 
                        * smoothstep(glow_radius * 0.3, glow_radius * 0.5, dist);
    vec3 rainbow = rainbow_color(dist, angle, t) * rainbow_ring * rainbow_intensity * pulse;
    
    // ============================================
    // 3. 动态光斑 (Lens Flares)
    // ============================================
    float flares = 0.0;
    for (float i = 1.0; i <= 6.0; i += 1.0) {
        float fi = i / 6.0;
        vec2 flare_pos = light_position - to_light * (fi * 2.0 + sin(t * flare_speed + i) * 0.2);
        float flare_size = glow_radius * (0.1 + fi * 0.3);
        float flare = smoothstep(flare_size, 0.0, distance(uv, flare_pos));
        flare *= star_flare(uv, flare_pos, flare_count, t * 0.5 + i, 4.0);
        flares += flare * flare_intensity * (1.0 - fi * 0.5);
    }
    
    // ============================================
    // 4. 光束 (God Rays)
    // ============================================
    float rays = 0.0;
    for (float i = 0.0; i < 3.0; i += 1.0) {
        float ray_angle = angle + t * ray_speed * (1.0 + i * 0.3);
        float ray_wave = sin(ray_angle * ray_count + i * 2.094) * 0.5 + 0.5;
        ray_wave = pow(ray_wave, 1.0 / ray_width);
        float ray_decay = smoothstep(glow_radius * 2.0, 0.0, dist);
        rays += ray_wave * ray_decay * ray_intensity * (0.5 + i * 0.25);
    }
    
    // ============================================
    // 5. 合成透明叠加
    // ============================================
    // 核心太阳（极亮）
    vec3 sun_color = vec3(1.0, 0.95, 0.8);
    vec3 light_color = sun_color * core * glow_intensity * 2.0;
    
    // 光晕层
    light_color += sun_color * glow * 0.5;
    
    // 彩虹
    light_color += rainbow;
    
    // 光斑
    light_color += vec3(1.0, 0.9, 0.7) * flares;
    
    // 光束
    light_color += sun_color * rays * 0.3;

    float total_light = core + glow + flares + rays * 0.5 + rainbow_ring * rainbow_intensity;
    float alpha = clamp(total_light, 0.0, 1.0);
    light_color *= 1.0 + chromatic_aberration * 4.0;
    
    COLOR = vec4(light_color, alpha);
}
