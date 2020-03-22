namespace Al.Pdn.Color.WhiteBalance
{
    using Common;
    using PaintDotNet;
    using PaintDotNet.Effects;
    using PaintDotNet.IndirectUI;
    using PaintDotNet.PropertySystem;
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    [EffectCategory(EffectCategory.Adjustment)]
    [PluginSupportInfo(typeof(PluginSupportInfo), DisplayName = "White Balance")]
    public class WhiteBalance : PropertyBasedEffect
    {
        private WhiteBalanceMethod _lastSetWhiteBalanceMethod = WhiteBalanceMethod.None;

        private ColorMap _colorMap;

        private static string StaticName => "White Balance";

        private static string StaticSubMenuName => "Color";

        private static Image StaticImage => null;

        public WhiteBalance() : base(StaticName, StaticImage, StaticSubMenuName,
            new EffectOptions() { RenderingSchedule = EffectRenderingSchedule.None, Flags = EffectFlags.Configurable })
        {
        }

        private enum WhiteBalanceMethod
        {
            None,
            GrayWorld,
            Retinex,
            YCbCr
        }

        private enum PropertyNames
        {
            RedGain,
            GreenGain,
            BlueGain,
            WhiteBalanceMethod
        }

        protected override PropertyCollection OnCreatePropertyCollection()
        {
            var propertyList = new List<Property>
            {
                StaticListChoiceProperty.CreateForEnum(PropertyNames.WhiteBalanceMethod, WhiteBalanceMethod.None, false),
                new DoubleProperty(PropertyNames.RedGain, 1.0, 0.0, 2.0),
                new DoubleProperty(PropertyNames.GreenGain, 1.0, 0.0, 2.0),
                new DoubleProperty(PropertyNames.BlueGain, 1.0, 0.0, 2.0)
            };

            return new PropertyCollection(propertyList);
        }

        protected override ControlInfo OnCreateConfigUI(PropertyCollection props)
        {
            var defaultConfigUi = CreateDefaultConfigUI(props);
            defaultConfigUi.SetPropertyControlValue(PropertyNames.RedGain, ControlInfoPropertyNames.DisplayName, "Red Gain");
            defaultConfigUi.SetPropertyControlValue(PropertyNames.GreenGain, ControlInfoPropertyNames.DisplayName, "Green Gain");
            defaultConfigUi.SetPropertyControlValue(PropertyNames.BlueGain, ControlInfoPropertyNames.DisplayName, "Blue Gain");

            defaultConfigUi.SetPropertyControlValue(PropertyNames.WhiteBalanceMethod, ControlInfoPropertyNames.DisplayName, "White Balance Method");

            var controlForPropertyName = defaultConfigUi.FindControlForPropertyName(PropertyNames.WhiteBalanceMethod);
            controlForPropertyName.SetValueDisplayName(WhiteBalanceMethod.None, "None");
            controlForPropertyName.SetValueDisplayName(WhiteBalanceMethod.GrayWorld, "GrayWorld");
            controlForPropertyName.SetValueDisplayName(WhiteBalanceMethod.Retinex, "Retinex");
            controlForPropertyName.SetValueDisplayName(WhiteBalanceMethod.YCbCr, "YCbCr");

            return defaultConfigUi;
        }

        protected override void OnSetRenderInfo(
            PropertyBasedEffectConfigToken newToken,
            RenderArgs dstArgs,
            RenderArgs srcArgs)
        {
            UpdatePropertiesIfNeeded(newToken);

            var redGain = newToken.GetProperty<DoubleProperty>(PropertyNames.RedGain).Value;
            var greenGain = newToken.GetProperty<DoubleProperty>(PropertyNames.GreenGain).Value;
            var blueGain = newToken.GetProperty<DoubleProperty>(PropertyNames.BlueGain).Value;

            _colorMap = ComputeColorMap(redGain, greenGain, blueGain);

            base.OnSetRenderInfo(newToken, dstArgs, srcArgs);
        }

        private void UpdatePropertiesIfNeeded(PropertyBasedEffectConfigToken newToken)
        {
            var whiteBalanceMethod = (WhiteBalanceMethod) newToken.GetProperty<StaticListChoiceProperty>(PropertyNames.WhiteBalanceMethod).Value;

            if (_lastSetWhiteBalanceMethod == whiteBalanceMethod)
                return;

            UpdateProperties(newToken, whiteBalanceMethod);
            _lastSetWhiteBalanceMethod = whiteBalanceMethod;
        }

        private void UpdateProperties(PropertyBasedEffectConfigToken newToken, WhiteBalanceMethod whiteBalanceMethod)
        {
            var whiteBalanceFunction = GetWhiteBalanceFunction(whiteBalanceMethod);

            if (whiteBalanceFunction == null) 
                return;

            var (redGain, greenGain, blueGain) = whiteBalanceFunction(SrcArgs.Surface);
            (redGain, greenGain, blueGain) = Normalize(redGain, greenGain, blueGain);

            newToken.SetPropertyValue(PropertyNames.RedGain, redGain);
            newToken.SetPropertyValue(PropertyNames.GreenGain, greenGain);
            newToken.SetPropertyValue(PropertyNames.BlueGain, blueGain);

            // XXX : How to update controls with new values ?
            // This cannot be done with PropertyBasedEffect effects (The so called IndirectUI).
            // The solution is to inherit from Effect, and develop the Winform UI manually. (See ScribbleEffect source code)
        }

        private Func<Surface, (double redGain, double greenGain, double blueGain)> GetWhiteBalanceFunction(WhiteBalanceMethod whiteBalanceMethod)
        {
            Func<Surface, (double redGain, double greenGain, double blueGain)> function = null;

            switch (whiteBalanceMethod)
            {
                case WhiteBalanceMethod.GrayWorld:
                    function = GrayWorld;
                    break;
                case WhiteBalanceMethod.Retinex:
                    function = Retinex;
                    break;
                case WhiteBalanceMethod.YCbCr:
                    function = YCbCr;
                    break;
                case WhiteBalanceMethod.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return function;
        }

        private static (double redGain, double greenGain, double blueGain) Normalize(double redGain, double greenGain, double blueGain)
        {
            var factor = Math.Max(Math.Max(redGain, greenGain), blueGain);

            return (redGain / factor, greenGain / factor, blueGain / factor);
        }

        private (double redGain, double greenGain, double blueGain) YCbCr(Surface src)
        {
            var maxY = 0.0;
            var redAccumulator = 0.0;
            var greenAccumulator = 0.0;
            var blueAccumulator = 0.0;

            for (var x = 0; x < src.Width; x++)
            {
                for (var y = 0; y < src.Height; y++)
                {
                    var rgbColor = src[x, y];
                    var ycbcrColor = YCbCrColor.FromRgb(rgbColor);
                    maxY = Math.Max(maxY, ycbcrColor.Y);
                    redAccumulator += rgbColor.R;
                    greenAccumulator += rgbColor.G;
                    blueAccumulator += rgbColor.B;
                }
            }

            var pixelCount = src.Width * src.Height;
            var redAverage = redAccumulator / pixelCount;
            var greenAverage = greenAccumulator / pixelCount;
            var blueAverage = blueAccumulator / pixelCount;

            return (maxY / redAverage, maxY/greenAverage, maxY / blueAverage);
        }

        private (double redGain, double greenGain, double blueGain) Retinex(Surface src)
        {
            var maxRed = 0.0;
            var maxGreen = 0.0;
            var maxBlue = 0.0;

            for (var x = 0; x < src.Width; x++)
            {
                for (var y = 0; y < src.Height; y++)
                {
                    var color = src[x, y];
                    maxRed = Math.Max(maxRed, color.R);
                    maxGreen = Math.Max(maxRed, color.G);
                    maxBlue = Math.Max(maxRed, color.B);
                }
            }

            return (maxGreen / maxRed, 1.0, maxGreen / maxBlue);
        }

        private static (double redGain, double greenGain, double blueGain) GrayWorld(Surface src)
        {
            var redAccumulator = 0.0;
            var greenAccumulator = 0.0;
            var blueAccumulator = 0.0;

            for (var x = 0; x < src.Width; x++)
            {
                for (var y = 0; y < src.Height; y++)
                {
                    var color = src[x, y];
                    redAccumulator += color.R;
                    greenAccumulator += color.G;
                    blueAccumulator += color.B;
                }
            }

            var pixelCount = src.Width * src.Height;
            var redAverage = redAccumulator / pixelCount;
            var greenAverage = greenAccumulator / pixelCount;
            var blueAverage = blueAccumulator / pixelCount;

            return (redAverage / greenAverage, 1.0, blueAverage / greenAverage);
        }

        private static ColorMap ComputeColorMap(double redGain, double greenGain, double blueGain)
        {
            var redMap = new byte[256];
            var greenMap = new byte[256];
            var blueMap = new byte[256];

            for (var i = 0; i < 256; i++)
            {
                redMap[i] = Int32Util.ClampToByte((int) Math.Round(i * redGain));
                greenMap[i] = Int32Util.ClampToByte((int)Math.Round(i * greenGain));
                blueMap[i] = Int32Util.ClampToByte((int)Math.Round(i * blueGain));
            }

            return new ColorMap(redMap, greenMap, blueMap);
        }

        protected override void OnRender(Rectangle[] renderRects, int startIndex, int length)
        {
            if (length == 0)
                return;

            for (var index = startIndex; index < startIndex + length; ++index)
                Render(DstArgs.Surface, SrcArgs.Surface, renderRects[index], _colorMap);
        }

        private void Render(Surface dst, Surface src, Rectangle rect, ColorMap computeColorMap)
        {
            for (var top = rect.Top; top < rect.Bottom; ++top)
            {
                for (var left = rect.Left; left < rect.Right; ++left)
                {
                    var colorBgra = src[left, top];
                    colorBgra.R = computeColorMap.Red[colorBgra.R];
                    colorBgra.G = computeColorMap.Green[colorBgra.G];
                    colorBgra.B = computeColorMap.Blue[colorBgra.B];
                    dst[left, top] = colorBgra;
                }
            }
        }
    }
}
