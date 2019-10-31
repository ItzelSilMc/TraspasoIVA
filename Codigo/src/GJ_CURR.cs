using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMXTRASPIVA
{
    /// <summary>
    /// Stores the policy amount in the currency of the system and should be traceable to each of the same coin .
    /// </summary>
    public class GJ_CURR
    {
        /// <summary>
        /// ERP Visual Journal number.
        /// </summary>
        public string GJ_ID { get; set; }

        public string CURRENCY_ID { get; set; }
        
        public double AMOUNT { get; set; }
        
        public double SELL_RATE { get; set; }
        
        public double BUY_RATE { get; set; }
    }
}
