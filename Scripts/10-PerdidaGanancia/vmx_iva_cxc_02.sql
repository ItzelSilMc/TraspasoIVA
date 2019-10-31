
ALTER VIEW [dbo].[VMX_IVA_CXC_02]
AS
SELECT     TOP (100) PERCENT VMX.SITE_ID, VMX.Cod_Cliente, VMX.Factura, VMX.Estatus, VMX.Monto_Deposito, VMX.BANK_ACCOUNT_ID, VMX.CHECK_ID, VMX.Cod_Deposito, VMX.TC_Deposito, 
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
						   VMX.Monto_Deposito * VMX.TC_Deposito Monto_Deposito2
						   
FROM         dbo.RECEIVABLE AS R INNER JOIN
                      dbo.RECEIVABLE_LINE AS RL ON R.INVOICE_ID = RL.INVOICE_ID INNER JOIN
                      dbo.VMX_IVA_CXC_01 AS VMX ON R.INVOICE_ID = VMX.Factura AND R.SITE_ID = VMX.SITE_ID
ORDER BY VMX.CHECK_ID


GO


