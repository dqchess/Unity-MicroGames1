//
//  ISTapjoyAdapter.h
//  ISTapjoyAdapter
//
//  Created by Daniil Bystrov on 4/13/16.
//  Copyright © 2016 IronSource. All rights reserved.
//

#import <Foundation/Foundation.h>
#import "IronSource/ISBaseAdapter+Internal.h"

static NSString * const TapjoyAdapterVersion     = @"4.1.3";

//System Frameworks For Tapjoy Adapter

@import AdSupport;
@import CFNetwork;
@import CoreData;
@import CoreGraphics;
@import CoreMotion;
@import Foundation;
@import ImageIO;
@import MapKit;
@import MediaPlayer;
@import MobileCoreServices;
@import QuartzCore;
@import Security;
@import SystemConfiguration;
@import UIKit;
@import WebKit;

//optional
@import CoreTelephony;
@import PassKit;
@import StoreKit;

@interface ISTapjoyAdapter : ISBaseAdapter

@end



