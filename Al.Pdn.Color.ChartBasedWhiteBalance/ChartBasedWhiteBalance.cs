namespace Al.Pdn.Color.ChartBasedWhiteBalance
{
    using Common;
    using PaintDotNet;
    using PaintDotNet.Effects;
    using PaintDotNet.IndirectUI;
    using PaintDotNet.PropertySystem;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// This is based on code by Yves Vander Haeghen.
    /// </summary>
    [EffectCategory(EffectCategory.Adjustment)]
    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "Chart White Balance")]
    public class ChartBasedWhiteBalance : PropertyBasedEffect
    {
        private ColorChart _colorChart;
        private bool _includeSecondaryColor;
        private ColorMap _colorMap;

        private static string StaticName => "Chart White Balance";

        private static string StaticSubMenuName => "Color";

        private static Image StaticImage => null;

        public ChartBasedWhiteBalance() : base(StaticName, StaticImage, StaticSubMenuName,
            new EffectOptions() {RenderingSchedule = EffectRenderingSchedule.None, Flags = EffectFlags.Configurable})
        {
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            var propertyList = new List<Property>
            {
                StaticListChoiceProperty.CreateForEnum(PropertyNames.ColorChart, ColorChart.MacBethColorCheckerChart, false),
                new BooleanProperty(PropertyNames.IncludeSecondaryColor, false)
            };

            return new PropertyCollection(propertyList);
        }

        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            var defaultConfigUi = CreateDefaultConfigUI(props);
            defaultConfigUi.SetPropertyControlValue(PropertyNames.ColorChart, ControlInfoPropertyNames.DisplayName, "Color Chart");
            var controlForPropertyName = defaultConfigUi.FindControlForPropertyName(PropertyNames.ColorChart);
            controlForPropertyName.SetValueDisplayName(ColorChart.MacBethColorCheckerChart, "MacBeth Color Checker Chart");
            controlForPropertyName.SetValueDisplayName(ColorChart.Qp201Chart, "QP201 chart");
            defaultConfigUi.SetPropertyControlValue(PropertyNames.IncludeSecondaryColor, ControlInfoPropertyNames.DisplayName, string.Empty);
            defaultConfigUi.SetPropertyControlValue(PropertyNames.IncludeSecondaryColor, ControlInfoPropertyNames.Description, "Include Secondary Color");

            return defaultConfigUi;
        }

        protected override void OnSetRenderInfo(
            PropertyBasedEffectConfigToken newToken,
            RenderArgs dstArgs,
            RenderArgs srcArgs)
        {
            _colorChart = (ColorChart) newToken.GetProperty<StaticListChoiceProperty>(PropertyNames.ColorChart).Value;
            _includeSecondaryColor = newToken.GetProperty<BooleanProperty>(PropertyNames.IncludeSecondaryColor).Value;
            _colorMap = ComputeColorMap();

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            if (length == 0)
                return;

            for (var index = startIndex; index < startIndex + length; ++index)
                Render(DstArgs.Surface, SrcArgs.Surface, renderRects[index], _colorMap);
        }

        private void Render(Surface dst, Surface src, Rectangle rect, ColorMap colorMap)
        {
            for (var top = rect.Top; top < rect.Bottom; ++top)
            {
                for (var left = rect.Left; left < rect.Right; ++left)
                {
                    var colorBgra = src[left, top];
                    colorBgra.R = colorMap.Red[colorBgra.R];
                    colorBgra.G = colorMap.Green[colorBgra.G];
                    colorBgra.B = colorMap.Blue[colorBgra.B];
                    dst[left, top] = colorBgra;
                }
            }
        }

        private readonly UnsafeRgbColorF _qp201ChartWhiteConstant =
            new UnsafeRgbColorF(0.86587247, 0.86957269, 0.83097223);

        private readonly UnsafeRgbColorF _qp201ChartBlackConstant =
            new UnsafeRgbColorF(0.047443929, 0.045684563, 0.044870793);

        private readonly UnsafeRgbColorF _macBethChartWhiteConstant =
            new UnsafeRgbColorF(0.89259259887904, 0.88688018889244, 0.84364974329818);

        private readonly UnsafeRgbColorF _macBethChartBlackConstant =
            new UnsafeRgbColorF(0.031673922352838, 0.031037900203252, 0.03154464401047);

        private ColorMap ComputeColorMap()
        {
            var primaryColor = EnvironmentParameters.PrimaryColor;
            var normalizedPrimaryColor = primaryColor.ToUnsafeRgbColorF();
            var primaryColorInverseGammaCorrection = normalizedPrimaryColor.Apply(InverseGammaCorrection);

            var secondaryColorInverseGammaCorrection = new UnsafeRgbColorF(0, 0, 0);

            var colorChartWhite = _colorChart == ColorChart.MacBethColorCheckerChart
                ? _macBethChartWhiteConstant
                : _qp201ChartWhiteConstant;

            var colorChartBlack = new UnsafeRgbColorF(0, 0, 0);

            if (_includeSecondaryColor)
            {
                var secondaryColor = EnvironmentParameters.SecondaryColor;
                var normalizedSecondaryColor = secondaryColor.ToUnsafeRgbColorF();
                secondaryColorInverseGammaCorrection = normalizedSecondaryColor.Apply(InverseGammaCorrection);

                colorChartBlack = _colorChart == ColorChart.MacBethColorCheckerChart
                    ? _macBethChartBlackConstant
                    : _qp201ChartBlackConstant;
            }

            var redMap = new byte[256];
            var greenMap = new byte[256];
            var blueMap = new byte[256];

            var denominator = primaryColorInverseGammaCorrection.Subtract(secondaryColorInverseGammaCorrection);
            var colorsAccordingToChart = denominator.IsZero
                ? UnsafeRgbColorF.One
                : colorChartWhite.Subtract(colorChartBlack).DivideBy(denominator);

            for (var index = 0; index < 256; index++)
            {
                var num = InverseGammaCorrection((double) index / byte.MaxValue);
                var color = num.Subtract(secondaryColorInverseGammaCorrection).MultiplyBy(colorsAccordingToChart)
                    .Add(colorChartBlack)
                    .Apply(GammaCorrection).ToColorBgra();
                redMap[index] = color.R;
                greenMap[index] = color.G;
                blueMap[index] = color.B;
            }

            return new ColorMap(redMap, greenMap, blueMap);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double GammaCorrection(double sIn)
        {
            return sIn > 0.018
                ? (sIn >= 1.0 ? 1.0 : 1.099 * Math.Pow(sIn, 0.45) - 0.099)
                : (sIn < 0.0 ? 0.0 : 4.5 * sIn);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double InverseGammaCorrection(double sIn)
        {
            return sIn <= 0.081 ? sIn / 4.5 : Math.Pow((sIn + 0.099) / 1.099, 2.22222222);
        }

        private enum PropertyNames
        {
            ColorChart,
            IncludeSecondaryColor,
        }

        private enum ColorChart
        {
            MacBethColorCheckerChart,
            Qp201Chart,
        }
    }
}
