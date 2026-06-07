shader_type canvas_item;
render_mode blend_mix;

uniform float star_density : hint_range(20.0, 240.0) = 110.0;
uniform float star_size : hint_range(0.001, 0.08) = 0.016;
uniform float twinkle_speed : hint_range(0.1, 8.0) = 2.4;
uniform float twinkle_interval : hint_range(0.4, 6.0) = 2.2;
uniform float twinkle_window : hint_range(0.05, 0.9) = 0.26;
uniform float twinkle_floor : hint_range(0.0, 1.0) = 0.35;
uniform float drift_speed : hint_range(0.0, 0.4) = 0.03;
uniform float glow_strength : hint_range(0.0, 2.0) = 0.55;
uniform float tint_strength : hint_range(0.0, 1.0) = 0.22;
uniform float breath_speed : hint_range(0.05, 2.5) = 0.33;
uniform float breath_intensity : hint_range(0.0, 0.7) = 0.18;
uniform float nebula_strength : hint_range(0.0, 2.0) = 0.58;
uniform float nebula_scale : hint_range(0.5, 8.0) = 2.2;
uniform float nebula_speed : hint_range(0.0, 2.0) = 0.22;
uniform vec3 nebula_cold : source_color = vec3(0.08, 0.34, 0.95);
uniform vec3 nebula_hot : source_color = vec3(0.62, 0.38, 1.00);
uniform float background_opacity : hint_range(0.0, 1.0) = 0.5;
uniform vec3 frame_dark_tint : source_color = vec3(0.02, 0.02, 0.025);
uniform float frame_luma_keep : hint_range(0.0, 0.5) = 0.18;
uniform vec3 bg_color : source_color = vec3(0.0, 0.0, 0.0);

float hash11(float p) {
    return fract(sin(p) * 43758.5453123);
}

float hash21(vec2 p) {
    return fract(sin(dot(p, vec2(127.1, 311.7))) * 43758.5453123);
}

float noise21(vec2 p) {
    vec2 i = floor(p);
    vec2 f = fract(p);
    f = f * f * (3.0 - 2.0 * f);

    float a = hash21(i);
    float b = hash21(i + vec2(1.0, 0.0));
    float c = hash21(i + vec2(0.0, 1.0));
    float d = hash21(i + vec2(1.0, 1.0));

    return mix(mix(a, b, f.x), mix(c, d, f.x), f.y);
}

vec2 rotate2(vec2 p, float a) {
    float s = sin(a);
    float c = cos(a);
    return vec2(c * p.x - s * p.y, s * p.x + c * p.y);
}

float fbm(vec2 p) {
    float value = 0.0;
    float amp = 0.5;
    for (int i = 0; i < 5; i++) {
        value += noise21(p) * amp;
        p = rotate2(p * 2.03, 0.63) + vec2(11.7, 7.9);
        amp *= 0.5;
    }
    return value;
}

float fbm_warped(vec2 p) {
    // Domain warping breaks the square-cell feel from value noise.
    vec2 q = vec2(
        fbm(p + vec2(0.0, 0.0)),
        fbm(p + vec2(5.2, 1.3))
    );

    vec2 r = vec2(
        fbm(p + q * 3.8 + vec2(1.7, 9.2)),
        fbm(p + q * 3.8 + vec2(8.3, 2.8))
    );

    return fbm(p + r * 2.4);
}

vec3 star_tint(float seed) {
    float hue = fract(seed * 0.61803398875 + 0.07);
    vec3 cold = vec3(0.70, 0.80, 1.00);
    vec3 warm = vec3(1.00, 0.88, 0.74);
    vec3 violet = vec3(0.82, 0.72, 1.00);

    vec3 mixed = mix(cold, warm, smoothstep(0.0, 0.55, hue));
    mixed = mix(mixed, violet, smoothstep(0.65, 1.0, hue));
    return mix(vec3(1.0), mixed, tint_strength);
}

vec3 star_layer(vec2 uv, float scale, float seed_offset, float parallax) {
    vec2 layer_uv = uv;
    layer_uv.x += TIME * drift_speed * parallax;

    vec2 cell_uv = layer_uv * scale;
    vec2 cell_id = floor(cell_uv);
    vec2 local = fract(cell_uv) - 0.5;

    float seed = hash21(cell_id + vec2(seed_offset, seed_offset * 1.37));
    float appears = step(1.0 - (star_density / 220.0), seed);
    if (appears <= 0.0) {
        return vec3(0.0);
    }

    vec2 jitter = vec2(
        hash21(cell_id + vec2(seed_offset + 13.1, seed_offset + 7.2)),
        hash21(cell_id + vec2(seed_offset + 5.4, seed_offset + 19.7))
    ) - 0.5;

    vec2 d = local - jitter * 0.70;
    float radius = mix(star_size * 0.45, star_size, hash11(seed * 91.3));
    float core = smoothstep(radius, 0.0, length(d));

    float twinkle_cycle = TIME * (twinkle_speed / max(0.001, twinkle_interval)) + seed * 19.0;
    float phase = fract(twinkle_cycle);
    float edge = 0.04;
    float window = clamp(twinkle_window, 0.05, 0.95);
    float pulse_in = smoothstep(0.0, edge, phase);
    float pulse_out = 1.0 - smoothstep(window, min(1.0, window + edge), phase);
    float pulse = clamp(pulse_in * pulse_out, 0.0, 1.0);

    float osc = 0.5 + 0.5 * sin(TIME * twinkle_speed + seed * 23.0);
    float twinkle = mix(twinkle_floor, 1.0, pulse * osc);

    float halo = smoothstep(radius * 2.2, 0.0, length(d)) * glow_strength;
    float brightness = (core + halo) * twinkle;

    return star_tint(seed) * brightness;
}

void fragment() {
    vec2 uv = UV;
    vec4 base = texture(TEXTURE, uv);
    float alpha_mask = step(0.001, base.a);
    float breath = 1.0 + sin(TIME * breath_speed * 6.28318530718) * breath_intensity;

    vec3 stars = vec3(0.0);
    stars += star_layer(uv, 38.0, 3.1, 0.45);
    stars += star_layer(uv, 58.0, 7.4, 0.70);
    stars += star_layer(uv, 92.0, 11.9, 1.00);
    stars *= breath;

    stars = clamp(stars, 0.0, 1.0);

    vec2 flow_uv = uv * nebula_scale + vec2(TIME * nebula_speed * 0.09, TIME * nebula_speed * 0.05);
    float n_base = fbm_warped(flow_uv);
    float n_detail = fbm_warped(flow_uv * 1.73 + vec2(7.1, 3.9));
    float n_ridge = 1.0 - abs(n_base * 2.0 - 1.0);

    float cloud_shape = n_base * 0.70 + n_detail * 0.34 + n_ridge * 0.16;
    float nebula_mask = smoothstep(0.58, 0.98, cloud_shape);
    nebula_mask *= smoothstep(0.12, 0.42, n_detail + n_ridge * 0.28);
    nebula_mask *= smoothstep(0.46, 0.82, n_base);

    vec3 nebula_col = mix(nebula_cold, nebula_hot, smoothstep(0.24, 0.90, n_detail));
    vec3 nebula = nebula_col * nebula_mask * nebula_strength * 0.75;

    // Neutral darkening: remove gold hue bias, keep a bit of luminance detail.
    float luma = dot(base.rgb, vec3(0.299, 0.587, 0.114));
    vec3 neutral_dark = frame_dark_tint + vec3(luma * frame_luma_keep);
    vec3 out_rgb = mix(base.rgb, neutral_dark, alpha_mask * background_opacity);
    out_rgb += nebula * alpha_mask;
    out_rgb += stars * alpha_mask * 0.75;

    COLOR = vec4(clamp(out_rgb, 0.0, 1.0), base.a);
}

