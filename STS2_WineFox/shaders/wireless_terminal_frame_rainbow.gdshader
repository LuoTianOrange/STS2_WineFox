shader_type canvas_item;

uniform vec2 frame_center = vec2(0.5, 0.5);
/// 沿外周绕几圈完整色相（>1 时窄边框上也能看到明显色差）。
uniform float perimeter_cycles : hint_range(0.5, 12.0) = 4.0;
/// 用 UV 额外拉开色相，避免实际绘制区域在角度上挤在一小段里时整框看起来一个色。
uniform float uv_hue_weight : hint_range(0.0, 2.0) = 0.45;
uniform float scroll_speed : hint_range(-3.0, 3.0) = 0.55;
uniform float s : hint_range(0.0, 5.0) = 1.5;
uniform float v = 1.2;

varying vec4 modulate_color;

void vertex() {
	modulate_color = COLOR;
}

void fragment() {
	vec4 col = texture(TEXTURE, UV);
	float alpha_mask = step(0.001, col.a);

	mat3 RGB_to_YIQ = mat3(
		vec3(0.2989, 0.5959, 0.2115),
		vec3(0.5870, -0.2774, -0.5229),
		vec3(0.1140, -0.3216, 0.3114));

	vec2 d = UV - frame_center;
	float ang = atan(d.y, d.x);
	float h = fract(
		ang * 0.15915494309 * perimeter_cycles
		+ dot(UV, vec2(0.73, 1.07)) * uv_hue_weight
		+ TIME * scroll_speed);

	col.rgb = RGB_to_YIQ * col.rgb;
	float hue = 1.0 - h;
	hue = mix(0.0, 6.283185, hue);

	float sin_hue = sin(hue);
	float cos_hue = cos(hue);
	mat3 hue_shift = mat3(
		vec3(1.0, 0.0, 0.0),
		vec3(0.0, cos_hue, -sin_hue),
		vec3(0.0, sin_hue, cos_hue));
	col.rgb *= hue_shift;

	mat3 sat_shift = mat3(
		vec3(1.0, 0.0, 0.0),
		vec3(0.0, s, 0.0),
		vec3(0.0, 0.0, s));
	col.rgb = sat_shift * col.rgb;

	col.rgb = mix(vec3(0.0, 0.0, 0.0), col.rgb, v);

	col.rgb = inverse(RGB_to_YIQ) * col.rgb;
	col.rgb = mix(texture(TEXTURE, UV).rgb, col.rgb, alpha_mask);
	COLOR = col;
	COLOR *= modulate_color;
}
