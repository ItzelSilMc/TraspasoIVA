using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMXTRASPIVA
{
    public class VMX_CONTOLPOLIZA_LINE
    {
        public string NO_TRANSACCION { get; set; }
        public string BANK_ACCOUNT_ID { get; set; }
        public string CONTROL_NO { get; set; }
        public string CHECK_ID { get; set; }
        public string VAT_AMOUNT { get; set; }
        public double SELL_RATE { get; set; }
        public double MONTO { get; set; }
        public string CUENTA { get; set; }
        public string DESCRIPCION { get; set; }
        public string CUENTA_TRASLADO { get; set; }
        public string DESCRIPTION { get; set; }
        public string TIPO_OPERACION { get; set; }
        public string CUSTOMER_ID { get; set; }
        public double PERDIDA_GANANCIA { get; set; }
        public double IVA_DEPOSITO { get; set; }
        public string SITE_ID { get; set; }
        public double MONTO_RET { get; set; }
        public double RET_DEPOSITO { get; set; }
    }
}
