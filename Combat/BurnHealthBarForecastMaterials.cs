using Godot;
using STS2RitsuLib.Utils;

namespace STS2_WineFox.Combat
{
    /// <summary>
    ///     Shared doom-bar shader material (game <c>doom_bar.gdshader</c> via <see cref="MaterialUtils" />) with a warm
    ///     orange gradient for burn forecasts.
    /// </summary>
    internal static class BurnHealthBarForecastMaterials
    {
        private static ShaderMaterial? _forecastMaterial;

        public static Material ForecastMaterial =>
            _forecastMaterial ??= Create();

        private static ShaderMaterial Create()
        {
            var gradient = new Gradient();
            gradient.AddPoint(0f, new(0.52f, 0.12f, 0.035f));
            gradient.AddPoint(0.5f, new(0.68f, 0.24f, 0.055f));
            gradient.AddPoint(1f, new(0.74f, 0.34f, 0.07f));

            return MaterialUtils.CreateDoomBarShaderMaterial(new() { Gradient = gradient });
        }
    }
}
