using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Chart
{
   
    public class CircleProgress : UIView
    {
        public readonly UIColor COLOR_PROGRESS = UIColor.FromRGB(253, 173, 42);
        public readonly UIColor COLOR_BG = UIColor.FromRGB(204, 204, 204);
        public readonly UIColor COLOR_PRO_100 = UIColor.FromRGB(92, 155, 49);

        nfloat pr_width = 12;
        nfloat bg_width = 8;

        CAShapeLayer bg_layer;
        CAShapeLayer progress_layer;
        UILabel lbl;
        nfloat fontSize = 16;
        nfloat progress = 0.0f;
        nfloat step = 0.03f;
        /// <summary>
        /// Initializes a new instance of the <see cref="T:test0000001.CircleProgress"/> class.
        /// </summary>
        /// <param name="_frame">Frame.</param>
        /// <param name="percent">范围Small Or Equal than 1</param>
        public CircleProgress(CGRect _frame, nfloat percent)
        {
            progress = (float)Math.Min(1f, percent < 0 ? 0 : percent);
            this.BackgroundColor = UIColor.White;
            this.Frame = _frame;
            AddBG();
            AddProgress();
            AddLbl();
        }

        CADisplayLink dl2;
        void AddBG()
        {
            UIBezierPath b_path = UIBezierPath.FromRoundedRect(new CGRect(bg_width, bg_width, this.Bounds.Width - bg_width - bg_width, this.Bounds.Height - bg_width - bg_width), (this.Bounds.Width - bg_width - bg_width) / 2f);

            bg_layer = new CAShapeLayer
            {
                StrokeColor = COLOR_BG.CGColor,
                Path = b_path.CGPath,
                LineWidth = bg_width,
                StrokeStart = 0.0f,
                StrokeEnd = 1f,
                FillColor = UIColor.White.CGColor,
                FillRule = CAShapeLayer.FillRuleEvenOdd,
            };
            this.Layer.AddSublayer(bg_layer);

        }
        public void UpdatePercent(nfloat newP)
        {
            progress = newP;
            if (progress_layer != null)
            {
                CADisplayLink dl = null;
                //progress_layer.StrokeEnd = 0;
                progress_layer.StrokeColor = COLOR_PROGRESS.CGColor;
                if (lbl != null)
                {
                    lbl.TextColor = COLOR_PROGRESS;
                }
                dl = CADisplayLink.Create(() => {

                    if (progress_layer.StrokeEnd < progress)
                    {
                        progress_layer.StrokeEnd += step;
                        if (lbl != null)
                        {
                            var radio = progress_layer.StrokeEnd;
                            if (radio <= progress)
                            {
                                var str = (radio * 100).ToString("0.0") + "%";
                                lbl.Text = str;
                            }

                        }
                    }
                    else
                    {
                        dl.Invalidate();
                        if (lbl != null)
                        {
                            var str = (progress * 100).ToString("0.0") + "%";
                            lbl.Text = str;
                        }
                        ChangeColor();
                    }

                });
                dl.AddToRunLoop(NSRunLoop.Current, NSRunLoopMode.Common);

            }


        }

        void ChangeColor()
        {
            if (progress == 1)
            {
                if (progress_layer != null)
                    progress_layer.StrokeColor = COLOR_PRO_100.CGColor;
                if (lbl != null)
                {
                    lbl.TextColor = COLOR_PRO_100;
                }
            }
        }
        void AddProgress()
        {
            UIBezierPath pr_path = UIBezierPath.FromRoundedRect(new CGRect(pr_width / 2f, pr_width / 2f, this.Bounds.Width - pr_width, this.Bounds.Height - pr_width), (this.Bounds.Width - pr_width) / 2f);

            progress_layer = new CAShapeLayer
            {
                StrokeColor = COLOR_PROGRESS.CGColor,
                Path = pr_path.CGPath,
                LineWidth = pr_width,
                StrokeStart = 0.0f,
                StrokeEnd = 0.0f,
                FillColor = UIColor.Clear.CGColor,
                FillRule = CAShapeLayer.FillRuleEvenOdd

            };
            this.Layer.AddSublayer(progress_layer);

            dl2 = CADisplayLink.Create(() => {
                if (progress_layer.StrokeEnd < progress)
                {
                    progress_layer.StrokeEnd += step;
                    if (lbl != null)
                    {
                        var radio = progress_layer.StrokeEnd;
                        if (radio <= progress)
                        {
                            var str = (radio * 100).ToString("0.0") + "%";
                            lbl.Text = str;
                        }
                    }
                }
                else
                {
                    dl2.Invalidate();
                    if (lbl != null)
                    {
                        var str = (progress * 100).ToString("0.0") + "%";
                        lbl.Text = str;
                    }
                    ChangeColor();
                }

            });
            dl2.AddToRunLoop(NSRunLoop.Current, NSRunLoopMode.Common);
        }

        void AddLbl()
        {
            lbl = new UILabel()
            {
                TranslatesAutoresizingMaskIntoConstraints = false,
                TextColor = COLOR_PROGRESS
                    ,
                Text = "0.0%",
                TextAlignment = UITextAlignment.Center
                ,
                Font = UIFont.BoldSystemFontOfSize(fontSize)

            };
            this.AddSubview(lbl);
            this.AddConstraints(NSLayoutConstraint.FromVisualFormat("|[lbl]|", NSLayoutFormatOptions.AlignAllCenterY, "lbl", lbl));
            this.AddConstraints(NSLayoutConstraint.FromVisualFormat("V:|[lbl]|", NSLayoutFormatOptions.AlignAllCenterX, "lbl", lbl));

        }
    }
}
