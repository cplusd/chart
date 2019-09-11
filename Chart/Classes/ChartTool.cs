using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;
using System.Collections.Generic;
using CoreText;
using Foundation;
using CoreFoundation;

namespace Chart
{
    public class ChartTool
    {
        //取图片颜色
        public static byte[] GetColor(UIImage image, nfloat x, nfloat y)
        {
            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
            byte[] da = new byte[] { 0, 0, 0, 0 };
            nint width = (nint)image.Size.Width;
            nint height = (nint)image.Size.Height;
            CGContext context = new CGBitmapContext(da, 1, 1, 8, 4, colorSpace, CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);
            context.SetBlendMode(CGBlendMode.Copy);
            context.TranslateCTM(-x, y - height);
            context.DrawImage(new CGRect(0, 0, width, height), image.CGImage);
            Console.WriteLine(da[0] + " ," + da[1] + " ," + da[2] + " ," + da[3]);
            return da;
        }

        public static UIBezierPath GetStringPath(string str)
        {
            CGPath letters = new CGPath();
            CTFont font = new CTFont("Helvetica-Bold", 32);
            CTStringAttributes attrs = new CTStringAttributes();
            attrs.Font = font;
            NSAttributedString attrString = new NSAttributedString(str, attrs);
            //根据字符串创建 line
            CTLine line = new CTLine(attrString);
            //获取每一个字符作为数组
            CTRun[] runArray = line.GetGlyphRuns();
            // 遍历字符数组
            int runCount = runArray.Length;
            for (int i = 0; i < runCount; i++)
            {
                CTRun run = runArray[i];
                CTFont runFont = run.GetAttributes().Font;
                int glyphCount = (int)run.GlyphCount;
                for (int j = 0; j < glyphCount; j++)
                {
                    var k = run.GetGlyphs(new NSRange(j, 1));
                    var position = run.GetPositions(new NSRange(j, 1));
                    CGPath runPath = runFont.GetPathForGlyph(k[0]);
                    if (runPath != null)
                    {
                        CGAffineTransform t = CGAffineTransform.MakeTranslation(position[0].X, position[0].Y);
                        //CGAffineTransform tt=CGAffineTransform.Rotate(t,(nfloat)Math.PI);



                        letters.AddPath(t, runPath);
                    }
                }




            }
            UIBezierPath resultPath = new UIBezierPath();
            resultPath.MoveTo(CGPoint.Empty);
            resultPath.AppendPath(UIBezierPath.FromPath(letters));
            return resultPath;
        }

    }
    public class PieModel
    {
        public UIColor StrokeColor { get; set; }
        public UIColor FillColor { get; set; }
        public nfloat LineWidth { get; set; }
        public nfloat ItemValue { get; set; }
        public nfloat StartAngle { get; set; }
        public nfloat EndAngle { get; set; }
    }
    public class OneDataModel
    {
        public List<nfloat> Points { get; set; }
        public UIColor StrokeColor { get; set; }
        public UIColor FillColor { get; set; }
        public nfloat LineWidth { get; set; }
        public CGColor[] GradientColor { get; set; }

    }

    public class ColumModel
    {
        public UIColor StrokeColor { get; set; }
        public UIColor FillColor { get; set; }
        public nfloat Value { get; set; }

    }

    #region A
    #region ---背景
    public class CrossLinesView : UIView
    {
        public const float duration = 0.7f;
        public readonly CGColor strokeColor = UIColor.FromRGB(230, 230, 230).CGColor;
        nfloat lineWidth = 1;
        UIEdgeInsets inset;
        nfloat xStep;
        nfloat yStep;
        int xLines = 5; //x 轴上的分隔线
        int yLines = 8;
        public nfloat drawAreaX, drawAreaY, drawAreaWidth, drawAreaHeight;
        //int type;
        bool showLines_parallel_x = true;  //平行于x轴的线
        bool showLines_parallel_y = true; //平行于y轴的线
        public CrossLinesView(CGRect _frame, UIEdgeInsets _inset, bool _showLines_parallel_x = true, bool _showLines_parallel_y = true)
        {
            showLines_parallel_x = _showLines_parallel_x;
            showLines_parallel_y = _showLines_parallel_y;
            //type = _type;
            this.Frame = _frame;
            this.BackgroundColor = UIColor.White;
            inset = _inset;
            drawAreaX = inset.Left;
            drawAreaY = inset.Top;
            drawAreaWidth = this.Frame.Width - drawAreaX - inset.Right;
            drawAreaHeight = this.Frame.Height - drawAreaY - inset.Bottom;
            xStep = drawAreaWidth / xLines;
            yStep = drawAreaHeight / yLines;
        }
        //for cur line 
        public CAKeyFrameAnimation GetKeyFrameAnimation(List<CGPoint> points, UIBezierPath curPath, bool fromPathNeedClose = false, string keyPath = "path")
        {
            UIBezierPath p0 = new UIBezierPath();
            int c = points.Count;
            for (int i = 0; i < c; i++)
            {
                if (i == 0)
                    p0.MoveTo(new CGPoint(points[i].X, drawAreaY + drawAreaHeight));
                else
                {
                    p0.AddLineTo(new CGPoint(points[i].X, drawAreaY + drawAreaHeight));
                }
            }
            if (fromPathNeedClose)
            {


                p0.AddLineTo(new CGPoint(points[c - 1].X, drawAreaY + drawAreaHeight - 1));
                p0.AddLineTo(new CGPoint(points[0].X, drawAreaY + drawAreaHeight - 1));

                p0.ClosePath();
            }

            CAKeyFrameAnimation kf = CAKeyFrameAnimation.FromKeyPath(keyPath);
            kf.Values = new NSObject[] {
                NSObject.FromObject(p0.CGPath),
                NSObject.FromObject(curPath.CGPath)
                };
            kf.KeyTimes = new NSNumber[] {
                new NSNumber(0f),new NSNumber(1f)
                };
            kf.Duration = duration;
            kf.RepeatCount = 1;
            kf.AutoReverses = false;
            kf.TimingFunction = new CAMediaTimingFunction(0.2f, 0.03f, 0.13f, 1.0f);
            return kf;
        }

        //public CAKeyFrameAnimation

        public List<CGPoint> GetPoint(OneDataModel source, float MultiMax = 0.0f)
        {
            var res = new List<CGPoint>();
            //获取x方向间距
            nfloat point_space;
            if (source.Points.Count > 1)
            {
                point_space = drawAreaWidth / (source.Points.Count - 1);
            }
            else
            {
                point_space = drawAreaWidth;
            }
            //找到数据点钟最大值 0~max ;最大值对应坐标轴中最顶部。
            nfloat maxy = 0f;
            if (MultiMax > 0)
            {
                maxy = MultiMax;
            }
            else
            {
                maxy = GetMaxY(source);
            }


            nfloat a = drawAreaHeight / maxy;
            //
            for (int i = 0, c = source.Points.Count; i < c; i++)
            {
                var px = point_space * i + drawAreaX;
                var k = source.Points[i];
                var l = a * k;
                var py = drawAreaY + drawAreaHeight - l;
                var cgpoint = new CGPoint(px, py);
                res.Add(cgpoint);
            }
            return res;
        }
        public nfloat GetMaxY(OneDataModel oneData)
        {
            var dataItems = oneData.Points;
            nfloat max = 0;
            for (int j = 0; j < dataItems.Count; j++)
            {
                max = max > dataItems[j] ? max : dataItems[j];
            }
            return max;
        }
        public nfloat GetMaxY(List<OneDataModel> dataItems)
        {
            nfloat max = 0;
            for (int j = 0; j < dataItems.Count; j++)
            {
                var yPoints = dataItems[j].Points;
                int c = yPoints.Count;
                for (int i = 0; i < c; i++)
                {
                    max = max > yPoints[i] ? max : yPoints[i];
                }
            }
            return max;
        }
        public override void Draw(CGRect rect)
        {
            CGContext context = UIGraphics.GetCurrentContext();
            context.SetStrokeColor(strokeColor);
            context.SetLineWidth(lineWidth);

            if (showLines_parallel_x && showLines_parallel_y)
            {
                //绘制平行于y轴的线
                for (int i = 0; i <= xLines; i++)
                {
                    nfloat tmpx = xStep * i + drawAreaX;
                    nfloat tmpy = drawAreaY;
                    context.MoveTo(tmpx, tmpy);
                    context.AddLineToPoint(tmpx, tmpy + drawAreaHeight);
                }
                //绘制平行于x轴的线
                for (int i = 0; i <= yLines; i++)
                {
                    nfloat tmpx = drawAreaX;
                    nfloat tmpy = drawAreaHeight + drawAreaY - i * yStep;
                    context.MoveTo(tmpx, tmpy);
                    context.AddLineToPoint(tmpx + drawAreaWidth, tmpy);
                }
            }
            else if (showLines_parallel_x)
            {
                //绘制平行于x轴的线
                for (int i = 0; i <= yLines; i++)
                {
                    nfloat tmpx = drawAreaX;
                    nfloat tmpy = drawAreaHeight + drawAreaY - i * yStep;
                    context.MoveTo(tmpx, tmpy);
                    context.AddLineToPoint(tmpx + drawAreaWidth, tmpy);
                }
            }
            else if (showLines_parallel_y)
            {
                //绘制平行于y轴的线
                for (int i = 0; i <= xLines; i++)
                {
                    nfloat tmpx = xStep * i + drawAreaX;
                    nfloat tmpy = drawAreaY;
                    context.MoveTo(tmpx, tmpy);
                    context.AddLineToPoint(tmpx, tmpy + drawAreaHeight);
                }
            }
            else
            {

            }
            context.StrokePath();


        }


    }
    #endregion

    #region ---简单1线

    public class BaseLineChart : CrossLinesView
    {
        bool needAnimation;
        OneDataModel Source;
        static UIEdgeInsets edgeInset = new UIEdgeInsets(15, 40, 40, 15);
        public BaseLineChart(CGRect _frame, OneDataModel _Source, bool _needAnimation = true) : base(_frame, edgeInset)
        {
            needAnimation = _needAnimation;
            if (_Source != null && _Source.Points.Count > 0)
            {
                Source = _Source;
                InitSubLayers();
            }

        }

        private void InitSubLayers()
        {

            UIBezierPath curPath = new UIBezierPath();
            List<CGPoint> points = GetPoint(Source);
            for (int i = 0, c = points.Count; i < c; i++)
            {
                if (i == 0)
                {
                    curPath.MoveTo(points[i]);
                }
                else
                {
                    var lastP = points[i - 1];
                    var currentP = points[i];
                    var controlP1 = new CGPoint((lastP.X + currentP.X) / 2f, lastP.Y);
                    var controlP2 = new CGPoint((lastP.X + currentP.X) / 2f, currentP.Y);
                    curPath.AddCurveToPoint(currentP, controlP1, controlP2);
                }
            }

            CAShapeLayer curLayer = new CAShapeLayer
            {
                FillColor = UIColor.Clear.CGColor,
                Path = curPath.CGPath,
                LineWidth = Source.LineWidth,
                StrokeColor = Source.StrokeColor.CGColor
            };

            this.Layer.AddSublayer(curLayer);

            if (needAnimation)
            {
                var kf = GetKeyFrameAnimation(points, curPath);
                curLayer.AddAnimation(kf, "kf");
            }



        }





    }


    #endregion

    #region---简单多条线
    public class MultiLinesChart : CrossLinesView
    {
        bool needAnimation;
        nfloat MaxYValue = 0;
        static UIEdgeInsets edgeInset = new UIEdgeInsets(15, 40, 40, 15);
        public MultiLinesChart(CGRect _frame, List<OneDataModel> dataItems, bool _needAnimation = true) : base(_frame, edgeInset)
        {
            needAnimation = _needAnimation;
            MaxYValue = GetMaxY(dataItems);
            for (int i = 0; i < dataItems.Count; i++)
            {
                InitSubLayers(dataItems[i]);
            }
        }
        void InitSubLayers(OneDataModel oneData)
        {

            UIBezierPath curPath = new UIBezierPath();
            List<CGPoint> points = GetPoint(oneData, (float)MaxYValue);
            for (int i = 0, c = points.Count; i < c; i++)
            {
                if (i == 0)
                {
                    curPath.MoveTo(points[i]);
                }
                else
                {
                    var lastP = points[i - 1];
                    var currentP = points[i];
                    var controlP1 = new CGPoint((lastP.X + currentP.X) / 2f, lastP.Y);
                    var controlP2 = new CGPoint((lastP.X + currentP.X) / 2f, currentP.Y);
                    curPath.AddCurveToPoint(currentP, controlP1, controlP2);
                }
            }

            CAShapeLayer curLayer = new CAShapeLayer
            {
                FillColor = UIColor.Clear.CGColor,
                Path = curPath.CGPath,
                LineWidth = oneData.LineWidth,
                StrokeColor = oneData.StrokeColor.CGColor
            };
            this.Layer.AddSublayer(curLayer);
            if (needAnimation)
            {
                var kf = GetKeyFrameAnimation(points, curPath);
                curLayer.AddAnimation(kf, "kf");
            }




        }

    }

    #endregion

    #region ---简单1线+fillcolor
    public class BaseLineChart_fill : CrossLinesView
    {
        OneDataModel Source;
        bool needAnimation;
        static UIEdgeInsets edgeInset = new UIEdgeInsets(15, 40, 40, 15);
        public BaseLineChart_fill(CGRect _frame, OneDataModel _Source, bool _needAnimation = true) : base(_frame, edgeInset)
        {
            needAnimation = _needAnimation;
            if (_Source != null && _Source.Points.Count > 0)
            {
                Source = _Source;
                InitSubLayers();

            }

        }

        private void InitSubLayers()
        {
            UIBezierPath curPath = new UIBezierPath();
            List<CGPoint> points = GetPoint(Source);
            for (int i = 0, c = points.Count; i < c; i++)
            {
                if (i == 0)
                {
                    curPath.MoveTo(points[i]);
                }
                else
                {
                    var lastP = points[i - 1];
                    var currentP = points[i];
                    var controlP1 = new CGPoint((lastP.X + currentP.X) / 2f, lastP.Y);
                    var controlP2 = new CGPoint((lastP.X + currentP.X) / 2f, currentP.Y);
                    curPath.AddCurveToPoint(currentP, controlP1, controlP2);
                }
            }

            CAShapeLayer curLayer = new CAShapeLayer
            {
                FillColor = UIColor.Clear.CGColor,
                Path = curPath.CGPath,
                LineWidth = Source.LineWidth,
                StrokeColor = Source.StrokeColor.CGColor
            };
            this.Layer.AddSublayer(curLayer);
            if (needAnimation)
            {
                var kf = GetKeyFrameAnimation(points, curPath);
                curLayer.AddAnimation(kf, "kf");
            }

            AddGradientLayer(curPath, points);
        }

        void AddGradientLayer(UIBezierPath _cgpath, List<CGPoint> points)
        {
            CAGradientLayer gradientLayer = new CAGradientLayer();
            gradientLayer.Frame = new CGRect(0, 0, this.Frame.Width, this.Frame.Height);
            gradientLayer.Colors = Source.GradientColor;
            gradientLayer.StartPoint = new CGPoint(0.5f, 0);
            gradientLayer.EndPoint = new CGPoint(0.5f, 1.1f);
            //mask
            _cgpath.AddLineTo(new CGPoint(drawAreaX + drawAreaWidth, drawAreaY + drawAreaHeight));
            _cgpath.AddLineTo(new CGPoint(drawAreaX, drawAreaY + drawAreaHeight));
            _cgpath.ClosePath();

            CAShapeLayer maskLayer = new CAShapeLayer();
            maskLayer.StrokeColor = UIColor.Clear.CGColor;
            maskLayer.FillColor = UIColor.Red.CGColor;
            maskLayer.LineWidth = 3;
            maskLayer.Path = _cgpath.CGPath;
            gradientLayer.Mask = maskLayer;
            if (needAnimation)
            {
                var kf = GetKeyFrameAnimation(points, _cgpath, true);
                maskLayer.AddAnimation(kf, "maskkf");
            }
            this.Layer.AddSublayer(gradientLayer);
        }





    }


    #endregion

    #region ---简单多线+fillcolor

    public class MultiLinesChart_fill : CrossLinesView
    {
        nfloat MaxYValue = 0;
        bool needAnimation;
        static UIEdgeInsets edgeInset = new UIEdgeInsets(15, 40, 40, 15);
        public MultiLinesChart_fill(CGRect _frame, List<OneDataModel> dataItems, bool _needAnimation = true) : base(_frame, edgeInset)
        {
            needAnimation = _needAnimation;
            MaxYValue = GetMaxY(dataItems);
            for (int i = 0; i < dataItems.Count; i++)
            {
                InitSubLayers(dataItems[i]);
            }
        }
        void InitSubLayers(OneDataModel oneData)
        {

            UIBezierPath curPath = new UIBezierPath();
            List<CGPoint> points = GetPoint(oneData, (float)MaxYValue);
            for (int i = 0, c = points.Count; i < c; i++)
            {
                if (i == 0)
                {
                    curPath.MoveTo(points[i]);
                }
                else
                {
                    var lastP = points[i - 1];
                    var currentP = points[i];
                    var controlP1 = new CGPoint((lastP.X + currentP.X) / 2f, lastP.Y);
                    var controlP2 = new CGPoint((lastP.X + currentP.X) / 2f, currentP.Y);
                    curPath.AddCurveToPoint(currentP, controlP1, controlP2);
                }
            }

            CAShapeLayer curLayer = new CAShapeLayer
            {
                FillColor = UIColor.Clear.CGColor,
                Path = curPath.CGPath,
                LineWidth = oneData.LineWidth,
                StrokeColor = oneData.StrokeColor.CGColor
            };
            this.Layer.AddSublayer(curLayer);
            if (needAnimation)
            {
                var kf = GetKeyFrameAnimation(points, curPath);
                curLayer.AddAnimation(kf, "kf");
            }

            AddGradientLayer(curPath, oneData, points);
        }
        void AddGradientLayer(UIBezierPath _cgpath, OneDataModel Source, List<CGPoint> points)
        {
            CAGradientLayer gradientLayer = new CAGradientLayer();
            gradientLayer.Frame = new CGRect(0, 0, this.Frame.Width, this.Frame.Height);
            gradientLayer.Colors = Source.GradientColor;
            gradientLayer.StartPoint = new CGPoint(0.5f, 0);
            gradientLayer.EndPoint = new CGPoint(0.5f, 1.1f);
            //mask
            _cgpath.AddLineTo(new CGPoint(drawAreaX + drawAreaWidth, drawAreaY + drawAreaHeight));
            _cgpath.AddLineTo(new CGPoint(drawAreaX, drawAreaY + drawAreaHeight));
            _cgpath.ClosePath();

            CAShapeLayer maskLayer = new CAShapeLayer();
            maskLayer.StrokeColor = UIColor.Clear.CGColor;
            maskLayer.FillColor = UIColor.Red.CGColor;
            maskLayer.LineWidth = 3;
            maskLayer.Path = _cgpath.CGPath;
            if (needAnimation)
            {
                var kf = GetKeyFrameAnimation(points, _cgpath, true);
                maskLayer.AddAnimation(kf, "maskkf");
            }
            gradientLayer.Mask = maskLayer;
            this.Layer.AddSublayer(gradientLayer);
        }


    }
    #endregion

    #region ---pie chart
    public class PieChart : CrossLinesView
    {
        static UIEdgeInsets edgeInset = new UIEdgeInsets(15, 40, 15, 15);
        //CGRect _frame, List<OneDataModel> dataItems,

        bool needAnimation;
        CGPoint center_pie;//中心坐标
        nfloat radius;//半径
        bool isRing;
        public PieChart(CGRect _frame, List<PieModel> dataItems, bool _needAnimation = true, bool _isRing = false) : base(_frame, edgeInset)
        {
            isRing = _isRing;
            needAnimation = _needAnimation;
            if (dataItems != null && dataItems.Count > 0)
            {
                if (drawAreaWidth > drawAreaHeight)
                {
                    var larger = drawAreaWidth - drawAreaHeight;
                    radius = drawAreaHeight / 2f;
                    var cx = drawAreaX + radius + larger / 2f;
                    var cy = drawAreaY + radius;
                    center_pie = new CGPoint(cx, cy);
                }
                else
                {
                    var larger = drawAreaHeight - drawAreaWidth;
                    radius = drawAreaWidth / 2f;
                    var cx = drawAreaX + radius;
                    var cy = drawAreaY + radius + larger;
                    center_pie = new CGPoint(cx, cy);
                }

            }
            //
            DrawPieSections(dataItems);

        }
        void DrawPieSections(List<PieModel> _scetions)
        {
            nfloat totalValues = 0;
            for (int i = 0; i < _scetions.Count; i++)
            {
                totalValues += _scetions[i].ItemValue;
            }


            nfloat per = (nfloat)(Math.PI * 2f / totalValues);
            for (int i = 0; i < _scetions.Count; i++)
            {
                var item = _scetions[i];
                if (i == 0)
                {
                    item.StartAngle = -(nfloat)Math.PI / 2f;
                    item.EndAngle = item.StartAngle + item.ItemValue * per;

                }
                else
                {
                    var lastOne = _scetions[i - 1];
                    item.StartAngle = lastOne.EndAngle;
                    item.EndAngle = item.StartAngle + item.ItemValue * per;
                }
                DrawPieLayer(item);

            }

        }

        void DrawPieLayer(PieModel pieData)
        {
            CAShapeLayer pie = new CAShapeLayer();
            pie.FillColor = pieData.FillColor.CGColor;
            pie.StrokeColor = pieData.StrokeColor.CGColor;
            pie.LineWidth = pieData.LineWidth;
            UIBezierPath path = new UIBezierPath();
            path.MoveTo(center_pie);
            path.AddArc(center_pie, radius, pieData.StartAngle, pieData.EndAngle, true);
            path.AddLineTo(center_pie);
            path.ClosePath();
            pie.Path = path.CGPath;
            this.Layer.AddSublayer(pie);


            //由小到大动画
            if (needAnimation)
            {
                //有小到大的动画
                //pie.AddAnimation(PieKeyFrameAnimation(pieData, path), "pie");
                //旋转绘制动画
                AddStrokeEndAnimation(pieData, pie);

            }



        }

        void AddStrokeEndAnimation(PieModel pieData, CAShapeLayer pie)
        {
            UIBezierPath maskP = UIBezierPath.FromArc(center_pie, radius, pieData.StartAngle, pieData.EndAngle, true);
            CAShapeLayer maskL = new CAShapeLayer();
            if (isRing)
            {
                maskL.LineWidth = radius;
            }
            else
            {
                maskL.LineWidth = radius + radius;
            }

            maskL.StrokeColor = UIColor.Red.CGColor;
            maskL.FillColor = UIColor.Clear.CGColor;
            maskL.Path = maskP.CGPath;
            maskL.StrokeEnd = 0;

            pie.Mask = maskL;


            CABasicAnimation animation = CABasicAnimation.FromKeyPath("strokeEnd");
            animation.From = NSNumber.FromNInt(0);
            animation.To = NSNumber.FromInt16(1);
            animation.RemovedOnCompletion = false;
            animation.FillMode = CAFillMode.Forwards;
            animation.Duration = duration;
            maskL.AddAnimation(animation, "ma");
        }


        //放大效果。
        public CAKeyFrameAnimation PieKeyFrameAnimation(PieModel pieData, UIBezierPath topath)
        {
            UIBezierPath path = new UIBezierPath();
            path.MoveTo(center_pie);
            path.AddArc(center_pie, 1, pieData.StartAngle, pieData.EndAngle, true);//ok
            path.AddLineTo(center_pie);
            path.ClosePath();
            CAKeyFrameAnimation kf = CAKeyFrameAnimation.FromKeyPath("path"); //ok
            kf.Values = new NSObject[] {
                NSObject.FromObject(path.CGPath),
                NSObject.FromObject(topath.CGPath)
                };
            kf.KeyTimes = new NSNumber[] {
                new NSNumber(0f),new NSNumber(1f)
                };
            kf.Duration = duration;
            kf.RepeatCount = 1;
            kf.AutoReverses = false;
            kf.TimingFunction = new CAMediaTimingFunction(0.2f, 0.03f, 0.13f, 1.0f);
            return kf;
        }


    }
    #endregion

    #region---column Chart
    public class ColumnChart : CrossLinesView
    {
        nfloat margin = 15;
        static UIEdgeInsets edgeInset = new UIEdgeInsets(15, 40, 15, 15);
        List<ColumModel> Souce;
        bool animated;
        public ColumnChart(CGRect _frame, List<ColumModel> _souce, bool _animated = true) : base(_frame, edgeInset, true, false)
        {
            animated = _animated;
            Souce = _souce;
            AddColumns();
        }

        void AddColumns()
        {
            nfloat maxValue = GetMax(Souce);
            int totalItemsOfOneGroup = Souce.Count;
            var itemWidth = (drawAreaWidth - margin * (totalItemsOfOneGroup + 1)) / totalItemsOfOneGroup;

            var a = drawAreaHeight / maxValue;

            for (int i = 0; i < totalItemsOfOneGroup; i++)
            {
                var x = margin * (i + 1) + i * itemWidth + drawAreaX;
                var y = drawAreaY + drawAreaHeight - a * Souce[i].Value;
                AddOneColumnLayer(x, y, itemWidth, Souce[i]);
            }


        }

        nfloat GetMax(List<ColumModel> _Souce)
        {

            nfloat max = 0;
            for (int j = 0; j < _Souce.Count; j++)
            {
                max = max > _Souce[j].Value ? max : _Souce[j].Value;
            }
            return max;

        }
        void AddOneColumnLayer(nfloat x, nfloat y, nfloat itemWidth, ColumModel item)
        {
            UIBezierPath path = new UIBezierPath();
            path.MoveTo(new CGPoint(x, drawAreaY + drawAreaHeight));
            path.AddLineTo(new CGPoint(x + itemWidth, drawAreaY + drawAreaHeight));
            path.AddLineTo(new CGPoint(x + itemWidth, y));
            path.AddLineTo(new CGPoint(x, y));
            path.ClosePath();


            CAShapeLayer columnLayer = new CAShapeLayer
            {
                FillColor = item.FillColor.CGColor,
                Path = path.CGPath,
                LineWidth = 1,
                StrokeColor = item.StrokeColor.CGColor
            };
            if (animated)
            {
                UIBezierPath p0 = new UIBezierPath();
                p0.MoveTo(new CGPoint(x, drawAreaY + drawAreaHeight));
                p0.AddLineTo(new CGPoint(x + itemWidth, drawAreaY + drawAreaHeight));
                p0.AddLineTo(new CGPoint(x + itemWidth, drawAreaY + drawAreaHeight - 1));
                p0.AddLineTo(new CGPoint(x, drawAreaY + drawAreaHeight - 1));
                p0.ClosePath();

                CAKeyFrameAnimation kf = CAKeyFrameAnimation.FromKeyPath("path");
                kf.Values = new NSObject[] {
                NSObject.FromObject(p0.CGPath),
                NSObject.FromObject(path.CGPath)
                };
                kf.KeyTimes = new NSNumber[] {
                new NSNumber(0f),new NSNumber(1f)
                };
                kf.Duration = duration;
                kf.RepeatCount = 1;
                kf.AutoReverses = false;
                kf.TimingFunction = new CAMediaTimingFunction(0.2f, 0.03f, 0.13f, 1.0f);
                columnLayer.AddAnimation(kf, "ckf");
            }
            this.Layer.AddSublayer(columnLayer);






        }

    }

    #endregion

    #region --Radar chart

    public class RadarChart : CrossLinesView
    {
        static UIEdgeInsets edgeInset = new UIEdgeInsets(15, 40, 15, 15);
        List<OneDataModel> Source;
        int num; //雷达分支数
        double angle;//= Math.PI * 2d / num;
        bool needAnimation = true;
        public RadarChart(CGRect _frame, List<OneDataModel> _source, bool _needAnimation = true) : base(_frame, edgeInset, true, false)
        {
            needAnimation = _needAnimation;
            Source = _source;
            if (Source == null || Source.Count == 0) return;
            if (Source[0].Points.Count <= 2) return;
            num = Source[0].Points.Count;
            angle = Math.PI * 2d / num;
            Addbglines();
            //duo ci
            AddFilledLayers();
        }

        private void AddFilledLayers()
        {
            nfloat max = GetMaxY(Source);
            var pice = r / max;
            for (int i = 0, c = Source.Count; i < c; i++)
            {
                var data_item = Source[i];
                CAShapeLayer radarLayer = new CAShapeLayer();
                radarLayer.FillColor = data_item.FillColor.CGColor;
                radarLayer.LineWidth = data_item.LineWidth;
                radarLayer.StrokeColor = data_item.StrokeColor.CGColor;
                UIBezierPath radarPath = new UIBezierPath();
                for (int j = 0, m = data_item.Points.Count; j < m; j++)
                {
                    var length = pice * data_item.Points[j];
                    var currentAngle = angle * j;
                    var h = Math.Sin(currentAngle) * length;
                    var _x = centX + (nfloat)h;
                    var t = length * Math.Cos(currentAngle);
                    var _y = centY - (nfloat)t;
                    if (j == 0)
                    {
                        radarPath.MoveTo(new CGPoint(_x, _y));
                    }
                    else
                    {
                        radarPath.AddLineTo(new CGPoint(_x, _y));
                    }
                }
                radarPath.ClosePath();
                radarLayer.Path = radarPath.CGPath;
                this.Layer.AddSublayer(radarLayer);
                if (needAnimation)
                {
                    CABasicAnimation animation = CABasicAnimation.FromKeyPath("path");
                    UIBezierPath p0 = new UIBezierPath();
                    p0.AddArc(new CGPoint(centX, centY), 1, (nfloat)(-Math.PI / 2d), (nfloat)(Math.PI * 3 / 2d), true);
                    p0.ClosePath();
                    animation.From = NSObject.FromObject(p0.CGPath);
                    animation.To = NSObject.FromObject(radarPath.CGPath);
                    animation.Duration = duration;
                    animation.RemovedOnCompletion = false;
                    animation.FillMode = CAFillMode.Forwards;
                    animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
                    radarLayer.AddAnimation(animation, "radarAnimation");
                }
            }



        }



        nfloat r = 0, centX = 0, centY = 0;
        void Addbglines()
        {

            //半径和圆心           
            if (drawAreaWidth >= drawAreaHeight)
            {
                r = drawAreaHeight / 2f;
                centX = drawAreaX + drawAreaWidth / 2f;
                centY = drawAreaHeight / 2f + drawAreaY;
            }
            else
            {
                r = drawAreaWidth / 2f;
                centX = drawAreaX + drawAreaWidth / 2f;
                centY = drawAreaHeight / 2f + drawAreaY;
            }
            // 根据数据个数绘制点

            int levels = 5;
            nfloat level_space = r / levels;


            CAShapeLayer linesLayer = new CAShapeLayer();
            linesLayer.FillColor = UIColor.Clear.CGColor;
            linesLayer.StrokeColor = strokeColor;
            linesLayer.LineWidth = 1;

            UIBezierPath path = new UIBezierPath();
            UIBezierPath path4 = new UIBezierPath(); //<path5
            UIBezierPath path3 = new UIBezierPath(); //<path4
            UIBezierPath path2 = new UIBezierPath(); //<path3
            UIBezierPath path1 = new UIBezierPath(); //<path2

            List<CGPoint> points = new List<CGPoint>();
            for (int i = 0; i < num; i++)
            {
                nfloat vx;
                nfloat vy;
                var currentAngle = angle * i;
                var h = Math.Sin(currentAngle) * r;
                vx = centX + (nfloat)h;
                var t = r * Math.Cos(currentAngle);
                vy = centY - (nfloat)t;
                if (i == 0)
                {
                    path.MoveTo(new CGPoint(vx, vy));
                    path4.MoveTo(new CGPoint(vx, vy + level_space));
                    path3.MoveTo(new CGPoint(vx, vy + level_space * 2));
                    path2.MoveTo(new CGPoint(vx, vy + level_space * 3));
                    path1.MoveTo(new CGPoint(vx, vy + level_space * 4));

                }
                else
                {
                    //path最外层
                    path.AddLineTo(new CGPoint(vx, vy));
                    //path4
                    var vx4 = centX + (nfloat)(Math.Sin(currentAngle) * (r - level_space));
                    var vy4 = centY - (nfloat)((r - level_space) * Math.Cos(currentAngle));
                    path4.AddLineTo(new CGPoint(vx4, vy4));
                    //path3
                    var vx3 = centX + (nfloat)(Math.Sin(currentAngle) * (r - 2 * level_space));
                    var vy3 = centY - (nfloat)((r - 2 * level_space) * Math.Cos(currentAngle));
                    path3.AddLineTo(new CGPoint(vx3, vy3));
                    //path2
                    var vx2 = centX + (nfloat)(Math.Sin(currentAngle) * (r - 3 * level_space));
                    var vy2 = centY - (nfloat)((r - 3 * level_space) * Math.Cos(currentAngle));
                    path2.AddLineTo(new CGPoint(vx2, vy2));
                    //path1最内层
                    var vx1 = centX + (nfloat)(Math.Sin(currentAngle) * (r - 4 * level_space));
                    var vy1 = centY - (nfloat)((r - 4 * level_space) * Math.Cos(currentAngle));
                    path1.AddLineTo(new CGPoint(vx1, vy1));

                }
                points.Add(new CGPoint(vx, vy));


            }
            path.ClosePath();
            path4.ClosePath();
            path3.ClosePath();
            path2.ClosePath();
            path1.ClosePath();

            path.CGPath.AddPath(path4.CGPath);
            path.CGPath.AddPath(path3.CGPath);
            path.CGPath.AddPath(path2.CGPath);
            path.CGPath.AddPath(path1.CGPath);

            UIBezierPath fsLine = new UIBezierPath();
            for (int i = 0; i < points.Count; i++)
            {
                fsLine.MoveTo(new CGPoint(centX, centY));
                fsLine.AddLineTo(new CGPoint(points[i].X, points[i].Y));
                path.CGPath.AddPath(fsLine.CGPath);
            }


            linesLayer.Path = path.CGPath;
            this.Layer.AddSublayer(linesLayer);


        }



    }

    #endregion


    #endregion


    #region B
    public class ShowPModel
    {
        public UIColor StrokeColor { get; set; }
        public UIColor FillColor { get; set; }
        public List<XYValueModel> Points { get; set; }
    }
    public class XYValueModel
    {
        public nfloat X { get; set; }
        public nfloat Y { get; set; }
        public nfloat Value { get; set; }
    }
    public class CenterAxisView : UIView
    {
        public const float duration = 0.7f;
        //中间坐标轴
        UIColor XYAxisColor = UIColor.FromRGB(21, 139, 202);
        nfloat XYLineWidth = 1.5f;
        //
        public nfloat drawAreaX, drawAreaY, drawAreaWidth, drawAreaHeight;
        UIEdgeInsets inset;
        public CenterAxisView(CGRect _frame, UIEdgeInsets _inset)
        {
            this.Frame = _frame;
            this.BackgroundColor = UIColor.White;
            inset = _inset;
            drawAreaX = inset.Left;
            drawAreaY = inset.Top;
            drawAreaWidth = this.Frame.Width - drawAreaX - inset.Right;
            drawAreaHeight = this.Frame.Height - drawAreaY - inset.Bottom;
        }

        public readonly CGColor strokeColor = UIColor.FromRGB(230, 230, 230).CGColor;
        public override void Draw(CGRect rect)
        {
            CGContext context = UIGraphics.GetCurrentContext();
            //中间x轴，y轴
            context.SetStrokeColor(XYAxisColor.CGColor);
            context.SetLineWidth(XYLineWidth);
            CGPoint x_left = new CGPoint(drawAreaX, drawAreaY + drawAreaHeight / 2f);
            CGPoint x_right = new CGPoint(drawAreaX + drawAreaWidth, drawAreaY + drawAreaHeight / 2f);
            CGPoint y_top = new CGPoint(drawAreaX + drawAreaWidth / 2f, drawAreaY);
            CGPoint y_bottom = new CGPoint(drawAreaX + drawAreaWidth / 2f, drawAreaY + drawAreaHeight);
            context.MoveTo(x_left.X, x_left.Y);
            context.AddLineToPoint(x_right.X, x_right.Y);
            context.StrokePath();
            context.MoveTo(y_top.X, y_top.Y);
            context.AddLineToPoint(y_bottom.X, y_bottom.Y);
            context.StrokePath();
            //border
            context.SetLineWidth(1);
            context.SetStrokeColor(strokeColor);
            UIBezierPath borderPath = new UIBezierPath();
            borderPath.MoveTo(new CGPoint(drawAreaX, drawAreaY));
            borderPath.AddLineTo(new CGPoint(drawAreaX + drawAreaWidth, drawAreaY));
            borderPath.AddLineTo(new CGPoint(drawAreaX + drawAreaWidth, drawAreaY + drawAreaHeight));
            borderPath.AddLineTo(new CGPoint(drawAreaX, drawAreaY + drawAreaHeight));
            borderPath.ClosePath();
            borderPath.Stroke();

        }
    }
    public class PointsChart : CenterAxisView
    {
        static UIEdgeInsets edge = new UIEdgeInsets(20, 40, 40, 20);
        List<ShowPModel> Allsource;
        bool needAnimation;
        public PointsChart(CGRect _frame, List<ShowPModel> _source, bool _animated = true) : base(_frame, edge)
        {
            Allsource = _source;
            needAnimation = _animated;
            for (int i = 0; i < Allsource.Count; i++)
            {
                AddPoints(Allsource[i]);
            }
        }
        nfloat MaxPointRadius = 20;
        nfloat maxValue = 0f;
        nfloat maxPoint_x = 0f;
        nfloat minPoint_x = 0f;
        nfloat maxPoint_y = 0f;
        nfloat minPoint_y = 0f;
        void AddPoints(ShowPModel source)
        {
            GetMaxSize(ref maxValue, ref maxPoint_x, ref minPoint_x, ref maxPoint_y, ref minPoint_y);
            //
            var x_distance = (nfloat)(Math.Abs(maxPoint_x) >= Math.Abs(minPoint_x) ? Math.Abs(maxPoint_x) * 2 : Math.Abs(minPoint_x) * 2);
            var x_slice = drawAreaWidth / x_distance;
            //
            var y_distance = (nfloat)(Math.Abs(maxPoint_y) >= Math.Abs(minPoint_y) ? Math.Abs(maxPoint_y) * 2 : Math.Abs(minPoint_y) * 2);
            var y_slice = drawAreaHeight / y_distance;
            //

            nfloat drawAreaCenter_x = drawAreaX + drawAreaWidth / 2f;
            nfloat drawAreaCenter_y = drawAreaY + drawAreaHeight / 2f;

            for (int i = 0; i < source.Points.Count; i++)
            {
                var souceItem = source.Points[i];
                //获取当前点的半径
                var currentPointR = souceItem.Value * MaxPointRadius / maxValue;
                //获取点坐标 px，py
                nfloat px = 0f, py = 0f;
                if (souceItem.X >= 0)
                {
                    if (souceItem.Y >= 0)
                    {
                        //第一象限
                        px = drawAreaCenter_x + souceItem.X * x_slice;
                        py = drawAreaCenter_y - souceItem.Y * y_slice;

                    }
                    else
                    {
                        //第二象限
                        px = drawAreaCenter_x + souceItem.X * x_slice;
                        py = drawAreaCenter_y + (nfloat)Math.Abs(souceItem.Y * y_slice);
                    }
                }
                else
                {
                    if (souceItem.Y >= 0)
                    {
                        //第四象限
                        px = drawAreaCenter_x - (nfloat)Math.Abs(souceItem.X * x_slice);
                        py = drawAreaCenter_y - souceItem.Y * y_slice;
                    }
                    else
                    {
                        //第三象限
                        px = drawAreaCenter_x - (nfloat)Math.Abs(souceItem.X * x_slice);
                        py = drawAreaCenter_y + (nfloat)Math.Abs(souceItem.Y * y_slice);
                    }

                }


                CAShapeLayer pointLayer = new CAShapeLayer();
                pointLayer.FillColor = source.FillColor.CGColor;
                pointLayer.StrokeColor = source.StrokeColor.CGColor;
                pointLayer.LineWidth = 1;
                UIBezierPath path = new UIBezierPath();
                path.AddArc(new CGPoint(px, py), currentPointR, (nfloat)(-Math.PI / 2f), (nfloat)(Math.PI * 3 / 2f), true);
                pointLayer.Path = path.CGPath;
                this.Layer.AddSublayer(pointLayer);
                if (needAnimation)
                {
                    CABasicAnimation animation = CABasicAnimation.FromKeyPath("path");
                    UIBezierPath p0 = new UIBezierPath();
                    p0.AddArc(new CGPoint(px, py), 0.1f, (nfloat)(-Math.PI / 2d), (nfloat)(Math.PI * 3 / 2d), true);
                    p0.ClosePath();
                    animation.From = NSObject.FromObject(p0.CGPath);
                    animation.To = NSObject.FromObject(path.CGPath);
                    animation.Duration = duration;
                    animation.RemovedOnCompletion = false;
                    animation.FillMode = CAFillMode.Forwards;
                    animation.TimingFunction = CAMediaTimingFunction.FromName(CAMediaTimingFunction.EaseOut);
                    pointLayer.AddAnimation(animation, "radarAnimation");
                }
            }



        }

        void GetMaxSize(ref nfloat maxValue, ref nfloat maxPoint_x, ref nfloat minPoint_x, ref nfloat maxPoint_y, ref nfloat minPoint_y)
        {
            for (int j = 0; j < Allsource.Count; j++)
            {
                var source = Allsource[j];
                for (int i = 0, c = source.Points.Count; i < c; i++)
                {
                    var dataItems = source.Points[i];
                    maxValue = maxValue > dataItems.Value ? maxValue : dataItems.Value;
                    maxPoint_x = maxPoint_x > dataItems.X ? maxPoint_x : dataItems.X;
                    minPoint_x = minPoint_x < dataItems.X ? minPoint_x : dataItems.X;
                    maxPoint_y = maxPoint_y > dataItems.Y ? maxPoint_y : dataItems.Y;
                    minPoint_y = minPoint_y < dataItems.Y ? minPoint_y : dataItems.Y;
                }
            }



        }
    }

    public class HeatChart : CenterAxisView
    {
        List<XYValueModel> Points;
        static UIEdgeInsets edge = new UIEdgeInsets(20, 40, 40, 20);
        nfloat max_x,min_x, max_y,min_y, max_value,min_value;
        nfloat slice_x,slice_y;
        public HeatChart(CGRect _frame, List<XYValueModel> _Points) : base(_frame, edge)
        {
            Points = _Points;
            GetMaxY(ref max_x,ref min_y,ref max_y,ref min_y,ref max_value,ref min_value);

            slice_x = drawAreaWidth / (max_x - min_x);
            slice_y = drawAreaHeight / (max_y- min_y);

        }
        void GetMaxY(ref nfloat max_x,ref nfloat min_x ,ref nfloat max_y,ref nfloat min_y, ref nfloat max_value,ref nfloat min_value)
        {
       
            for (int j = 0; j < Points.Count; j++)
            {
                max_x = max_x > Points[j].X ? max_x : Points[j].X;
                min_x = min_x < Points[j].X ? min_x : Points[j].X;
                max_y = max_y > Points[j].Y ? max_y : Points[j].Y;
                min_y = min_y < Points[j].Y ? min_y : Points[j].Y;
                max_value = max_value > Points[j].Value ? max_value : Points[j].Value;
                min_value = min_value < Points[j].Value ? min_value : Points[j].Value;
            }

        }



        CGImage dotimg;
        UIImageView imv2;
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
            nint width = (nint)drawAreaWidth;
            nint height = (nint)drawAreaHeight;
            byte[] da = new byte[width * height * 4];
            CGBitmapContext cGContext = new CGBitmapContext(da, width, height, 8, width * 4, colorSpace, CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);
            Random random = new Random(DateTime.Now.Second);
            nfloat max = max_value;
            nfloat min = min_value;
            for (int i = 0,c=Points.Count; i < c; i++)
            {
                var item = Points[i];
                nfloat x = item.X * slice_x;//+drawAreaX;
                nfloat y = item.Y * slice_y;//+drawAreaY;
                nfloat count = item.Value;
                var alpha = (count - min) / (max - min);
                cGContext.SetAlpha(alpha);
                CGGradient _cGGradient = new CGGradient(CGColorSpace.CreateDeviceRGB(), new CGColor[] { UIColor.Black.ColorWithAlpha(0).CGColor, UIColor.Black.ColorWithAlpha(1).CGColor }, new nfloat[] { 1f, 0f });
                cGContext.DrawRadialGradient(_cGGradient, new CGPoint(x, y), 1, new CGPoint(x, y), 10, CGGradientDrawingOptions.DrawsBeforeStartLocation);
            }

            dotimg = cGContext.ToImage();
            imv2 = new UIImageView();
            imv2.Frame = new CGRect(drawAreaX, drawAreaY, dotimg.Width, dotimg.Height);
            imv2.Image = UIImage.FromImage(dotimg);
            this.AddSubview(imv2);

            new System.Threading.Thread(() => {

                CGImage newImg = GetColor2(dotimg);
                if (newImg != null)
                {
                    NSThread.SleepFor(0.9f);
                    InvokeOnMainThread(() => {
                        imv2.Image = UIImage.FromImage(newImg);
                        CATransition trans = CATransition.CreateAnimation();
                        trans.Duration = 1;
                        imv2.Layer.AddAnimation(trans, "at");
                    });
                }
            }).Start();

        }



        //取渐变色rgba；（如果是取图片的需要绘制图片）
        byte[] GetGradientColor()
        {
            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
            nint width = 256;
            nint height = 1;
            byte[] da = new byte[width * height * 4];
            CGBitmapContext context = new CGBitmapContext(da, width, height, 8, width * 4, colorSpace, CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);
            context.SetBlendMode(CGBlendMode.Copy);
            CGGradient _cGGradient = new CGGradient(CGColorSpace.CreateDeviceRGB(), new CGColor[] { UIColor.Blue.CGColor, UIColor.Green.CGColor, UIColor.Yellow.CGColor, UIColor.Red.CGColor }, new nfloat[] { 0.25f, 0.5f, 0.75f, 1f });
            context.DrawLinearGradient(_cGGradient, new CGPoint(0, 0), new CGPoint(width, 0), CGGradientDrawingOptions.DrawsAfterEndLocation);
            return da;
        }


        CGImage GetColor2(CGImage image)
        {
            CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
            nint width = image.Width;
            nint height = image.Height;
            byte[] da = new byte[width * height * 4];
            CGBitmapContext context = new CGBitmapContext(da, width, height, 8, width * 4, colorSpace, CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big);
            context.SetBlendMode(CGBlendMode.Copy);
            context.DrawImage(new CGRect(0, 0, width, height), image);
            byte[] colors = GetGradientColor();
            int length = da.Length;
            for (int i = 3; i < length; i += 4)
            {
                var alpha = da[i];
                var offset = alpha * 4;
                da[i - 3] = colors[offset];
                da[i - 2] = colors[offset + 1];
                da[i - 1] = colors[offset + 2];
            }

            CGDataProvider dataProvider = new CGDataProvider(da);
            CGImage newImg = new CGImage((int)width, (int)height, 8, 32, (int)(4 * width), colorSpace, CGBitmapFlags.PremultipliedLast | CGBitmapFlags.ByteOrder32Big, dataProvider, null, true, CGColorRenderingIntent.Default);
            return newImg;

        }

    }
    #endregion


    #region C

    public class FlyModel {
        public CGPoint StartP { get; set; }
        public CGPoint EndP { get; set; }
        //public CGPoint ControP { get; set; }
    }

    public class FlyChart:UIView
    {
        nfloat duration = 2f;
        List<FlyModel> source;
        nfloat max_x; nfloat min_x;  nfloat max_y;  nfloat min_y;
        public FlyChart(CGRect frame,List<FlyModel>_s)
        {   this.Frame = frame;
            source = _s;
            this.BackgroundColor = UIColor.Black;
            this.Layer.BorderColor = UIColor.Orange.CGColor;
            this.Layer.BorderWidth = 1;

            GetMaxSize(ref max_x, ref min_x, ref max_y, ref min_y);

            nfloat dis_x = max_x - min_x;
            nfloat dis_y = max_y - min_y;
            nfloat slice_x = this.Frame.Width / dis_x;
            nfloat slice_y = this.Frame.Height / dis_y;

            for(int i=0;i<source.Count;i++)
            {
                var item = source[i];
                var _x0 = item.StartP.X * slice_x;
                var _y0 = item.StartP.Y * slice_y;

                var _x1 = item.EndP.X * slice_x;
                var _y1 = item.EndP.Y * slice_y;
                AddLayer(new CGPoint(_x0,_y0),new CGPoint(_x1,_y1));
            }
           
        }
       

        void AddLayer(CGPoint start,CGPoint end)
        {
            start = new CGPoint(this.Frame.Width/2f,this.Frame.Height/1.7f);
            UIBezierPath path = new UIBezierPath();
            path.MoveTo(start);
            path.AddQuadCurveToPoint(end, new CGPoint((start.X+ end.X)/3f,(start.Y+ end.Y)/3f));
           

            CAShapeLayer LineLayer = new CAShapeLayer();
            LineLayer.StrokeColor = UIColor.FromRGB(189,189,82).CGColor;
            LineLayer.FillColor = UIColor.Clear.CGColor;
            LineLayer.LineWidth = 1;
            LineLayer.StrokeStart = 0;
            LineLayer.StrokeEnd = 1;
            LineLayer.Path = path.CGPath;
            Layer.AddSublayer(LineLayer);
            //position animation
            CAKeyFrameAnimation cAKey = CAKeyFrameAnimation.FromKeyPath("position");
            cAKey.Duration = duration;
            cAKey.Path = path.CGPath;          
            //size animation
            CABasicAnimation sizeAnimation = CABasicAnimation.FromKeyPath("transform");
            sizeAnimation.To= NSValue.FromCATransform3D(CATransform3D.MakeScale(0.7f, 0.8f, 0.9f));
            sizeAnimation.From = NSValue.FromCATransform3D(CATransform3D.MakeScale(0.3f, 0.3f, 0.3f));
            sizeAnimation.Duration = duration;           
            //group animation
            CAAnimationGroup group = new CAAnimationGroup();
            group.Duration = duration;
            group.RepeatCount = float.MaxValue;
            group.Animations = new CAAnimation[] {cAKey,sizeAnimation };
            group.RemovedOnCompletion = false;
            group.FillMode = CAFillMode.Forwards;

            CAShapeLayer item = new CAShapeLayer();
            UIBezierPath _p = new UIBezierPath();
            _p.AddArc(new CGPoint(0, 0), 2.2f, 0, (nfloat)Math.PI * 2, true);
            item.FillColor = UIColor.White.ColorWithAlpha(1f).CGColor;
            item.Path = _p.CGPath;
            item.AddAnimation(group, "pos");
            var replicatorLayer = new CAReplicatorLayer();             replicatorLayer.AddSublayer(item);             replicatorLayer.RepeatCount = float.MaxValue;             replicatorLayer.InstanceCount = 25;             replicatorLayer.InstanceDelay = 0.011f;             replicatorLayer.InstanceAlphaOffset = -0.04f;
            
            Layer.AddSublayer(replicatorLayer);

        }


        void GetMaxSize(ref nfloat max_x, ref nfloat min_x, ref nfloat max_y, ref nfloat min_y)
        {
            for (int i = 0, c = source.Count; i < c; i++)
            {
                var d = source[i];
                max_x = max_x > d.StartP.X ? max_x : d.StartP.X;
                max_x = max_x > d.EndP.X ? max_x : d.EndP.X;

                min_x = min_x < d.StartP.X ? min_x : d.StartP.X;
                min_x = min_x < d.EndP.X ? min_x : d.EndP.X;


                max_y = max_y > d.StartP.Y ? max_y : d.StartP.Y;
                max_y = max_y > d.EndP.Y ? max_y : d.EndP.Y;

                min_y = min_y < d.StartP.Y ? min_y : d.StartP.Y;
                min_y = min_y < d.EndP.Y ? min_y : d.EndP.Y;

            }

        }


    }

    #endregion

}

