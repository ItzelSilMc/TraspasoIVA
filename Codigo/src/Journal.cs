using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMXTRASPIVA
{
    public class Journal
    {
        /// <summary>
        /// ERP Visual Journal number.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// It is the end date of the reporting period.
        /// </summary>
        public string GJ_DATE { get; set; }

        /// <summary>
        /// Description of accounting policy.
        /// </summary>
        public string DESCRIPTION { get; set; }

        /// <summary>
        /// It is the end date of the reporting period.
        /// </summary>
        public string POSTING_DATE { get; set; }

        /// <summary>
        /// Amount debit or charge. It is the total sum of the amounts grouped .
        /// </summary>
        public double TOTAL_DR_AMOUNT { get; set; }

        /// <summary>
        /// Monto del crédito o abono. It is the total sum of the amounts grouped .
        /// </summary>
        public double TOTAL_CR_AMOUNT { get; set; }

        /// <summary>
        /// Value "1" .
        /// </summary>
        public double SELL_RATE { get; set; }

        /// <summary>
        /// Value "1" .
        /// </summary>
        public double BUY_RATE { get; set; }

        /// <summary>
        /// Name of entity selected by the user. This field is obtained from ENTITY.ID .
        /// </summary>
        public string ENTITY_ID { get; set; }

        /// <summary>
        /// The system date when the policy is generated is taken.
        /// </summary>
        public string CREATE_DATE { get; set; }

        /// <summary>
        /// User name used by the application .
        /// </summary>
        public string USER_ID { get; set; }

        /// <summary>
        /// It is the currency of the entity selected by the user.
        /// </summary>
        public string CURRENCY_ID { get; set; }

        /// <summary>
        /// Valor de ‘Y’.
        /// </summary>
        public string POST_ALL_TRACKING { get; set; }

        /// <summary>
        /// Valor de ‘Y’.
        /// </summary>
        public string POST_AS_NATIVE { get; set; }

        /// <summary>
        /// Valor de ‘N’.
        /// </summary>
        public string USER_EXCH_RATE { get; set; }

        /// <summary>
        /// Valor de ‘Y’.
        /// </summary>
        public string POSTING_CANDIDATE { get; set; }

        public string SITE_ID { get; set; }

        public string CURRENCY_NATIVE {get; set;}

        public DateTime EFFECTIVE_DATE_EXCHANGE { get; set; }

        /// <summary>
        /// Constructor that sets the parameters of the class.
        /// </summary>
        public Journal()
        {
            ID = string.Empty;
            GJ_DATE = DateTime.Now.ToShortDateString();
            DESCRIPTION = string.Empty;
            POSTING_DATE = DateTime.Now.ToShortDateString();
            TOTAL_DR_AMOUNT = 0;
            TOTAL_CR_AMOUNT = 0;            
            BUY_RATE = 1;
            ENTITY_ID = string.Empty;
            CREATE_DATE = DateTime.Now.ToShortDateString();
            USER_ID = string.Empty;
            CURRENCY_ID = string.Empty;
            SELL_RATE = 1;
            POST_ALL_TRACKING = "Y";
            POST_AS_NATIVE = "Y";
            USER_EXCH_RATE = "N";
            POSTING_CANDIDATE = "Y";
            CURRENCY_NATIVE = string.Empty;
        }
    }
}
