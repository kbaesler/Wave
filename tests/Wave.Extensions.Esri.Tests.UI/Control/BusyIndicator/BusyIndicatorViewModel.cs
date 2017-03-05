using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wave.Extensions.Esri.Tests.UI.Control.BusyIndicator
{
    public class BusyIndicatorViewModel : BaseViewModel
    {              
        private bool _IsSpinning;
        public bool IsSpinning
        {
            get { return _IsSpinning; }
            set
            {
                _IsSpinning = value;
                OnPropertyChanged("IsSpinning");
            }
        }
    }
}
