#pragma clang diagnostic ignored "-Wdeprecated-declarations"
#pragma clang diagnostic ignored "-Wtypedef-redefinition"
#pragma clang diagnostic ignored "-Wobjc-designated-initializers"
#pragma clang diagnostic ignored "-Wunguarded-availability-new"
#define DEBUG 1
#include <stdarg.h>
#include <objc/objc.h>
#include <objc/runtime.h>
#include <objc/message.h>
#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <CoreGraphics/CoreGraphics.h>
#import <QuartzCore/QuartzCore.h>

@class UIApplicationDelegate;
@class AppDelegate;
@class Chart_CrossLinesView;
@class Chart_BaseLineChart;
@class Chart_MultiLinesChart;
@class Chart_BaseLineChart_fill;
@class Chart_MultiLinesChart_fill;
@class Chart_PieChart;
@class Chart_ColumnChart;
@class Chart_RadarChart;
@class Chart_CenterAxisView;
@class Chart_PointsChart;
@class Chart_FlyChart;
@class Chart_ShowChartViewController;
@class UITableViewSource;
@class Chart_ViewController_TBS;
@class Chart_ViewController_TBC;
@class ViewController;
@class Chart_HeatChart;
@class Chart_CircleProgress;
@class UIKit_UIControlEventProxy;
@class Foundation_NSDispatcher;
@class __MonoMac_NSActionDispatcher;
@class __MonoMac_NSSynchronizationContextDispatcher;
@class Foundation_NSAsyncDispatcher;
@class __MonoMac_NSAsyncSynchronizationContextDispatcher;
@class UIKit_UIScrollView__UIScrollViewDelegate;
@class __NSObject_Disposer;

@interface UIApplicationDelegate : NSObject<UIApplicationDelegate> {
}
	-(id) init;
@end

@interface AppDelegate : NSObject<UIApplicationDelegate, UIApplicationDelegate> {
}
	-(void) release;
	-(id) retain;
	-(int) xamarinGetGCHandle;
	-(void) xamarinSetGCHandle: (int) gchandle;
	-(UIWindow *) window;
	-(void) setWindow:(UIWindow *)p0;
	-(BOOL) application:(UIApplication *)p0 didFinishLaunchingWithOptions:(NSDictionary *)p1;
	-(void) applicationWillResignActive:(UIApplication *)p0;
	-(void) applicationDidEnterBackground:(UIApplication *)p0;
	-(void) applicationWillEnterForeground:(UIApplication *)p0;
	-(void) applicationDidBecomeActive:(UIApplication *)p0;
	-(void) applicationWillTerminate:(UIApplication *)p0;
	-(BOOL) conformsToProtocol:(void *)p0;
	-(id) init;
@end

@interface Chart_CrossLinesView : UIView {
}
	-(void) release;
	-(id) retain;
	-(int) xamarinGetGCHandle;
	-(void) xamarinSetGCHandle: (int) gchandle;
	-(void) drawRect:(CGRect)p0;
	-(BOOL) conformsToProtocol:(void *)p0;
@end

@interface Chart_BaseLineChart : Chart_CrossLinesView {
}
@end

@interface Chart_MultiLinesChart : Chart_CrossLinesView {
}
@end

@interface Chart_BaseLineChart_fill : Chart_CrossLinesView {
}
@end

@interface Chart_MultiLinesChart_fill : Chart_CrossLinesView {
}
@end

@interface Chart_PieChart : Chart_CrossLinesView {
}
@end

@interface Chart_ColumnChart : Chart_CrossLinesView {
}
@end

@interface Chart_RadarChart : Chart_CrossLinesView {
}
@end

@interface Chart_CenterAxisView : UIView {
}
	-(void) release;
	-(id) retain;
	-(int) xamarinGetGCHandle;
	-(void) xamarinSetGCHandle: (int) gchandle;
	-(void) drawRect:(CGRect)p0;
	-(BOOL) conformsToProtocol:(void *)p0;
@end

@interface Chart_PointsChart : Chart_CenterAxisView {
}
@end

@interface Chart_FlyChart : UIView {
}
	-(void) release;
	-(id) retain;
	-(int) xamarinGetGCHandle;
	-(void) xamarinSetGCHandle: (int) gchandle;
	-(BOOL) conformsToProtocol:(void *)p0;
@end

@interface Chart_ShowChartViewController : UIViewController {
}
	-(void) release;
	-(id) retain;
	-(int) xamarinGetGCHandle;
	-(void) xamarinSetGCHandle: (int) gchandle;
	-(void) viewDidLoad;
	-(void) viewWillAppear:(BOOL)p0;
	-(BOOL) conformsToProtocol:(void *)p0;
	-(id) init;
@end

@interface UITableViewSource : NSObject<UIScrollViewDelegate, UIScrollViewDelegate> {
}
	-(id) init;
@end

@interface ViewController : UIViewController {
}
	-(void) release;
	-(id) retain;
	-(int) xamarinGetGCHandle;
	-(void) xamarinSetGCHandle: (int) gchandle;
	-(void) viewDidLoad;
	-(void) didReceiveMemoryWarning;
	-(BOOL) conformsToProtocol:(void *)p0;
@end

@interface Chart_HeatChart : Chart_CenterAxisView {
}
	-(void) drawRect:(CGRect)p0;
@end

@interface Chart_CircleProgress : UIView {
}
	-(void) release;
	-(id) retain;
	-(int) xamarinGetGCHandle;
	-(void) xamarinSetGCHandle: (int) gchandle;
	-(BOOL) conformsToProtocol:(void *)p0;
@end


