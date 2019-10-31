using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMXTRASPIVA
{
    public class MonedaRastreo
    {
        private string _ID;

        public string ID
        {
            get { return _ID; }
            set { _ID = value; }
        }
        private double _SELL_RATE;

        public double SELL_RATE
        {
            get { return _SELL_RATE; }
            set { _SELL_RATE = value; }
        }
        private double _BUY_RATE;

        public double BUY_RATE
        {
            get { return _BUY_RATE; }
            set { _BUY_RATE = value; }
        }
        private DateTime _EFFECTIVE_DATE;

        public DateTime EFFECTIVE_DATE
        {
            get { return _EFFECTIVE_DATE; }
            set { _EFFECTIVE_DATE = value; }
        }
    }
}
