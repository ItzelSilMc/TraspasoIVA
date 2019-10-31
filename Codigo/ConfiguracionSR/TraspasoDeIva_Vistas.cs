using System.Text;
using CTECH.Acceso_a_Datos;

/*
Esta libreria se creo para manejar el update de las vistas básicas de traspaso de iva
causado por el switch de perdida ganancia por tipo de cambio de CR <-> DR
Esta clase deberá ser actualizada cuando se realice una nueva versión de dichas vistas:

 VMX_IVA_CXC_02
 VMX_IVA_TRASL_CXP_01
 VMX_IVA_TRASL_CXP_02
 VMX_IVA_TRASL_CXP_03

*/

namespace ConfiguracionSR
{

    public class vistasIVA
    {
        public StringBuilder vmx_iva_cxc_02 = new StringBuilder();
        public StringBuilder vmx_iva_trasl_cxp_01 = new StringBuilder();
        public StringBuilder vmx_iva_trasl_cxp_02 = new StringBuilder();
        public StringBuilder vmx_iva_trasl_cxp_03 = new StringBuilder();
    }

    public class vistasIVA_PGactivado : vistasIVA
    {
        //vistas correspondientes al cambio de la v1.0.0.12 para NHK

        public vistasIVA_PGactivado()
        {
            vmx_iva_cxc_02.Append(@"--PG_ACTIVADO
                                    ALTER VIEW [dbo].[VMX_IVA_CXC_02]
                                    AS
                                    SELECT    TOP (100) PERCENT VMX.SITE_ID, VMX.Cod_Cliente, VMX.Factura, VMX.Estatus, VMX.Monto_Deposito, VMX.BANK_ACCOUNT_ID, VMX.CHECK_ID, VMX.Cod_Deposito, VMX.TC_Deposito, 
                                                        VMX.Anio_Conc, VMX.Mes_Conc, R.TOTAL_AMOUNT, RL.LINE_NO, RL.AMOUNT, RL.VAT_AMOUNT, RL.VAT_GL_ACCT_ID, 
                                                        VMX.Monto_Deposito2 / R.TOTAL_AMOUNT * RL.VAT_AMOUNT AS MONTO_IVA_CURR, R.SELL_RATE AS TC_FACTURA, 
                                                        ROUND(VMX.Monto_Deposito2 / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * R.SELL_RATE, 2, 0) AS IVA_MXN_TRASLADAR_FACT, 
                                                        ROUND(VMX.Monto_Deposito / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * VMX.TC_Deposito, 2, 0) AS IVA_MXN_TRASLADAR_DEP, 
                                                        ROUND(VMX.Monto_Deposito / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * VMX.TC_Deposito, 2, 0) - 
                                                        ROUND(VMX.Monto_Deposito2 / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * R.SELL_RATE, 2, 0) 
                                                        AS PERDIDA_GANANCIA, 
                                                        VMX.MES_POSTEO, VMX.ANIO_POSTEO, 
                                                        CASE WHEN ROUND(VMX.Monto_Deposito / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * VMX.TC_Deposito, 2, 0) 
                                                        - ROUND(VMX.Monto_Deposito2 / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * R.SELL_RATE, 2, 0) < 0 THEN
                                                              (SELECT     GL_ACCOUNT_ID
                                                                FROM          GL_INTERFACE_ACCT
                                                                WHERE      (INTERFACE_ID = 'REALIZED_GAIN') AND (SITE_ID = R.SITE_ID)) ELSE
                                                              (SELECT     GL_ACCOUNT_ID
                                                                FROM          GL_INTERFACE_ACCT
                                                                WHERE      (INTERFACE_ID = 'REALIZED_LOSS') AND (SITE_ID = R.SITE_ID)) END AS CUENTA_PER_GANANCIA,
                                                               --SE AGREGARON CAMPOS
                                                               round((ROUND(VMX.Monto_Deposito / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * VMX.TC_Deposito, 2, 0)/round((CASE WHEN RL.VAT_PERCENT = 0 THEN 1 ELSE RL.VAT_PERCENT END/100), 2, 0)),2)Monto_Deposito_Linea,
                                                        VMX.Monto_Deposito * VMX.TC_Deposito Monto_Deposito2, -- Monto del depósito en moneda nativa

		                                                ISNULL(R.POSTING_DATE,R.CREATE_DATE)FECHA_FACTURA,
		                                                VMX.FECHA_PAGO FECHA_PAGO,
		                                                VMX.Moneda_Deposito Moneda_Pago,
		                                                R.CURRENCY_ID Moneda_Factura
						   
                                    FROM         dbo.RECEIVABLE AS R INNER JOIN
                                                          dbo.RECEIVABLE_LINE AS RL ON R.INVOICE_ID = RL.INVOICE_ID INNER JOIN
                                                          dbo.VMX_IVA_CXC_01 AS VMX ON R.INVOICE_ID = VMX.Factura AND R.SITE_ID = VMX.SITE_ID
                                    ORDER BY VMX.CHECK_ID
                                    ");



            vmx_iva_trasl_cxp_01.Append(@"--PG_ACTIVADO
                                        ALTER VIEW [dbo].[VMX_IVA_TRASL_CXP_01]
                                        AS
                                        SELECT     CDL.SITE_ID, CDL.BANK_ACCOUNT_ID, CDL.CONTROL_NO, CD.VENDOR_ID, CD.CHECK_NO, CDL.PCURR_AMOUNT AS Monto_Pago, CDL.AMOUNT AS Monto_Pago2, 
                                                              CDC.SELL_RATE AS TC_Pago, CDL.SELL_RATE AS TC_Pago2, CD.CURRENCY_ID, CDL.VOUCHER_ID, CD.CLEARED, 
					                                          YEAR(CD.CLEARED_DATE) AS ANIO_CONCILIACION, MONTH(CD.CLEARED_DATE) AS MES_CONCILIACION,                    
					                                          YEAR(CD.POSTING_DATE) AS ANIO_POSTEO, MONTH(CD.POSTING_DATE) AS MES_POSTEO,
					                                          ISNULL(CD.PAYMENT_BATCH_NO,CD.CHECK_NO) AS PAYMENT_NO,
                                                    ISNULL(CD.CLEARED_DATE,CD.POSTING_DATE) AS FECHA_PAGO,
		                                            CD.PAYMENT_BATCH_NO
                                        FROM         dbo.CASH_DISBURSE_LINE AS CDL INNER JOIN
                                                              dbo.CASH_DISBURSEMENT AS CD ON CDL.BANK_ACCOUNT_ID = CD.BANK_ACCOUNT_ID AND CDL.CONTROL_NO = CD.CONTROL_NO INNER JOIN
                                                              dbo.CASH_DISBURSE_CURR AS CDC ON CD.BANK_ACCOUNT_ID = CDC.BANK_ACCOUNT_ID AND CD.CONTROL_NO = CDC.CONTROL_NO AND 
                                                              CD.CURRENCY_ID = CDC.CURRENCY_ID
                                        WHERE     (CDL.VOUCHER_ID IS NOT NULL) AND (CD.STATUS <> 'X')
                                        ");



            vmx_iva_trasl_cxp_02.Append(@"--PG_ACTIVADO
                                        ALTER VIEW [dbo].[VMX_IVA_TRASL_CXP_02]
                                        AS
                                        SELECT     VMX.SITE_ID, VMX.BANK_ACCOUNT_ID, VMX.CONTROL_NO, VMX.VENDOR_ID, VMX.CHECK_NO, VMX.Monto_Pago, VMX.TC_Pago, VMX.CURRENCY_ID, 
                                                              VMX.VOUCHER_ID, P.INVOICE_ID, VMX.CLEARED, VMX.ANIO_CONCILIACION, VMX.MES_CONCILIACION, PL.LINE_NO, PL.VAT_AMOUNT, PL.VAT_GL_ACCT_ID, 
                                                              P.TOTAL_AMOUNT, PL.AMOUNT, P.SELL_RATE AS TC_FACTURA, CASE WHEN P.TOTAL_AMOUNT = 0 THEN 0 ELSE ROUND(VMX.TC_PAGO / P.TOTAL_AMOUNT, 3, 0) 
                                                              END AS PORC_PAGO, CASE WHEN P.TOTAL_AMOUNT = 0 THEN 0 ELSE ROUND(VMX.TC_PAGO / P.TOTAL_AMOUNT * PL.VAT_AMOUNT, 3, 0) 
                                                              END AS IVA_TRASLADO_NATIVO, ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) AS IVA_TRASLADO_MXN_FACT, 
                                                              /*
                                                              ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) AS IVA_TRASLADO_MXN_PAGO, 
                                                              ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) 
                                                              - ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) AS GANANCIA_PERDIDA, 
                                                              CASE WHEN ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) 
                                                              - ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) < 0 THEN
                                                              */
                                                              --CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago) = 0 THEN 0 ELSE ROUND(CASE WHEN VMX.TC_PAGO = 1 THEN VMX.Monto_Pago  ELSE VMX.Monto_Pago2 END / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) END AS IVA_TRASLADO_MXN_PAGO, --
                                                        
                                                        CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago) = 0 THEN 0 
			                                            ELSE ROUND(CASE WHEN VMX.TC_PAGO = 1 THEN VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago
							                                                    WHEN VMX.TC_PAGO <> 1 AND VMX.TC_PAGO2 = 1 THEN Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago2
							                                                    ELSE VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago END 
					                                                    , 2, 0) 
		                                                END AS IVA_TRASLADO_MXN_PAGO,

                                                        CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago) = 0 THEN 0 
			                                                    ELSE ROUND(CASE WHEN VMX.TC_PAGO = 1 THEN VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago
							                                                    WHEN VMX.TC_PAGO <> 1 AND VMX.TC_PAGO2 = 1 THEN Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago2
							                                                    ELSE VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago END 
					                                                    , 2, 0) 
		                                                END
		                                                -CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE)= 0 THEN 0 
			                                                    ELSE ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) 
		                                                    END  AS GANANCIA_PERDIDA,                                                                

                                                              --CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago) = 0 THEN 0 ELSE ROUND((CASE WHEN VMX.TC_PAGO = 1 THEN VMX.Monto_Pago  ELSE VMX.Monto_Pago2 END) / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) END
					                                          --CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE)= 0 THEN 0 ELSE ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) END  AS GANANCIA_PERDIDA, -- V2
					                                          --ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) - ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) AS GANANCIA_PERDIDA, V1
                                                             
                                                              CASE WHEN (CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE) = 0 THEN 0 ELSE ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) END
                                                              - CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago) = 0 THEN 0 ELSE ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0)END) < 0 THEN
                                                                  (SELECT     GL_ACCOUNT_ID
                                                                    FROM          GL_INTERFACE_ACCT
                                                                    WHERE      (INTERFACE_ID = 'REALIZED_GAIN') AND (SITE_ID = P.SITE_ID))
						                                        WHEN (CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE) = 0 THEN 0 ELSE ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) END
                                                              - CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago) = 0 THEN 0 ELSE ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0)END) > 0 THEN
                                                                  (SELECT     GL_ACCOUNT_ID
                                                                    FROM          GL_INTERFACE_ACCT
                                                                    WHERE      (INTERFACE_ID = 'REALIZED_LOSS') AND (SITE_ID = P.SITE_ID)) 
						                                        ELSE '' END AS CUENTA_PER_GANANCIA, 
                                                                    VMX.ANIO_POSTEO, VMX.MES_POSTEO,
                                                                    ROUND(CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago) = 0 THEN 0 ELSE ROUND(CASE WHEN VMX.TC_PAGO = 1 THEN VMX.Monto_Pago  ELSE VMX.Monto_Pago2 END / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) END/CASE WHEN PL.VAT_PERCENT = 0 THEN 1 ELSE round((PL.VAT_PERCENT/100), 2, 0)END,2)Monto_Deposito_Linea,
							                                        CASE WHEN VMX.TC_PAGO = 1 THEN VMX.Monto_Pago  ELSE VMX.Monto_Pago2 END * VMX.TC_Pago Monto_Pago2 --
							                                        ,ISNULL(VMX.PAYMENT_NO,VMX.CHECK_NO)PAYMENT_NO--
							                                ,ISNULL(P.POSTING_DATE,P.CREATE_DATE) FECHA_FACTURA
							                                ,VMX.FECHA_PAGO,P.CURRENCY_ID Moneda_Factura
                                        FROM         dbo.VMX_IVA_TRASL_CXP_01 AS VMX INNER JOIN
                                                              dbo.PAYABLE AS P ON VMX.VOUCHER_ID = P.VOUCHER_ID AND VMX.SITE_ID = P.SITE_ID INNER JOIN
                                                              dbo.PAYABLE_LINE AS PL ON P.VOUCHER_ID = PL.VOUCHER_ID
                                       ");



            vmx_iva_trasl_cxp_03.Append(@"--PG_ACTIVADO
                                        ALTER VIEW [dbo].[VMX_IVA_TRASL_CXP_03]
                                        AS
                                        SELECT     VMX.SITE_ID, VMX.BANK_ACCOUNT_ID, VMX.CONTROL_NO, VMX.VENDOR_ID, VMX.CHECK_NO, VMX.Monto_Pago, VMX.TC_Pago, VMX.CURRENCY_ID, 
                                                              VMX.VOUCHER_ID, P.INVOICE_ID, VMX.CLEARED, VMX.ANIO_CONCILIACION, VMX.MES_CONCILIACION, PL.LINE_NO, - PL.AMOUNT AS AMOUNT, 
                                                              PL.GL_ACCOUNT_ID, P.TOTAL_AMOUNT, P.SELL_RATE AS TC_FACTURA, CASE WHEN P.TOTAL_AMOUNT = 0 THEN 0 ELSE (- 1) 
                                                              * ROUND(VMX.TC_PAGO / P.TOTAL_AMOUNT, 3, 0) END AS PORC_PAGO, CASE WHEN P.TOTAL_AMOUNT = 0 THEN 0 ELSE (- 1) 
                                                              * ROUND((VMX.TC_PAGO / P.TOTAL_AMOUNT) * PL.AMOUNT, 3, 0) END AS RET_TRASLADO_NATIVO, ROUND((VMX.Monto_Pago2 / P.TOTAL_AMOUNT) 
                                                              * (PL.AMOUNT * - 1) * P.SELL_RATE, 2, 0) AS RETENCION_MXN_FACT, ROUND((VMX.Monto_Pago / P.TOTAL_AMOUNT) * (PL.AMOUNT * - 1) * VMX.TC_Pago, 2, 0) 
                                                              AS RETENCION_MXN_PAGO, 
                                                              ROUND((VMX.Monto_Pago / P.TOTAL_AMOUNT) * (PL.AMOUNT * - 1) * VMX.TC_Pago, 2, 0) 
                                                              - ROUND((VMX.Monto_Pago2 / P.TOTAL_AMOUNT) * (PL.AMOUNT * - 1) * P.SELL_RATE, 2, 0) AS GANANCIA_PERDIDA, 
                                                              CASE WHEN ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * (PL.AMOUNT * - 1) * P.SELL_RATE, 2, 0) 
                                                              - ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * (PL.AMOUNT * - 1) * VMX.TC_Pago, 2, 0) < 0 THEN
                                                                  (SELECT     GL_ACCOUNT_ID
                                                                    FROM          GL_INTERFACE_ACCT
                                                                    WHERE      (INTERFACE_ID = 'REALIZED_LOSS') AND (SITE_ID = P.SITE_ID)) ELSE
                                                                  (SELECT     GL_ACCOUNT_ID
                                                                    FROM          GL_INTERFACE_ACCT
                                                                    WHERE      (INTERFACE_ID = 'REALIZED_GAIN') AND (SITE_ID = P.SITE_ID)) END AS CUENTA_PER_GANANCIA, VMX.ANIO_POSTEO, VMX.MES_POSTEO, 
                                                              PL.AMOUNT AS Expr1,
					                                          VMX.PAYMENT_NO
					                                ,ISNULL(P.POSTING_DATE,P.CREATE_DATE) FECHA_FACTURA
					                                ,VMX.FECHA_PAGO,P.CURRENCY_ID Moneda_Factura
                                        FROM         dbo.VMX_IVA_TRASL_CXP_01 AS VMX INNER JOIN
                                                              dbo.PAYABLE AS P ON VMX.VOUCHER_ID = P.VOUCHER_ID AND VMX.SITE_ID = P.SITE_ID INNER JOIN
                                                              dbo.PAYABLE_LINE AS PL ON P.VOUCHER_ID = PL.VOUCHER_ID INNER JOIN
                                                              dbo.VMX_CUENTAS_RETENCION AS VCR ON PL.GL_ACCOUNT_ID = VCR.Cuenta
                                        WHERE     (VCR.Estado = N'True')

                                        ");
        }

    }

    public class vistasIVA_PGdesactivado : vistasIVA
    {
        //vistas habituales

        public vistasIVA_PGdesactivado()
        {

            //Basados en el script 08-SQL-Add-PaymentNo
            vmx_iva_cxc_02.Append(@"--PG_DESACTIVADO
                                    ALTER VIEW [dbo].[VMX_IVA_CXC_02]
                                    AS
                                    SELECT     TOP (100) PERCENT VMX.SITE_ID, VMX.Cod_Cliente, VMX.Factura, VMX.Estatus, VMX.Monto_Deposito, VMX.BANK_ACCOUNT_ID, VMX.CHECK_ID, VMX.Cod_Deposito, VMX.TC_Deposito, 
                                                            VMX.Anio_Conc, VMX.Mes_Conc, R.TOTAL_AMOUNT, RL.LINE_NO, RL.AMOUNT, RL.VAT_AMOUNT, RL.VAT_GL_ACCT_ID, 
                                                            VMX.Monto_Deposito2 / R.TOTAL_AMOUNT * RL.VAT_AMOUNT AS MONTO_IVA_CURR, R.SELL_RATE AS TC_FACTURA, 
                                                            ROUND(VMX.Monto_Deposito2 / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * R.SELL_RATE, 2, 0) AS IVA_MXN_TRASLADAR_FACT, 
                                                            ROUND(VMX.Monto_Deposito / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * VMX.TC_Deposito, 2, 0) AS IVA_MXN_TRASLADAR_DEP, 
                                                            ROUND(VMX.Monto_Deposito / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * VMX.TC_Deposito, 2, 0) - ROUND(VMX.Monto_Deposito2 / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * R.SELL_RATE, 2, 0) 
                                                            AS PERDIDA_GANANCIA, VMX.MES_POSTEO, VMX.ANIO_POSTEO, CASE WHEN ROUND(VMX.Monto_Deposito / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * VMX.TC_Deposito, 2, 0) 
                                                            - ROUND(VMX.Monto_Deposito2 / R.TOTAL_AMOUNT * RL.VAT_AMOUNT * R.SELL_RATE, 2, 0) < 0 THEN
                                                                (SELECT     GL_ACCOUNT_ID
                                                                FROM          GL_INTERFACE_ACCT
                                                                WHERE      (INTERFACE_ID = 'REALIZED_GAIN') AND (SITE_ID = R.SITE_ID)) ELSE
                                                                (SELECT     GL_ACCOUNT_ID
                                                                FROM          GL_INTERFACE_ACCT
                                                                WHERE      (INTERFACE_ID = 'REALIZED_LOSS') AND (SITE_ID = R.SITE_ID)) END AS CUENTA_PER_GANANCIA
                                                
                                                ,VMX.Monto_Deposito * VMX.TC_Deposito Monto_Deposito2, -- Monto del depósito en moneda nativa

		                                        ISNULL(R.POSTING_DATE,R.CREATE_DATE)FECHA_FACTURA,
		                                        VMX.FECHA_PAGO FECHA_PAGO,
		                                        VMX.Moneda_Deposito Moneda_Pago,
		                                        R.CURRENCY_ID Moneda_Factura

                                    FROM         dbo.RECEIVABLE AS R INNER JOIN
                                                            dbo.RECEIVABLE_LINE AS RL ON R.INVOICE_ID = RL.INVOICE_ID INNER JOIN
                                                            dbo.VMX_IVA_CXC_01 AS VMX ON R.INVOICE_ID = VMX.Factura AND R.SITE_ID = VMX.SITE_ID
                                    ORDER BY VMX.CHECK_ID
                                    ");



            vmx_iva_trasl_cxp_01.Append(@"--PG_DESACTIVADO
                                        ALTER VIEW [dbo].[VMX_IVA_TRASL_CXP_01]
                                        AS
                                        SELECT     CDL.SITE_ID, CDL.BANK_ACCOUNT_ID, CDL.CONTROL_NO, CD.VENDOR_ID, CD.CHECK_NO, CDL.PCURR_AMOUNT AS Monto_Pago, CDL.AMOUNT AS Monto_Pago2, 
                                                    CDC.SELL_RATE AS TC_Pago, CDL.SELL_RATE AS TC_Pago2, CD.CURRENCY_ID, CDL.VOUCHER_ID, CD.CLEARED, YEAR(CD.CLEARED_DATE) 
                                                    AS ANIO_CONCILIACION, MONTH(CD.CLEARED_DATE) AS MES_CONCILIACION, YEAR(CD.POSTING_DATE) AS ANIO_POSTEO, MONTH(CD.POSTING_DATE) 
                                                    AS MES_POSTEO,
                                                    ISNULL(CD.PAYMENT_BATCH_NO,CD.CHECK_NO) AS PAYMENT_NO,
                                                    ISNULL(CD.CLEARED_DATE,CD.POSTING_DATE) AS FECHA_PAGO
                                        FROM         dbo.CASH_DISBURSE_LINE AS CDL INNER JOIN
                                                                dbo.CASH_DISBURSEMENT AS CD ON CDL.BANK_ACCOUNT_ID = CD.BANK_ACCOUNT_ID AND CDL.CONTROL_NO = CD.CONTROL_NO INNER JOIN
                                                                dbo.CASH_DISBURSE_CURR AS CDC ON CD.BANK_ACCOUNT_ID = CDC.BANK_ACCOUNT_ID AND CD.CONTROL_NO = CDC.CONTROL_NO AND 
                                                                CD.CURRENCY_ID = CDC.CURRENCY_ID
                                        WHERE     (CDL.VOUCHER_ID IS NOT NULL) AND (CD.STATUS <> 'X')
                                        ");



            vmx_iva_trasl_cxp_02.Append(@"--PG_DESACTIVADO
                                        ALTER VIEW [dbo].[VMX_IVA_TRASL_CXP_02]
                                        AS
                                        SELECT     VMX.SITE_ID, VMX.BANK_ACCOUNT_ID, VMX.CONTROL_NO, VMX.VENDOR_ID, VMX.CHECK_NO, VMX.Monto_Pago, VMX.TC_Pago, VMX.CURRENCY_ID, 
                                                  VMX.VOUCHER_ID, P.INVOICE_ID, VMX.CLEARED, VMX.ANIO_CONCILIACION, VMX.MES_CONCILIACION, PL.LINE_NO, PL.VAT_AMOUNT, PL.VAT_GL_ACCT_ID, 
                                                  P.TOTAL_AMOUNT, PL.AMOUNT, P.SELL_RATE AS TC_FACTURA, CASE WHEN P.TOTAL_AMOUNT = 0 THEN 0 ELSE ROUND(VMX.TC_PAGO / P.TOTAL_AMOUNT, 3, 0) 
                                                  END AS PORC_PAGO, CASE WHEN P.TOTAL_AMOUNT = 0 THEN 0 ELSE ROUND(VMX.TC_PAGO / P.TOTAL_AMOUNT * PL.VAT_AMOUNT, 3, 0) 
                                                  END AS IVA_TRASLADO_NATIVO, ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) AS IVA_TRASLADO_MXN_FACT, 
                                                  /* v1
                                                  ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) AS IVA_TRASLADO_MXN_PAGO, 
                                                  ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) 
                                                  - ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) AS GANANCIA_PERDIDA, 
                                                  */   

                                                 
                                                        CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago) = 0 THEN 0 
			                                            ELSE ROUND(CASE WHEN VMX.TC_PAGO = 1 THEN VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago
							                                                    WHEN VMX.TC_PAGO <> 1 AND VMX.TC_PAGO2 = 1 THEN Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago2
							                                                    ELSE VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago END 
					                                                    , 2, 0) 
		                                                END AS IVA_TRASLADO_MXN_PAGO,

                                                        CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago) = 0 THEN 0 
			                                                    ELSE ROUND(CASE WHEN VMX.TC_PAGO = 1 THEN VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago
							                                                    WHEN VMX.TC_PAGO <> 1 AND VMX.TC_PAGO2 = 1 THEN Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago2
							                                                    ELSE VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago END 
					                                                    , 2, 0) 
		                                                END
		                                                -CASE WHEN (P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE)= 0 THEN 0 
			                                                    ELSE ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) 
		                                                    END  AS GANANCIA_PERDIDA,       


       
                                                  CASE WHEN ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) 
                                                  - ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) < 0 THEN
                                                      (SELECT     GL_ACCOUNT_ID
                                                        FROM          GL_INTERFACE_ACCT
                                                        WHERE      (INTERFACE_ID = 'REALIZED_LOSS') AND (SITE_ID = P.SITE_ID)) ELSE
                                                      (SELECT     GL_ACCOUNT_ID
                                                        FROM          GL_INTERFACE_ACCT
                                                        WHERE      (INTERFACE_ID = 'REALIZED_GAIN') AND (SITE_ID = P.SITE_ID)) END AS CUENTA_PER_GANANCIA, VMX.ANIO_POSTEO, VMX.MES_POSTEO,
                                                    ISNULL(VMX.PAYMENT_NO,VMX.CHECK_NO)PAYMENT_NO
							                        ,ISNULL(P.POSTING_DATE,P.CREATE_DATE) FECHA_FACTURA
							                        ,VMX.FECHA_PAGO,P.CURRENCY_ID Moneda_Factura
                                        FROM         dbo.VMX_IVA_TRASL_CXP_01 AS VMX INNER JOIN
                                                                dbo.PAYABLE AS P ON VMX.VOUCHER_ID = P.VOUCHER_ID AND VMX.SITE_ID = P.SITE_ID INNER JOIN
                                                                dbo.PAYABLE_LINE AS PL ON P.VOUCHER_ID = PL.VOUCHER_ID
                                    ");



            vmx_iva_trasl_cxp_03.Append(@"--PG_DESACTIVADO
                                        ALTER VIEW [dbo].[VMX_IVA_TRASL_CXP_03]
                                        AS
                                        SELECT     VMX.SITE_ID, VMX.BANK_ACCOUNT_ID, VMX.CONTROL_NO, VMX.VENDOR_ID, VMX.CHECK_NO, VMX.Monto_Pago, VMX.TC_Pago, VMX.CURRENCY_ID, 
                                                      VMX.VOUCHER_ID, P.INVOICE_ID, VMX.CLEARED, VMX.ANIO_CONCILIACION, VMX.MES_CONCILIACION, PL.LINE_NO, - PL.AMOUNT AS AMOUNT, 
                                                      PL.GL_ACCOUNT_ID, P.TOTAL_AMOUNT, P.SELL_RATE AS TC_FACTURA, CASE WHEN P.TOTAL_AMOUNT = 0 THEN 0 ELSE (- 1) 
                                                      * ROUND(VMX.TC_PAGO / P.TOTAL_AMOUNT, 3, 0) END AS PORC_PAGO, CASE WHEN P.TOTAL_AMOUNT = 0 THEN 0 ELSE (- 1) 
                                                      * ROUND((VMX.TC_PAGO / P.TOTAL_AMOUNT) * PL.AMOUNT, 3, 0) END AS RET_TRASLADO_NATIVO, ROUND((VMX.Monto_Pago2 / P.TOTAL_AMOUNT) 
                                                      * (PL.AMOUNT * - 1) * P.SELL_RATE, 2, 0) AS RETENCION_MXN_FACT, ROUND((VMX.Monto_Pago / P.TOTAL_AMOUNT) * (PL.AMOUNT * - 1) * VMX.TC_Pago, 2, 0) 
                                                      AS RETENCION_MXN_PAGO, ROUND((VMX.Monto_Pago2 / P.TOTAL_AMOUNT) * (PL.AMOUNT * - 1) * P.SELL_RATE, 2, 0) 
                                                      - ROUND((VMX.Monto_Pago / P.TOTAL_AMOUNT) * (PL.AMOUNT * - 1) * VMX.TC_Pago, 2, 0) AS GANANCIA_PERDIDA, 
                                                      CASE WHEN ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * (PL.AMOUNT * - 1) * P.SELL_RATE, 2, 0) 
                                                      - ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * (PL.AMOUNT * - 1) * VMX.TC_Pago, 2, 0) > 0 THEN
                                                          (SELECT     GL_ACCOUNT_ID
                                                            FROM          GL_INTERFACE_ACCT
                                                            WHERE      (INTERFACE_ID = 'REALIZED_LOSS') AND (SITE_ID = P.SITE_ID)) ELSE
                                                          (SELECT     GL_ACCOUNT_ID
                                                            FROM          GL_INTERFACE_ACCT
                                                            WHERE      (INTERFACE_ID = 'REALIZED_GAIN') AND (SITE_ID = P.SITE_ID)) END AS CUENTA_PER_GANANCIA, VMX.ANIO_POSTEO, VMX.MES_POSTEO, 
                                                      PL.AMOUNT AS Expr1,
                                                      VMX.PAYMENT_NO
					                                ,ISNULL(P.POSTING_DATE,P.CREATE_DATE) FECHA_FACTURA
					                                ,VMX.FECHA_PAGO,P.CURRENCY_ID Moneda_Factura
                                        FROM         dbo.VMX_IVA_TRASL_CXP_01 AS VMX INNER JOIN
                                                              dbo.PAYABLE AS P ON VMX.VOUCHER_ID = P.VOUCHER_ID AND VMX.SITE_ID = P.SITE_ID INNER JOIN
                                                              dbo.PAYABLE_LINE AS PL ON P.VOUCHER_ID = PL.VOUCHER_ID INNER JOIN
                                                              dbo.VMX_CUENTAS_RETENCION AS VCR ON PL.GL_ACCOUNT_ID = VCR.Cuenta
                                        WHERE     (VCR.Estado = N'True')
                                    ");

        }

    }



}
