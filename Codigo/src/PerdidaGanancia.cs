using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace VMXTRASPIVA
{
    public class PerdidaGanancia
    {
        public double Perdida { get; set; }
        public double Ganancia { get; set; }
        public string CuentaPerdida { get; set; }
        public string CuentaGanancia { get; set; }

        public PerdidaGanancia getPerdidaGanancia(DataTable dtDetalleMovimientos)
        {
            // Perdida Ganancia
            double dPerdidaGanancia = 0;
            double dPerdida = 0;
            double dGanancia = 0;
            PerdidaGanancia oPG = new PerdidaGanancia();

            foreach (DataRow rowDetalle in dtDetalleMovimientos.Rows)
            {
                dPerdidaGanancia = double.Parse(rowDetalle["PERDIDA_GANANCIA"].ToString());

                if (dPerdidaGanancia < 0)
                {
                    dPerdida += Math.Abs(dPerdidaGanancia);
                }
                else
                {
                    dGanancia += Math.Abs(dPerdidaGanancia);
                }
            }

            oPG.Perdida = Math.Round(dPerdida, 4);
            oPG.Ganancia = Math.Round(dGanancia, 4);

            return oPG;
        }

        public PerdidaGanancia getPerdidaGanancia(DataTable pdtDetalla, string pCtaOrigen, string pCtaDestino)
        {
            PerdidaGanancia oPG = new PerdidaGanancia();
            DataRow[] result = pdtDetalla.Select("VAT_GL_ACCT_ID = '" + pCtaOrigen + "' AND TRASLADO = '" + pCtaDestino + "'");
            string sCtaPerdida = string.Empty;
            string sCtaGanancia = string.Empty;
            double dPerdidaGanancia = 0;

            foreach (DataRow rowDetalle in result)
            {
                dPerdidaGanancia = double.Parse(rowDetalle["PERDIDA_GANANCIA"].ToString());

                if (dPerdidaGanancia != 0)
                {
                    if (rowDetalle["VAT_GL_ACCT_ID"].ToString().Equals(pCtaOrigen) && rowDetalle["TRASLADO"].ToString().Equals(pCtaDestino) && dPerdidaGanancia < 0)
                    {
                        oPG.Perdida += Math.Abs(dPerdidaGanancia);
                        oPG.CuentaPerdida = rowDetalle["CUENTA_PER_GANANCIA"].ToString();
                    }
                    else
                    {
                        oPG.Ganancia += dPerdidaGanancia;
                        oPG.CuentaGanancia = rowDetalle["CUENTA_PER_GANANCIA"].ToString();
                    }
                }

            }

            return oPG;
        }
    }
}
