shader_type canvas_item;

uniform sampler2D atlas_texture : source_color;
uniform int atlas_columns = 4;
uniform int atlas_rows = 4;

// 每个粒子的随机种子，由粒子系统传入 INSTANCE_CUSTOM.x
varying float v_random_seed;
varying vec2 v_particle_uv;

void vertex() {
    v_random_seed = INSTANCE_CUSTOM.x;
    v_particle_uv = UV;
}

void fragment() {
    // 计算当前粒子使用图集中的哪一格
    int total_frames = atlas_columns * atlas_rows;
    int frame_index = int(mod(v_random_seed * 100.0, float(total_frames)));
    int col = frame_index % atlas_columns;
    int row = frame_index / atlas_columns;

    float col_f = float(col);
    float row_f = float(row);
    float atlas_cols_f = float(atlas_columns);
    float atlas_rows_f = float(atlas_rows);

    // 直接映射 UV 到图集单格
    vec2 atlas_uv = vec2(
        (v_particle_uv.x + col_f) / atlas_cols_f,
        (v_particle_uv.y + row_f) / atlas_rows_f
    );

    vec4 col_tex = texture(atlas_texture, atlas_uv);
    COLOR = col_tex;
}
