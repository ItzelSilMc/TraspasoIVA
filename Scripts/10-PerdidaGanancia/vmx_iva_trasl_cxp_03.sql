

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
FROM         dbo.VMX_IVA_TRASL_CXP_01 AS VMX INNER JOIN
                      dbo.PAYABLE AS P ON VMX.VOUCHER_ID = P.VOUCHER_ID AND VMX.SITE_ID = P.SITE_ID INNER JOIN
                      dbo.PAYABLE_LINE AS PL ON P.VOUCHER_ID = PL.VOUCHER_ID INNER JOIN
                      dbo.VMX_CUENTAS_RETENCION AS VCR ON PL.GL_ACCOUNT_ID = VCR.Cuenta
WHERE     (VCR.Estado = N'True')




GO


