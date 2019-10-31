namespace VMXTRASPIVA
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using UtilidadesX;
    using System.Text;
    using CTech.Helper.Configuracion;

    internal class Detalles_Poliza
    {
        private SqlConnection cnn;
        private UtilidadesFBX utilidaes = new UtilidadesFBX();

        public Detalles_Poliza(SqlConnection cnn)
        {
            this.cnn = cnn;
        }

        /// <summary>
        /// Muestra el detalle del movimiento seleccionado
        /// </summary>
        /// <param name="tipo">Tipo de movimiento CXP o CXC</param>
        /// <param name="cuenta_origen">Cuenta origen contabilidad</param>
        /// <param name="cuenta_traslado">Cuenta destino contabilidad</param>
        /// <param name="mes">Mes</param>
        /// <param name="ano">Año</param>
        /// <param name="conciliado">Indica si esta o no conciliado</param>
        /// <param name="Banco">Banco</param>
        /// <param name="Control">Numero de factura o de cheque</param>
        /// <returns>Un datatable con el datalle del movimiento seleccionado</returns>
        public DataTable detalles(string tipo, string cuenta_origen, string cuenta_traslado, int mes, int ano, bool conciliado, string Banco, string Control, string site_id)
        {
            DataTable dt = new DataTable();
            string query = string.Empty;

            if (conciliado && tipo == "CXC")
            {
                //CXC.TC_Pago AS SELL_RATE
                query = MapeoQuerySql.ObtenerPorId("Detalles_Poliza.detalles.ConciliadoYCxC");
                query = string.Format(query, ano, mes, cuenta_traslado, cuenta_origen, Banco, Control, site_id);
            }
            else if (conciliado && tipo == "CXP")
            {
                //,CXP.SELL_RATE
                query = MapeoQuerySql.ObtenerPorId("Detalles_Poliza.detalles.ConciliadoYCxP");
                query = string.Format(query, ano, mes, cuenta_traslado, cuenta_origen, Banco, Control, site_id);
            }
            else if (!conciliado && tipo == "CXP")
            {
                //,CXP.SELL_RATE
                query = MapeoQuerySql.ObtenerPorId("Detalles_Poliza.detalles.NoConciliadoYCxP");
                query = string.Format(query, ano, mes, cuenta_traslado, cuenta_origen, Banco, Control, site_id);
            }
            else
            {
                //,CXC.TC_Pago AS SELL_RATE
                query = MapeoQuerySql.ObtenerPorId("Detalles_Poliza.detalles.PorDefault");
                query = string.Format(query, ano, mes, cuenta_traslado, cuenta_origen, Banco, Control, site_id);
            }

            return this.utilidaes.llenar_tabla(dt, query, this.cnn);
        }


        public DataTable detalles_Mov_cabecera(string NO_TRANSACCION, string tipo)
        {
            DataTable dt = new DataTable();

            string str_sql = MapeoQuerySql.ObtenerPorId("Detalles_Poliza.detalles_Mov_cabecera");
            str_sql = string.Format(str_sql, tipo, NO_TRANSACCION);

            return this.utilidaes.llenar_tabla(dt, str_sql, this.cnn);
        }

        public DataTable detalles_Mov_linea(string NO_TRANSACCION, string tipo)
        {
            DataTable dt = new DataTable();

            string str_sql = MapeoQuerySql.ObtenerPorId("Detalles_Poliza.detalles_Mov_linea");
            str_sql = string.Format(str_sql, tipo, NO_TRANSACCION);

            return this.utilidaes.llenar_tabla(dt, str_sql, this.cnn);
        }


    }
}

