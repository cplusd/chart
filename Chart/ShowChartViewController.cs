using System;
using System.Collections.Generic;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;
namespace Chart
{
    public enum chartType
    {
        txt_path,
        single_one_cur_line,
        single_one_cur_fill_line,
        multi_cur_lines,
        multi_cur_fill_lines,
        pie_chart,
        cirlce_ring_chart,
        column_chart,
        radar_chart,
        points_chart,
        circle_progress_chart,
        heat_chart,
        fly_line_chart,
        transform3d
    }
    public class ShowChartViewController:UIViewController
    {
        public chartType CurrentType;
        public ShowChartViewController()
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.BackgroundColor = UIColor.Black;
            InitView();

        }

        private void BackEven(object sender, EventArgs e)
        {
            if (this.NavigationController != null)
            {
                this.NavigationController.PopViewController(true);
            }
            else
            {
                this.DismissViewController(true, null);
            }

        }

        UIButton top;
        UIView Context;
        private void InitView()
        {
            top= new UIButton() {TranslatesAutoresizingMaskIntoConstraints=false,BackgroundColor=UIColor.Black };
            top.SetTitle("back", UIControlState.Normal);
            top.SetTitleColor(UIColor.White, UIControlState.Normal);
            top.TouchUpInside += BackEven;
             Context = new UIView() {TranslatesAutoresizingMaskIntoConstraints=false,BackgroundColor=UIColor.Black};
            Context.Layer.CornerRadius = 4;
            Context.Layer.BorderColor = UIColor.FromRGB(234,234,234).CGColor;
            Context.Layer.BorderWidth = 1;

            View.AddSubviews(top,Context);

            View.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|-[top(55)]-[Context]-|", NSLayoutFormatOptions.AlignAllLeading| NSLayoutFormatOptions .AlignAllTrailing| NSLayoutFormatOptions.AlignAllCenterX, "top", top, "Context", Context));

            View.AddConstraint(NSLayoutConstraint.Create(top,NSLayoutAttribute.Leading,NSLayoutRelation.Equal,View,NSLayoutAttribute.Leading,1,8));
            View.AddConstraint(NSLayoutConstraint.Create(top, NSLayoutAttribute.Trailing, NSLayoutRelation.Equal, View, NSLayoutAttribute.Trailing, 1, -8));


        }


        //
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            new System.Threading.Thread(()=> {
                InvokeOnMainThread(() => { ShowChart(); });
            }).Start();


        }




        void ShowChart()
        {       
            switch(CurrentType)
            {
                case chartType.single_one_cur_line:
                    OneCurLine();
                    break;
                case chartType.multi_cur_lines:
                    MultiCurLines();
                    break;
                case chartType.single_one_cur_fill_line:
                    OneCurLine_fill();
                    break;
                case chartType.multi_cur_fill_lines:
                    MultiCurLines_fill();
                    break;
                case chartType.pie_chart:
                    PieChart();
                    break;
                case chartType.cirlce_ring_chart:
                    CircleRingChart();
                    break;
                case chartType.column_chart:
                    ColumnChart();
                    break;
                case chartType.txt_path:
                    TxtPath();
                    break;
                case chartType.radar_chart:
                    TestRadarChart();
                    break;
                case chartType.points_chart:
                    TestPointsChart();
                    break;
                case chartType.circle_progress_chart:
                    TestCircleProgressChart();
                    break;
                case chartType.heat_chart:
                    TestHeatChart();
                    break;
                case chartType.fly_line_chart:
                    TestFlyLineChart();
                    break;
                case chartType.transform3d:
                    TestTransform3d();
                    break;
            }

        }

        private void TestTransform3d()
        {


            Transform3D k = new Transform3D(new CGRect(20,20,200,200));
            Context.AddSubview(k);

        }

        private void TestFlyLineChart()
        {
            var s = new List<FlyModel>();
            Random random = new Random(DateTime.Now.Second);
            int min = 0;
            int max = 1000;
            for(int i=0;i<15;i++)
            {
                var x0 = random.Next(min, max);
                var y0 = random.Next(min, max);
                var x1 = random.Next(min, max);
                var y1 = random.Next(min, max);
                s.Add(new FlyModel() {
                    StartP=new CGPoint(x0,y0), 
                    EndP=new CGPoint(x1,y1)});

                }


            FlyChart k = new FlyChart(new CGRect(10, 10, 340, 340),s);
            Context.AddSubview(k);
        }

        private void TestHeatChart()
        {
            List<XYValueModel> Points = new List<XYValueModel>();
            Random random = new Random(DateTime.Now.Second);        
            for (int i = 0; i < 222; i++)
            {
                int x = random.Next(0, 1000);
                int y = random.Next(0, 1000);
                int count = random.Next(1, 1000);
                Points.Add(new XYValueModel() { 
                    X=x,Y=y,Value=count
                });
            }

          


            HeatChart k = new HeatChart(new CGRect(10, 10, 340, 340), Points);
            Context.AddSubview(k);
        }

        private void TestCircleProgressChart()
        {

            var CircleChart = new CircleProgress(new CGRect(20, 20, 140, 140), 0.16f);
            var CircleChart2 = new CircleProgress(new CGRect(180, 20, 140, 140), 0.31f);
            var CircleChart3 = new CircleProgress(new CGRect(20, 180, 140, 140), 0.71f);
            var CircleChart4 = new CircleProgress(new CGRect(180, 180, 140, 140), 1f);
            Context.AddSubviews(CircleChart,CircleChart2, CircleChart3, CircleChart4);
        }

        private void TestPointsChart()
        {


            List<ShowPModel> sourceAll = new List<ShowPModel>();
            //one
            var item0 = new ShowPModel();
            item0.FillColor = UIColor.Red.ColorWithAlpha(0.5f);
            item0.StrokeColor = UIColor.Red.ColorWithAlpha(0.5f);
            var source0 = new List<XYValueModel>();
            Random randomx = new Random(DateTime.Now.Second);
            //Random randomy = new Random(21+DateTime.Now.Second);
            //Random randomV = new Random(5+DateTime.Now.Second);
            for (int i=0;i<30;i++)
            {
                var x = randomx.Next(-500, 500);
                var y = randomx.Next(-200, 200);
                var v = randomx.Next(0, 30);

                var item = new XYValueModel() {X=x,Y=y,Value=v};
                source0.Add(item);
            }
            item0.Points = source0;
            sourceAll.Add(item0);
            //another one
            var item1 = new ShowPModel();
            item1.FillColor = UIColor.Orange.ColorWithAlpha(0.5f);
            item1.StrokeColor = UIColor.Orange.ColorWithAlpha(0.5f);
            var source1 = new List<XYValueModel>();
            for (int i = 0; i < 30; i++)
            {
                var x = randomx.Next(-500, 500);
                var y = randomx.Next(-200, 200);
                var v = randomx.Next(0, 30);

                var item = new XYValueModel() { X = x, Y = y, Value = v };
                source1.Add(item);
            }
            item1.Points = source1;
            sourceAll.Add(item1);


            PointsChart k = new PointsChart(new CGRect(10, 120, 340, 340), sourceAll);
            Context.AddSubview(k);

        }

        private void TestRadarChart()
        {
            RadarChart k = new RadarChart(new CGRect(10, 120, 340, 340),new List<OneDataModel>() { 
                new OneDataModel(){LineWidth=1,StrokeColor=UIColor.Red,FillColor=UIColor.Red.ColorWithAlpha(0.5f),Points=new List<nfloat>(){30,10,30,20,30,15}
                },
                new OneDataModel(){LineWidth=1,StrokeColor=UIColor.Green,FillColor=UIColor.Green.ColorWithAlpha(0.5f),Points=new List<nfloat>(){20,23,12,27,18,19}
                },
             
            });
            Context.AddSubview(k);
        }

        private void TxtPath()
        {

          var path = ChartTool.GetStringPath("Object To evaluate.x");
          CAShapeLayer layer = new CAShapeLayer();           
          layer.Frame = new CGRect(new CGPoint(20, 20), path.CGPath.PathBoundingBox.Size);           
          layer.Path = path.CGPath;
          layer.StrokeColor = UIColor.Red.CGColor;
          layer.FillColor = UIColor.Clear.CGColor;
          //layer.BackgroundColor = UIColor.White.CGColor;
          layer.GeometryFlipped = true;
          layer.LineWidth = 2;
          Context.Layer.AddSublayer(layer);



            CABasicAnimation animation = CABasicAnimation.FromKeyPath("strokeEnd");
            animation.From = NSNumber.FromNInt(0);
            animation.To = NSNumber.FromInt16(1);
            animation.RemovedOnCompletion = false;
            animation.FillMode = CAFillMode.Forwards;
            animation.Duration = 4;
            layer.AddAnimation(animation, "ma");

        }

        private void ColumnChart()
        {                   
            ColumnChart k = new ColumnChart(new CGRect(10, 120, 340, 300),new List<ColumModel>() { 
                new ColumModel(){FillColor=UIColor.Red.ColorWithAlpha(0.5f),StrokeColor=UIColor.Red,Value=10},
                new ColumModel(){FillColor=UIColor.Yellow.ColorWithAlpha(0.5f),StrokeColor=UIColor.Yellow,Value=20},
                new ColumModel(){FillColor=UIColor.Purple.ColorWithAlpha(0.5f),StrokeColor=UIColor.Purple,Value=5},
                new ColumModel(){FillColor=UIColor.Blue.ColorWithAlpha(0.5f),StrokeColor=UIColor.Blue,Value=10},
                new ColumModel(){FillColor=UIColor.Cyan.ColorWithAlpha(0.5f),StrokeColor=UIColor.Cyan,Value=5},
                new ColumModel(){FillColor=UIColor.Green.ColorWithAlpha(0.5f),StrokeColor=UIColor.Green,Value=14},
                            
            });
            Context.AddSubview(k);

        }

        private void CircleRingChart()
        {
            PieChart k = new PieChart(new CGRect(10, 120, 340, 300), new List<PieModel>()
            {
                new PieModel(){
                StrokeColor=UIColor.White.ColorWithAlpha(0.9f),
                FillColor=UIColor.Red.ColorWithAlpha(0.8f),
                LineWidth=3,
                ItemValue=16,
                }
                ,new PieModel(){
                StrokeColor=UIColor.White.ColorWithAlpha(0.9f),
                FillColor=UIColor.Green.ColorWithAlpha(0.8f),
                LineWidth=3,
                ItemValue=20,
                },new PieModel(){
                StrokeColor=UIColor.White.ColorWithAlpha(0.9f),
                FillColor=UIColor.Orange.ColorWithAlpha(0.8f),
                LineWidth=3,
                ItemValue=30,
                }
            },true,true);
            Context.AddSubview(k);

        }

        void OneCurLine()
        {
          
            OneDataModel oneCurLine = new OneDataModel()
            {
              Points= new List<nfloat>() { 0, 1, 5, 10, 1 },
              LineWidth=2,
              StrokeColor= UIColor.FromRGB(35, 211, 197)
            };
            BaseLineChart k = new BaseLineChart(new CGRect(10,120,340,300),oneCurLine);
            Context.AddSubview(k);
        }
        void MultiCurLines()
        {

            OneDataModel oneCurLine0 = new OneDataModel()
            {
                Points = new List<nfloat>() { 0, 1, 5, 30, 1 },
                LineWidth = 2,
                StrokeColor = UIColor.FromRGB(35, 211, 197)
            };
            OneDataModel oneCurLine1 = new OneDataModel()
            {
                Points = new List<nfloat>() { 10, 3, 5, 40, 23 },
                LineWidth = 3,
                StrokeColor = UIColor.Orange
            };
            OneDataModel oneCurLine2 = new OneDataModel()
            {
                Points = new List<nfloat>() { 16, 23, 8, 20, 13 },
                LineWidth = 4,
                StrokeColor = UIColor.Cyan
            };
            OneDataModel oneCurLine3 = new OneDataModel()
            {
                Points = new List<nfloat>() { 0, 13, 0, 13, 0 },
                LineWidth = 2,
                StrokeColor = UIColor.Red
            };
            MultiLinesChart k = new MultiLinesChart(new CGRect(10, 120, 340, 300),new List<OneDataModel>() {oneCurLine0,oneCurLine1, oneCurLine2 , oneCurLine3 });
            Context.AddSubview(k);
        }
        void OneCurLine_fill()
        {
            OneDataModel oneCurLine = new OneDataModel()
            {
                Points = new List<nfloat>() { 0, 1, 5, 10, 1 },
                LineWidth = 2,
                StrokeColor = UIColor.FromRGB(35, 211, 197),
                GradientColor=new CGColor[] { UIColor.FromRGB(44, 253, 250).ColorWithAlpha(0.5f).CGColor, UIColor.White.ColorWithAlpha(0.5f).CGColor },
            };
            BaseLineChart_fill k = new BaseLineChart_fill(new CGRect(10, 120, 340, 300), oneCurLine);
            Context.AddSubview(k);
        }
        void MultiCurLines_fill()
        {
           
           OneDataModel oneCurLine0 = new OneDataModel()
           {
               Points = new List<nfloat>() { 0, 1, 5, 30, 1 },
               LineWidth = 2,
               StrokeColor = UIColor.FromRGB(35, 211, 197),
               GradientColor = new CGColor[] { UIColor.FromRGB(35, 211, 197).ColorWithAlpha(0.5f).CGColor, UIColor.White.ColorWithAlpha(0.5f).CGColor }
           };
            OneDataModel oneCurLine1 = new OneDataModel()
            {
                Points = new List<nfloat>() { 10, 3, 5, 40, 23 },
                LineWidth = 3,
                StrokeColor = UIColor.Orange,
                GradientColor = new CGColor[] { UIColor.Orange.ColorWithAlpha(0.5f).CGColor, UIColor.White.ColorWithAlpha(0.5f).CGColor }
            };
            OneDataModel oneCurLine2 = new OneDataModel()
            {
                Points = new List<nfloat>() { 16, 23, 8, 20, 13 },
                LineWidth = 4,
                StrokeColor = UIColor.Cyan,
                GradientColor = new CGColor[] {UIColor.Cyan.ColorWithAlpha(0.5f).CGColor,UIColor.White.ColorWithAlpha(0.5f).CGColor}
            };
            OneDataModel oneCurLine3 = new OneDataModel()
            {
                Points = new List<nfloat>() { 0, 13, 0, 13, 0 },
                LineWidth = 2,
                StrokeColor = UIColor.Red,
                GradientColor = new CGColor[] { UIColor.Red.ColorWithAlpha(0.5f).CGColor, UIColor.White.ColorWithAlpha(0.5f).CGColor }
            };
            MultiLinesChart_fill k = new MultiLinesChart_fill(new CGRect(10, 120, 340, 300), new List<OneDataModel>() { oneCurLine0, oneCurLine1, oneCurLine2 , oneCurLine3 });
            Context.AddSubview(k);
        }

        void PieChart()
        {
            PieChart k = new PieChart(new CGRect(10, 120, 340, 300), new List<PieModel>()
            {
                new PieModel(){
                StrokeColor=UIColor.White.ColorWithAlpha(0.9f),
                FillColor=UIColor.Red.ColorWithAlpha(0.8f),
                LineWidth=3,
                ItemValue=16,
                }
                ,new PieModel(){
                     StrokeColor=UIColor.White.ColorWithAlpha(0.9f),
                FillColor=UIColor.Green.ColorWithAlpha(0.8f),
                LineWidth=3,
                ItemValue=20,
                },new PieModel(){
                     StrokeColor=UIColor.White.ColorWithAlpha(0.9f),
                FillColor=UIColor.Orange.ColorWithAlpha(0.8f),
                LineWidth=3,
                ItemValue=30,
                }
            });
            Context.AddSubview(k);
        }


    }
}
