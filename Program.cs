using System;
using System.Windows.Forms;
using Ninject;
using Ninject.Extensions.Factory;
using Ninject.Extensions.Conventions;

namespace MyPhotoshop
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var container = new StandardKernel();
            container.Bind<MainWindow>().ToSelf().InSingletonScope()
                .OnActivation(w =>
                {
                    foreach (var filter in typeof(Filters).GetProperties())
                    {
                        w.AddFilter((IFilter)filter.GetValue(null, null));
                    }
                });
            var window = container.Get<MainWindow>();
            Application.Run(window);
        }
    }
}
