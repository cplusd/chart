using Foundation;
using System;
using System.Collections.Generic;
using UIKit;

namespace Chart
{
    public partial class ViewController : UIViewController
    {

        class CellModel
        {
            public string TypeName { get; set; }
            public string TypeIcon { get; set; }
            public chartType ChartType { get; set; }
        }

        List<CellModel> SourceList;
        public ViewController(IntPtr handle) : base(handle)
        {
        }
        UITableView tableView;
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            InitSourceList();
            InitTableView();
        }

        private void InitSourceList()
        {
            SourceList = new List<CellModel>() {

                new CellModel(){TypeName="文本路径",TypeIcon="txtPath.jpg",ChartType=chartType.txt_path},
                //new CellModel(){TypeName="clock",TypeIcon="txtPath.jpg",ChartType=chartType.txt_path},
                //力学图 \聚合图                
                new CellModel(){TypeName="单曲线",TypeIcon="sample_single_cur_line.jpg",ChartType=chartType.single_one_cur_line},
                new CellModel(){TypeName="多曲线",TypeIcon="sample_multi_cur_line.jpg",ChartType=chartType.multi_cur_lines},
                new CellModel(){TypeName="单曲线+fill",TypeIcon="sample_single_cur_fill_line.jpg",ChartType=chartType.single_one_cur_fill_line},
                new CellModel(){TypeName="多曲线+fill",TypeIcon="sample_multi_cur_fill_line.jpg",ChartType=chartType.multi_cur_fill_lines},
                new CellModel(){TypeName="Pie Chart",TypeIcon="sample_pie.jpg",ChartType=chartType.pie_chart},
                new CellModel(){TypeName="circle ring Chart",TypeIcon="circle_ring.jpg",ChartType=chartType.cirlce_ring_chart},
                new CellModel(){TypeName="Column Chart",TypeIcon="columnS.jpg",ChartType=chartType.column_chart},
                new CellModel(){TypeName="Radar Chart",TypeIcon="radar.jpg",ChartType=chartType.radar_chart},
                new CellModel(){TypeName="Points Chart",TypeIcon="points.jpg",ChartType=chartType.points_chart},
                new CellModel(){TypeName="CircleProgress Chart",TypeIcon="circleProgress.jpg",ChartType=chartType.circle_progress_chart},
                new CellModel(){TypeName="热力图",TypeIcon="heatChart.jpg",ChartType=chartType.heat_chart},
                new CellModel(){TypeName="飞线图",TypeIcon="flyLine.jpg",ChartType=chartType.fly_line_chart},
                new CellModel(){TypeName="Transform3D",TypeIcon="transform3d.jpg",ChartType=chartType.transform3d},
            };
        }

        void InitTableView()
        {
            tableView = new UITableView() { TranslatesAutoresizingMaskIntoConstraints = false };
            View.AddSubview(tableView);
            tableView.EstimatedRowHeight = 44;
            View.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|-64-[tableView]-|", NSLayoutFormatOptions.AlignAllLeft, "tableView", tableView));
            View.AddConstraints(NSLayoutConstraint.FromVisualFormat("H:|[tableView]|", NSLayoutFormatOptions.AlignAllLeft, "tableView", tableView));
            var source = new TBS(SourceList);
            source.RowSelectedEvent += HandleRowClicked;
            tableView.Source = source;
        }
        //点击chart 跳转页面
        private void HandleRowClicked(int obj)
        {
            ShowChartViewController showChartViewController = new ShowChartViewController();
            showChartViewController.CurrentType = SourceList[obj].ChartType;
            this.PresentModalViewController(showChartViewController,true);
        }

        class TBS : UITableViewSource
        {
            List<CellModel> SourceList;
            public Action<int> RowSelectedEvent;
            public TBS(List<CellModel> _SourceList)
            {
                SourceList = _SourceList;
            }

            public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
            {
                if (!(tableView.DequeueReusableCell(TBC.CID) is TBC cell))
                {
                    cell = new TBC(TBC.CID);
                }
                var item = SourceList[indexPath.Row];
                cell.ChartIcon.Image = UIImage.FromFile(item.TypeIcon);
                cell.ChartTypeName.Text = item.TypeName;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
                return cell;
            }

            public override nint RowsInSection(UITableView tableview, nint section)
            {
                return SourceList.Count;
            }

            public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
            {
                RowSelectedEvent?.Invoke(indexPath.Row);
            }


        }

        class TBC:UITableViewCell
        {
            public nfloat IconWidth = 130;
            public nfloat IconHeight = 130;
            public UIImageView ChartIcon { get; set; }
            public UILabel ChartTypeName { get; set; }
            public const string CID = "ChartCID";
            public TBC(string cid):base(UITableViewCellStyle.Default,cid)
            {
                ChartIcon = new UIImageView() { TranslatesAutoresizingMaskIntoConstraints = false ,BackgroundColor=UIColor.FromRGBA(121, 255, 122, 255) };// 121 ,255 ,122 ,255
                ChartTypeName = new UILabel() { TranslatesAutoresizingMaskIntoConstraints = false };
                ContentView.AddSubviews(ChartIcon,ChartTypeName);
                ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat("|-[ChartIcon("+ IconWidth + ")]-(>=0)-[ChartTypeName]-|", NSLayoutFormatOptions.AlignAllCenterY, "ChartIcon", ChartIcon, "ChartTypeName", ChartTypeName));//("+ IconWidth + ")
                ContentView.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|-1-[ChartIcon(" + IconWidth + ")]-1-|", NSLayoutFormatOptions.AlignAllLeft, "ChartIcon", ChartIcon));
            }
                    
        
        }


        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();           
        }
    }
}