shader_type canvas_item;

/// 横向完整色相循环次数。
uniform float horizontal_cycles : hint_range(0.5, 8.0) = 2.0;
/// 轻微引入纵向变化，避免纯横条纹过于生硬。
uniform float vertical_hue_weight : hint_range(0.0, 1.0) = 0.12;
uniform float scroll_speed : hint_range(-3.0, 3.0) = 0.55;
/// 饱和度缩放（<1 更淡）。
uniform float s : hint_range(0.0, 2.0) = 0.6;
/// 亮度缩放。
uniform float v : hint_range(0.5, 1.5) = 1.0;
/// 彩虹效果混合强度（更低更淡）。
uniform float rainbow_strength : hint_range(0.0, 1.0) = 0.5;
/// 额外提亮到白色，营造偏粉彩质感。
uniform float pastel_lift : hint_range(0.0, 0.5) = 0.12;

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

	vec3 base_rgb = col.rgb;
	float h = fract(
		UV.x * horizontal_cycles
		+ UV.y * vertical_hue_weight
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
	col.rgb = mix(col.rgb, vec3(1.0), pastel_lift);
	col.rgb = mix(base_rgb, col.rgb, alpha_mask * rainbow_strength);
	COLOR = col;
	COLOR *= modulate_color;
}
