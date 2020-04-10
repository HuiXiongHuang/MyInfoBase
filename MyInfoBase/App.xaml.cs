using Lierda.WPFHelper;
using System.Windows;

namespace MyInfoBase
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        LierdaCracker cracker = new LierdaCracker(); 
        protected override void OnStartup(StartupEventArgs e)
        {
            cracker.Cracker(100);//垃圾回收间隔时间       
            base.OnStartup(e);     
        }

    }
}
