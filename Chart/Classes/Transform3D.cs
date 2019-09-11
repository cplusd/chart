using System;
using UIKit;
using CoreGraphics;
using CoreAnimation;
namespace Chart
{
    public class Transform3D:UIView
    {
        public Transform3D(CGRect frame)
        {
            this.Frame = frame;
            this.BackgroundColor = UIColor.Green;
            var transform = CATransform3D.Identity;
            transform.m34 = -1 / 500f;
            this.Layer.Transform = transform.Rotate((nfloat)Math.PI / 4f, 0, 1, 0);
            var panGesture = new UIPanGestureRecognizer(HandlePan);
            this.AddGestureRecognizer(panGesture);

        }
        CGPoint angle = new CGPoint(0, 0);
        private void HandlePan(UIPanGestureRecognizer reg)
        {
            var point = reg.TranslationInView(this);

            var angleX = angle.X + (point.X / 30);
            var angleY = angle.Y - (point.Y / 30);


            var transform = CATransform3D.Identity;
            transform.m34 = -1 / 500;
            transform = transform.Rotate(angleX, 0, 1, 0);//CATransform3DRotate(transform, angleX, 0, 1, 0)
            transform = transform.Rotate(angleY, 1, 0, 0);//CATransform3DRotate(transform, angleY, 1, 0, 0)
            //blueView.layer.transform = transform
            this.Layer.Transform = transform;


            if (reg.State == UIGestureRecognizerState.Ended){
                angle.X = angleX;
                angle.Y = angleY;
            }





        }






    }
}
