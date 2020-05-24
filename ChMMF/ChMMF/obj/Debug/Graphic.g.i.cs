﻿#pragma checksum "..\..\Graphic.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "6A5647625A8E51B3DEE5761CF7210B8790A82C81A6E59DA65C98AF52E8B7F450"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using D3PaletteControl;
using Microsoft.Research.DynamicDataDisplay;
using Microsoft.Research.DynamicDataDisplay.Charts;
using Microsoft.Research.DynamicDataDisplay.Charts.Axes;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps;
using Microsoft.Research.DynamicDataDisplay.Charts.Maps.Network;
using Microsoft.Research.DynamicDataDisplay.Charts.Navigation;
using Microsoft.Research.DynamicDataDisplay.Charts.Shapes;
using Microsoft.Research.DynamicDataDisplay.Common.Palettes;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.FileServers;
using Microsoft.Research.DynamicDataDisplay.Maps.Servers.Network;
using Microsoft.Research.DynamicDataDisplay.Navigation;
using Microsoft.Research.DynamicDataDisplay.PointMarkers;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Adaptive_MSE {
    
    
    /// <summary>
    /// Graphic
    /// </summary>
    public partial class Graphic : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 7 "..\..\Graphic.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Microsoft.Research.DynamicDataDisplay.ChartPlotter graphic;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\Graphic.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dataGridResults;
        
        #line default
        #line hidden
        
        
        #line 10 "..\..\Graphic.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox listBoxIterations;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\Graphic.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelIteration;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\Graphic.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label labelGraphic;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\Graphic.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonPrevGraphic;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\Graphic.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button buttonNextGraphic;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Adaptive_MSE;component/graphic.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\Graphic.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 5 "..\..\Graphic.xaml"
            ((Adaptive_MSE.Graphic)(target)).Loaded += new System.Windows.RoutedEventHandler(this.graphic_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.graphic = ((Microsoft.Research.DynamicDataDisplay.ChartPlotter)(target));
            return;
            case 3:
            this.dataGridResults = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 4:
            this.listBoxIterations = ((System.Windows.Controls.ListBox)(target));
            
            #line 10 "..\..\Graphic.xaml"
            this.listBoxIterations.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ListBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            this.labelIteration = ((System.Windows.Controls.Label)(target));
            return;
            case 6:
            this.labelGraphic = ((System.Windows.Controls.Label)(target));
            return;
            case 7:
            this.buttonPrevGraphic = ((System.Windows.Controls.Button)(target));
            
            #line 14 "..\..\Graphic.xaml"
            this.buttonPrevGraphic.Click += new System.Windows.RoutedEventHandler(this.buttonPrevGraphic_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.buttonNextGraphic = ((System.Windows.Controls.Button)(target));
            
            #line 15 "..\..\Graphic.xaml"
            this.buttonNextGraphic.Click += new System.Windows.RoutedEventHandler(this.buttonNextGraphic_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
