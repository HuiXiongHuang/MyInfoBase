﻿#pragma checksum "..\..\MessageBox.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "AC3C59C17F99B4E2432FAD1C6B46D6C7CDB39FF83F8092B6599B636327CDA84B"
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace WPFMessageBoxView {
    
    
    /// <summary>
    /// MessageBox
    /// </summary>
    public partial class MessageBox : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 2 "..\..\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal WPFMessageBoxView.MessageBox MessageBoxWindow;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Media.ScaleTransform Scale;
        
        #line default
        #line hidden
        
        
        #line 118 "..\..\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Expander DetailsExpander;
        
        #line default
        #line hidden
        
        
        #line 124 "..\..\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox DetailsText;
        
        #line default
        #line hidden
        
        
        #line 134 "..\..\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.StackPanel ButtonsPanel;
        
        #line default
        #line hidden
        
        
        #line 138 "..\..\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image ImagePlaceholder;
        
        #line default
        #line hidden
        
        
        #line 140 "..\..\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label MessageLabel;
        
        #line default
        #line hidden
        
        
        #line 141 "..\..\MessageBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock MessageText;
        
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
            System.Uri resourceLocater = new System.Uri("/WPFMessageBoxView;component/messagebox.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MessageBox.xaml"
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
            this.MessageBoxWindow = ((WPFMessageBoxView.MessageBox)(target));
            
            #line 9 "..\..\MessageBox.xaml"
            this.MessageBoxWindow.Closing += new System.ComponentModel.CancelEventHandler(this.MessageBoxWindow_Closing);
            
            #line default
            #line hidden
            
            #line 10 "..\..\MessageBox.xaml"
            this.MessageBoxWindow.Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 11 "..\..\MessageBox.xaml"
            this.MessageBoxWindow.MouseDown += new System.Windows.Input.MouseButtonEventHandler(this.Window_MouseDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.Scale = ((System.Windows.Media.ScaleTransform)(target));
            return;
            case 3:
            this.DetailsExpander = ((System.Windows.Controls.Expander)(target));
            return;
            case 4:
            this.DetailsText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 5:
            this.ButtonsPanel = ((System.Windows.Controls.StackPanel)(target));
            return;
            case 6:
            this.ImagePlaceholder = ((System.Windows.Controls.Image)(target));
            return;
            case 7:
            this.MessageLabel = ((System.Windows.Controls.Label)(target));
            return;
            case 8:
            this.MessageText = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

