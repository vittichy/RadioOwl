using Caliburn.Micro;
using RadioOwl.Forms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RadioOwl
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            // sileny zpusob jak v caliburnu nastavit iconu formulare ;-)
            // http://stackoverflow.com/questions/27227892/how-do-i-set-a-window-application-icon-in-a-application-set-up-with-caliburn-mic
            var settings = new Dictionary<string, object>
            {
                // new ico? 
                { "Icon", new BitmapImage(new Uri("pack://application:,,,/RadioOwl;component/icons/1477096338_owl.png")) },
                //{ "Icon", new BitmapImage(new Uri("pack://application:,,,/RadioOwl;component/owl_horror_witch_halloween.png")) },
            };
            DisplayRootViewFor(typeof(MainViewModel), settings);
        }
    }
}


