

ALTER VIEW [dbo].[VMX_IVA_TRASL_CXP_01]
AS
SELECT     CDL.SITE_ID, CDL.BANK_ACCOUNT_ID, CDL.CONTROL_NO, CD.VENDOR_ID, CD.CHECK_NO, CDL.PCURR_AMOUNT AS Monto_Pago, CDL.AMOUNT AS Monto_Pago2, 
                      CDC.SELL_RATE AS TC_Pago, CDL.SELL_RATE AS TC_Pago2, CD.CURRENCY_ID, CDL.VOUCHER_ID, CD.CLEARED, YEAR(CD.CLEARED_DATE) 
                      AS ANIO_CONCILIACION, MONTH(CD.CLEARED_DATE) AS MES_CONCILIACION, YEAR(CD.POSTING_DATE) AS ANIO_POSTEO, MONTH(CD.POSTING_DATE) 
                      AS MES_POSTEO,CD.PAYMENT_BATCH_NO AS PAYMENT_NO
FROM         dbo.CASH_DISBURSE_LINE AS CDL INNER JOIN
                      dbo.CASH_DISBURSEMENT AS CD ON CDL.BANK_ACCOUNT_ID = CD.BANK_ACCOUNT_ID AND CDL.CONTROL_NO = CD.CONTROL_NO INNER JOIN
                      dbo.CASH_DISBURSE_CURR AS CDC ON CD.BANK_ACCOUNT_ID = CDC.BANK_ACCOUNT_ID AND CD.CONTROL_NO = CDC.CONTROL_NO AND 
                      CD.CURRENCY_ID = CDC.CURRENCY_ID
WHERE     (CDL.VOUCHER_ID IS NOT NULL) AND (CD.STATUS <> 'X')



GO


ALTER VIEW [dbo].[VMX_IVA_TRASL_CXP_02]
AS
SELECT     VMX.SITE_ID, VMX.BANK_ACCOUNT_ID, VMX.CONTROL_NO, VMX.VENDOR_ID, VMX.CHECK_NO, VMX.Monto_Pago, VMX.TC_Pago, VMX.CURRENCY_ID, 
                      VMX.VOUCHER_ID, P.INVOICE_ID, VMX.CLEARED, VMX.ANIO_CONCILIACION, VMX.MES_CONCILIACION, PL.LINE_NO, PL.VAT_AMOUNT, PL.VAT_GL_ACCT_ID, 
                      P.TOTAL_AMOUNT, PL.AMOUNT, P.SELL_RATE AS TC_FACTURA, CASE WHEN P.TOTAL_AMOUNT = 0 THEN 0 ELSE ROUND(VMX.TC_PAGO / P.TOTAL_AMOUNT, 3, 0) 
                      END AS PORC_PAGO, CASE WHEN P.TOTAL_AMOUNT = 0 THEN 0 ELSE ROUND(VMX.TC_PAGO / P.TOTAL_AMOUNT * PL.VAT_AMOUNT, 3, 0) 
                      END AS IVA_TRASLADO_NATIVO, ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) AS IVA_TRASLADO_MXN_FACT, 
                      ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) AS IVA_TRASLADO_MXN_PAGO, 
                      ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) 
                      - ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) AS GANANCIA_PERDIDA, 
                      CASE WHEN ROUND(VMX.Monto_Pago2 / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * P.SELL_RATE, 2, 0) 
                      - ROUND(VMX.Monto_Pago / P.TOTAL_AMOUNT * PL.VAT_AMOUNT * VMX.TC_Pago, 2, 0) < 0 THEN
                          (SELECT     GL_ACCOUNT_ID
                            FROM          GL_INTERFACE_ACCT
                            WHERE      (INTERFACE_ID = 'REALIZED_LOSS') AND (SITE_ID = P.SITE_ID)) ELSE
                          (SELECT     GL_ACCOUNT_ID
                            FROM          GL_INTERFACE_ACCT
                            WHERE      (INTERFACE_ID = 'REALIZED_GAIN') AND (SITE_ID = P.SITE_ID)) END AS CUENTA_PER_GANANCIA, VMX.ANIO_POSTEO, VMX.MES_POSTEO,VMX.PAYMENT_NO
FROM         dbo.VMX_IVA_TRASL_CXP_01 AS VMX INNER JOIN
                      dbo.PAYABLE AS P ON VMX.VOUCHER_ID = P.VOUCHER_ID AND VMX.SITE_ID = P.SITE_ID INNER JOIN
                      dbo.PAYABLE_LINE AS PL ON P.VOUCHER_ID = PL.VOUCHER_ID


GO

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

ALTER PROCEDURE [dbo].[PRC_POLIZAS_POR_TRASPASAR_CXP_DET] (@ANIO INT,@MES INT, @FECHA VARCHAR(15),@CONSILIADO INT, @SITE_ID VARCHAR(15))
AS

	SET NOCOUNT ON;
  CREATE TABLE #POLIZASCXP(	 
	BANK_ACCOUNT_ID VARCHAR(15),
    CONTROL_NO INT,
    VAT_GL_ACCT_ID VARCHAR(30),
    DESCRIPCION VARCHAR(200),
    TRASLADO VARCHAR(30),
    TC_FACTURA DECIMAL(15,8),
    TC_DEPOSITO DECIMAL(15,8),    
    IVA_MXN_TRASLADAR_FACT DECIMAL(15,2),
    IVA_MXN_TRASLADAR_DEP DECIMAL(15,2),
    RET_MXN_TRASLADAR_FACT DECIMAL(15,2),
    RET_MXN_TRASLADAR_DEP DECIMAL(15,2),
    PERDIDA_GANANCIA DECIMAL(15,2),
    CUENTA_PER_GANANCIA VARCHAR(30),
    FECHA VARCHAR(10),
    CLIENTE VARCHAR(120),
    SITE_ID VARCHAR(15),
    VOUCHER VARCHAR(15),
    VOUCHER_LN VARCHAR(10),
    RFC VARCHAR(25),
    NAME VARCHAR(50),
    AMOUNT DECIMAL(15,2),
    INVOICE_ID VARCHAR(15),
    PAYMENT_NO INT
    );

	CREATE TABLE #POLIZASNEGATIVAS( 
	BANK_ACCOUNT_ID VARCHAR(15),
    CONTROL_NO INT,
    SITE_ID VARCHAR(15));
	
	IF (@CONSILIADO = 1)
	BEGIN
		-- Obtener todos los movimientos consiliados		
		INSERT #POLIZASCXP(SITE_ID, BANK_ACCOUNT_ID, CONTROL_NO, VAT_GL_ACCT_ID, DESCRIPCION, TRASLADO, 
			TC_FACTURA, IVA_MXN_TRASLADAR_FACT,RET_MXN_TRASLADAR_FACT, TC_DEPOSITO, IVA_MXN_TRASLADAR_DEP, RET_MXN_TRASLADAR_DEP,PERDIDA_GANANCIA, CUENTA_PER_GANANCIA, FECHA, CLIENTE
			,VOUCHER,VOUCHER_LN,RFC,NAME,AMOUNT,INVOICE_ID,PAYMENT_NO)
			SELECT A.SITE_ID
				, A.BANK_ACCOUNT_ID 
				,A.CONTROL_NO 
				,A.VAT_GL_ACCT_ID
				,B.DESCRIPCION
				,B.TRASLADO
				,A.TC_FACTURA
				-- Hacer cero el monto, cuando el Monto de IVA prorrateado sea un centavo positivo o negativo.
				,CASE 
					WHEN SUM(A.IVA_TRASLADO_MXN_FACT) = 0.01 THEN 0 
					WHEN SUM(A.IVA_TRASLADO_MXN_FACT) = -0.01 THEN 0 
				ELSE SUM(A.IVA_TRASLADO_MXN_FACT)  END
				,0
				,A.TC_Pago				
				,CASE 
					WHEN SUM(A.GANANCIA_PERDIDA) = 0.01 THEN A.IVA_TRASLADO_MXN_PAGO + 0.01
					WHEN SUM(A.GANANCIA_PERDIDA) = -0.01 THEN A.IVA_TRASLADO_MXN_PAGO - 0.01
				ELSE SUM(A.IVA_TRASLADO_MXN_PAGO) END AS IVA_MXN_TRASLADAR_DEP	
				,0
				,CASE 
					WHEN SUM(A.GANANCIA_PERDIDA) = 0.01 THEN 0 
					WHEN SUM(A.GANANCIA_PERDIDA) = -0.01 THEN 0 
				ELSE SUM(A.GANANCIA_PERDIDA)END
				,A.CUENTA_PER_GANANCIA
				,@FECHA
				,A.VENDOR_ID
				,A.VOUCHER_ID
				,A.LINE_NO
				,isnull(V.VAT_REGISTRATION,'')
				,V.NAME
				,A.AMOUNT
				,A.INVOICE_ID
				,A.PAYMENT_NO
			FROM VMX_DIOTCUENTAS AS b,VMX_IVA_TRASL_CXP_02 AS a --CASH_DISBURSE
			INNER JOIN VENDOR V ON V.ID = A.VENDOR_ID
			WHERE b.Cuenta =  a.VAT_GL_ACCT_ID
				AND b.Estado = 'True'
				AND a.ANIO_CONCILIACION = @ANIO
				AND a.MES_CONCILIACION = @MES	
				AND a.SITE_ID = @SITE_ID			
			GROUP BY a.SITE_ID, a.BANK_ACCOUNT_ID,a.CONTROL_NO,a.VAT_GL_ACCT_ID,b.Descripcion,a.TC_FACTURA,b.Traslado,A.IVA_TRASLADO_MXN_FACT,
				A.TC_Pago,A.IVA_TRASLADO_MXN_PAGO,A.GANANCIA_PERDIDA,A.CUENTA_PER_GANANCIA,a.VENDOR_ID
				,A.VOUCHER_ID
				,A.LINE_NO
				,V.VAT_REGISTRATION
				,V.NAME
				,AMOUNT
				,A.INVOICE_ID
				,A.PAYMENT_NO
				
			-- Obtener los movimientos con montos negativos
			INSERT #POLIZASNEGATIVAS(SITE_ID,BANK_ACCOUNT_ID,CONTROL_NO)
				SELECT A.SITE_ID, A.BANK_ACCOUNT_ID,A.CONTROL_NO      
				FROM VMX_IVA_TRASL_CXP_02 AS a,VMX_DIOTCUENTAS AS b 
				WHERE b.Cuenta =  a.VAT_GL_ACCT_ID AND b.Estado = 'True' 
					-- Consiliados
					AND a.ANIO_CONCILIACION = @ANIO AND a.MES_CONCILIACION = @MES AND a.SITE_ID = @SITE_ID
				GROUP BY a.SITE_ID, a.BANK_ACCOUNT_ID,a.CONTROL_NO	,a.VAT_GL_ACCT_ID        
				HAVING CASE WHEN SUM(A.IVA_TRASLADO_MXN_FACT) = 0.01 THEN 0 WHEN SUM(A.IVA_TRASLADO_MXN_FACT) = -0.01 THEN 0 ELSE SUM(A.IVA_TRASLADO_MXN_FACT)END < 0
	END
	ELSE
	BEGIN
		-- Obtener todos los movimientos consiliados y no consiliados
		INSERT #POLIZASCXP(SITE_ID, BANK_ACCOUNT_ID, CONTROL_NO, VAT_GL_ACCT_ID, DESCRIPCION, TRASLADO, 
			TC_FACTURA, IVA_MXN_TRASLADAR_FACT, RET_MXN_TRASLADAR_FACT,TC_DEPOSITO, IVA_MXN_TRASLADAR_DEP,RET_MXN_TRASLADAR_DEP, PERDIDA_GANANCIA, CUENTA_PER_GANANCIA, FECHA, CLIENTE
			,VOUCHER,VOUCHER_LN,RFC,NAME,AMOUNT,INVOICE_ID,A.PAYMENT_NO)
			SELECT A.SITE_ID
				,A.BANK_ACCOUNT_ID 
				,A.CONTROL_NO 
				,A.VAT_GL_ACCT_ID
				,B.DESCRIPCION
				,B.TRASLADO
				,A.TC_FACTURA
				-- Hacer cero el monto, cuando el Monto de IVA prorrateado sea un centavo positivo o negativo.
				,CASE 
					WHEN SUM(A.IVA_TRASLADO_MXN_FACT) = 0.01 THEN 0 
					WHEN SUM(A.IVA_TRASLADO_MXN_FACT) = -0.01 THEN 0 
				ELSE SUM(A.IVA_TRASLADO_MXN_FACT)  END
				,0
				,A.TC_Pago				
				,CASE 
					WHEN SUM(A.GANANCIA_PERDIDA) = 0.01 THEN A.IVA_TRASLADO_MXN_PAGO + 0.01
					WHEN SUM(A.GANANCIA_PERDIDA) = -0.01 THEN A.IVA_TRASLADO_MXN_PAGO - 0.01
				ELSE SUM(A.IVA_TRASLADO_MXN_PAGO) END AS IVA_MXN_TRASLADAR_DEP	
				,0
				,CASE 
					WHEN SUM(A.GANANCIA_PERDIDA) = 0.01 THEN 0 
					WHEN SUM(A.GANANCIA_PERDIDA) = -0.01 THEN 0 
				ELSE SUM(A.GANANCIA_PERDIDA)END
				,A.CUENTA_PER_GANANCIA
				,@FECHA
				,A.VENDOR_ID
				,A.VOUCHER_ID
				,A.LINE_NO
				,isnull(V.VAT_REGISTRATION,'')
				,V.NAME
				,A.AMOUNT
				,A.INVOICE_ID
				,A.PAYMENT_NO
			FROM VMX_DIOTCUENTAS AS b,VMX_IVA_TRASL_CXP_02 AS a --CASH_DISBURSE
			INNER JOIN VENDOR V ON V.ID = A.VENDOR_ID
			WHERE b.Cuenta =  a.VAT_GL_ACCT_ID
				AND b.Estado = 'True'
				AND a.ANIO_POSTEO = @ANIO
				AND a.MES_POSTEO = @MES
				AND a.SITE_ID = @SITE_ID
			GROUP BY a.SITE_ID, a.BANK_ACCOUNT_ID,a.CONTROL_NO,a.VAT_GL_ACCT_ID,b.Descripcion,a.TC_FACTURA,b.Traslado,A.IVA_TRASLADO_MXN_FACT,
				A.TC_Pago,A.IVA_TRASLADO_MXN_PAGO,A.GANANCIA_PERDIDA,A.CUENTA_PER_GANANCIA,a.VENDOR_ID
				,A.VOUCHER_ID
				,A.LINE_NO
				,V.VAT_REGISTRATION
				,V.NAME
				,AMOUNT
				,A.INVOICE_ID
				,A.PAYMENT_NO
		
		-- Obtener los movimientos con montos negativos
		INSERT #POLIZASNEGATIVAS(SITE_ID, BANK_ACCOUNT_ID,CONTROL_NO)
			SELECT A.SITE_ID, A.BANK_ACCOUNT_ID,A.CONTROL_NO      
			FROM VMX_IVA_TRASL_CXP_02 AS a,VMX_DIOTCUENTAS AS b --CASH_DISBURSE
			WHERE b.Cuenta =  a.VAT_GL_ACCT_ID AND b.Estado = 'True' 
				-- No Consiliados
				AND a.ANIO_POSTEO = @ANIO AND a.MES_POSTEO = @MES AND a.SITE_ID = @SITE_ID
			GROUP BY a.SITE_ID, a.BANK_ACCOUNT_ID,a.CONTROL_NO	,a.VAT_GL_ACCT_ID        
			HAVING CASE WHEN SUM(A.IVA_TRASLADO_MXN_FACT) = 0.01 THEN 0 WHEN SUM(A.IVA_TRASLADO_MXN_FACT) = -0.01 THEN 0 ELSE SUM(A.IVA_TRASLADO_MXN_FACT)END < 0
	END		
    
    -- Eliminar las polizas negativas. Filtrar por Banco y No. Control                        
	DELETE FROM #POLIZASCXP 
	FROM #POLIZASCXP AS P LEFT JOIN #POLIZASNEGATIVAS AS N ON P.BANK_ACCOUNT_ID = N.BANK_ACCOUNT_ID AND P.CONTROL_NO = N.CONTROL_NO AND P.SITE_ID = N.SITE_ID
	WHERE N.BANK_ACCOUNT_ID IS NOT NULL AND N.CONTROL_NO IS NOT NULL
	
	-- Eliminar las polizas que han completado el traspaso de IVA.
	DELETE FROM #POLIZASCXP
	FROM #POLIZASCXP AS P 
		LEFT JOIN VMX_CONTOLPOLIZA_LINE AS PL 
			ON P.BANK_ACCOUNT_ID = PL.BANK_ACCOUNT_ID AND P.CONTROL_NO = PL.CONTROL_NO AND P.VAT_GL_ACCT_ID = PL.CUENTA AND P.SITE_ID = PL.SITE_ID --AND PL.CUSTOMER_ID = P.CLIENTE
	WHERE PL.BANK_ACCOUNT_ID IS NOT NULL AND PL.CONTROL_NO IS NOT NULL AND PL.CUENTA IS NOT NULL
		AND PL.TIPO_OPERACION = 'CXP'
	
	SELECT SITE_ID
		,BANK_ACCOUNT_ID AS Banco
		,CONTROL_NO AS "No. Control"
		,CLIENTE AS Proveedor
		,RFC AS "RFC"
		,NAME AS "Nombre Proveedor"
		,VAT_GL_ACCT_ID AS Cuenta
		,DESCRIPCION AS "Descripci�n"
		,TRASLADO AS "Cuenta Traslado"
		,VOUCHER AS Voucher
		,VOUCHER_LN AS "Linea Voucher"
		,INVOICE_ID AS Factura
		,AMOUNT AS Monto
		,TC_FACTURA AS "Tipo Cambio Factura"
		,TC_DEPOSITO AS "Tipo Cambio Deposito"
		,ISNULL(IVA_MXN_TRASLADAR_FACT,0.0) AS "IVA Factura"
		,ISNULL(IVA_MXN_TRASLADAR_DEP,0.0) AS "IVA Deposito"
		,ISNULL(RET_MXN_TRASLADAR_FACT,0.0) AS "Retenci�n Factura"
		,ISNULL(RET_MXN_TRASLADAR_DEP,0.0) AS "Retenci�n Deposito"
		,ISNULL(PERDIDA_GANANCIA,0.0) AS "Perdida Ganancia"
		,CUENTA_PER_GANANCIA AS "Cuenta Perdida Ganancia"
		,FECHA AS Fecha
		,PAYMENT_NO "Payment No"

	FROM #POLIZASCXP 
	ORDER BY BANK_ACCOUNT_ID,CONTROL_NO,VOUCHER,VOUCHER_LN

	DROP TABLE #POLIZASCXP                    
	DROP TABLE #POLIZASNEGATIVAS


GO

ALTER PROCEDURE [dbo].[PRC_POLIZAS_POR_TRASPASAR_RET_CXP_DET] (@ANIO INT,@MES INT, @FECHA VARCHAR(15),@CONSILIADO INT, @SITE_ID VARCHAR(15))
AS
	SET NOCOUNT ON;

    CREATE TABLE #POLIZASCXP(	 
	BANK_ACCOUNT_ID VARCHAR(15),
    CONTROL_NO INT,
    VAT_GL_ACCT_ID VARCHAR(30),
    DESCRIPCION VARCHAR(200),
    TRASLADO VARCHAR(30),
    TC_FACTURA DECIMAL(15,8),
    TC_DEPOSITO DECIMAL(15,8),    
    IVA_MXN_TRASLADAR_FACT DECIMAL(15,2),
    IVA_MXN_TRASLADAR_DEP DECIMAL(15,2),
    RET_MXN_TRASLADAR_FACT DECIMAL(15,2),
    RET_MXN_TRASLADAR_DEP DECIMAL(15,2),
    PERDIDA_GANANCIA DECIMAL(15,2),
    CUENTA_PER_GANANCIA VARCHAR(30),
    FECHA VARCHAR(10),
    CLIENTE VARCHAR(120),
    SITE_ID VARCHAR(15),
    VOUCHER VARCHAR(15),
    VOUCHER_LN VARCHAR(10),
    RFC VARCHAR(25),
    NAME VARCHAR(50),
    AMOUNT DECIMAL(15,2),
    INVOICE_ID VARCHAR(15),
    PAYMENT_NO INT
    );

	CREATE TABLE #POLIZASNEGATIVAS( 
	BANK_ACCOUNT_ID VARCHAR(15),
    CONTROL_NO INT,
    SITE_ID VARCHAR(15));
	
	IF (@CONSILIADO = 1)
	BEGIN
		-- Obtener todos los movimientos consiliados RETENCION		
		INSERT #POLIZASCXP(SITE_ID, BANK_ACCOUNT_ID, CONTROL_NO, VAT_GL_ACCT_ID, DESCRIPCION, TRASLADO, 
			TC_FACTURA, IVA_MXN_TRASLADAR_FACT,RET_MXN_TRASLADAR_FACT, TC_DEPOSITO, IVA_MXN_TRASLADAR_DEP,RET_MXN_TRASLADAR_DEP, PERDIDA_GANANCIA, CUENTA_PER_GANANCIA, FECHA, CLIENTE
			,VOUCHER,VOUCHER_LN,RFC,NAME,AMOUNT,INVOICE_ID,PAYMENT_NO)
			SELECT	A.SITE_ID
				, A.BANK_ACCOUNT_ID 
				,A.CONTROL_NO 
				,A.GL_ACCOUNT_ID
				,B.DESCRIPCION
				,B.TRASLADO
				,A.TC_FACTURA
				,0 --IVA no aplica por que es retenci�n
				,CASE 
					WHEN SUM(A.RETENCION_MXN_FACT) = 0.01 THEN 0 
					WHEN SUM(A.RETENCION_MXN_FACT) = -0.01 THEN 0 
				ELSE SUM(A.RETENCION_MXN_FACT)  END RET_MXN_TRASLADAR_FACT
				,A.TC_Pago	
				,0 --IVA no aplica por que es retenci�n			
				,CASE 
					WHEN SUM(A.GANANCIA_PERDIDA) = 0.01 THEN A.RETENCION_MXN_PAGO + 0.01
					WHEN SUM(A.GANANCIA_PERDIDA) = -0.01 THEN A.RETENCION_MXN_PAGO - 0.01
				ELSE SUM(A.RETENCION_MXN_PAGO) END AS RET_MXN_TRASLADAR_DEP	
				,CASE 
					WHEN SUM(A.GANANCIA_PERDIDA) = 0.01 THEN 0 
					WHEN SUM(A.GANANCIA_PERDIDA) = -0.01 THEN 0 
				ELSE SUM(A.GANANCIA_PERDIDA)END
				,A.CUENTA_PER_GANANCIA
				,@FECHA
				,A.VENDOR_ID
				,A.VOUCHER_ID
				,A.LINE_NO
				,isnull(V.VAT_REGISTRATION,'')
				,V.NAME
				,A.AMOUNT
				,A.INVOICE_ID
				,A.PAYMENT_NO
			FROM VMX_CUENTAS_RETENCION AS b,VMX_IVA_TRASL_CXP_03 AS a
			INNER JOIN VENDOR V ON V.ID = A.VENDOR_ID
			WHERE b.Cuenta =  a.GL_ACCOUNT_ID AND b.Estado = 'True' 
				AND a.ANIO_CONCILIACION = @ANIO
				AND a.MES_CONCILIACION = @MES	
				AND a.SITE_ID = @SITE_ID			
			GROUP BY a.SITE_ID, a.BANK_ACCOUNT_ID,a.CONTROL_NO,a.GL_ACCOUNT_ID,b.Descripcion,a.TC_FACTURA,b.Traslado,A.RETENCION_MXN_FACT,A.TC_Pago,A.RETENCION_MXN_PAGO,A.GANANCIA_PERDIDA,A.CUENTA_PER_GANANCIA,a.VENDOR_ID
				,A.VOUCHER_ID
				,A.LINE_NO
				,V.VAT_REGISTRATION
				,V.NAME
				,A.AMOUNT
				,A.INVOICE_ID
				,A.PAYMENT_NO
			
			-- Obtener los movimientos con montos negativos retencion
			INSERT #POLIZASNEGATIVAS(SITE_ID,BANK_ACCOUNT_ID,CONTROL_NO)
				SELECT A.SITE_ID, A.BANK_ACCOUNT_ID,A.CONTROL_NO      
				FROM VMX_IVA_TRASL_CXP_03 AS a,VMX_CUENTAS_RETENCION AS b 
				WHERE b.Cuenta =  a.GL_ACCOUNT_ID AND b.Estado = 'True' 
					-- Consiliados
					AND a.ANIO_CONCILIACION = @ANIO AND a.MES_CONCILIACION = @MES AND a.SITE_ID = @SITE_ID
				GROUP BY a.SITE_ID, a.BANK_ACCOUNT_ID,a.CONTROL_NO	,a.GL_ACCOUNT_ID        
				HAVING CASE WHEN SUM(A.RETENCION_MXN_FACT) = 0.01 THEN 0 WHEN SUM(A.RETENCION_MXN_FACT) = -0.01 THEN 0 ELSE SUM(A.RETENCION_MXN_FACT)END < 0

	END
	ELSE
	BEGIN
		-- Obtener todos los movimientos consiliados y no consiliados RETENCION
		INSERT #POLIZASCXP(SITE_ID, BANK_ACCOUNT_ID, CONTROL_NO, VAT_GL_ACCT_ID, DESCRIPCION, TRASLADO, 
			TC_FACTURA, IVA_MXN_TRASLADAR_FACT,RET_MXN_TRASLADAR_FACT, TC_DEPOSITO, IVA_MXN_TRASLADAR_DEP,RET_MXN_TRASLADAR_DEP, PERDIDA_GANANCIA, CUENTA_PER_GANANCIA, FECHA, CLIENTE
			,VOUCHER,VOUCHER_LN,RFC,NAME,AMOUNT,INVOICE_ID,PAYMENT_NO)
			SELECT A.SITE_ID
				,A.BANK_ACCOUNT_ID 
				,A.CONTROL_NO 
				,A.GL_ACCOUNT_ID
				,B.DESCRIPCION
				,B.TRASLADO
				,A.TC_FACTURA
				,0
				-- Hacer cero el monto, cuando el Monto de RETENCION prorrateado sea un centavo positivo o negativo.
				,CASE 
					WHEN SUM(A.RETENCION_MXN_FACT) = 0.01 THEN 0 
					WHEN SUM(A.RETENCION_MXN_FACT) = -0.01 THEN 0 
				ELSE SUM(A.RETENCION_MXN_FACT)  END AS RET_MXN_TRASLADAR_FACT
				,A.TC_Pago		
				,0		
				,CASE 
					WHEN SUM(A.GANANCIA_PERDIDA) = 0.01 THEN A.RETENCION_MXN_PAGO + 0.01
					WHEN SUM(A.GANANCIA_PERDIDA) = -0.01 THEN A.RETENCION_MXN_PAGO - 0.01
				ELSE SUM(A.RETENCION_MXN_PAGO) END AS RET_MXN_TRASLADAR_DEP	
				,CASE 
					WHEN SUM(A.GANANCIA_PERDIDA) = 0.01 THEN 0 
					WHEN SUM(A.GANANCIA_PERDIDA) = -0.01 THEN 0 
				ELSE SUM(A.GANANCIA_PERDIDA)END
				,A.CUENTA_PER_GANANCIA
				,@FECHA
				,A.VENDOR_ID
				,A.VOUCHER_ID
				,A.LINE_NO
				,isnull(V.VAT_REGISTRATION,'')
				,V.NAME
				,A.AMOUNT
				,A.INVOICE_ID
				,A.PAYMENT_NO
			FROM VMX_CUENTAS_RETENCION AS b,VMX_IVA_TRASL_CXP_03 AS a
			INNER JOIN VENDOR V ON V.ID = A.VENDOR_ID
			WHERE b.Cuenta =  a.GL_ACCOUNT_ID
				AND b.Estado = 'True'
				AND a.ANIO_POSTEO = @ANIO
				AND a.MES_POSTEO = @MES
				AND a.SITE_ID = @SITE_ID
			GROUP BY a.SITE_ID, a.BANK_ACCOUNT_ID,a.CONTROL_NO,a.GL_ACCOUNT_ID,b.Descripcion,a.TC_FACTURA,b.Traslado,A.RETENCION_MXN_FACT,
				A.TC_Pago,A.RETENCION_MXN_PAGO,A.GANANCIA_PERDIDA,A.CUENTA_PER_GANANCIA,a.VENDOR_ID
				,A.VOUCHER_ID
				,A.LINE_NO
				,V.VAT_REGISTRATION
				,V.NAME
				,A.AMOUNT
				,A.INVOICE_ID
				,A.PAYMENT_NO
		
		-- Obtener los movimientos con montos negativos RETENCION
		INSERT #POLIZASNEGATIVAS(SITE_ID, BANK_ACCOUNT_ID,CONTROL_NO)
			SELECT A.SITE_ID, A.BANK_ACCOUNT_ID,A.CONTROL_NO      
			FROM VMX_IVA_TRASL_CXP_03 AS a,VMX_DIOTCUENTAS AS b 
			WHERE b.Cuenta =  a.GL_ACCOUNT_ID AND b.Estado = 'True' 
				-- No Consiliados
				AND a.ANIO_POSTEO = @ANIO AND a.MES_POSTEO = @MES AND a.SITE_ID = @SITE_ID
			GROUP BY a.SITE_ID, a.BANK_ACCOUNT_ID,a.CONTROL_NO	,a.GL_ACCOUNT_ID,A.VOUCHER_ID,A.LINE_NO    
			HAVING CASE WHEN SUM(A.RETENCION_MXN_FACT) = 0.01 THEN 0 WHEN SUM(A.RETENCION_MXN_FACT) = -0.01 THEN 0 ELSE SUM(A.RETENCION_MXN_FACT)END < 0
	END		
    
    -- Eliminar las polizas negativas. Filtrar por Banco y No. Control                        
	DELETE FROM #POLIZASCXP 
	FROM #POLIZASCXP AS P LEFT JOIN #POLIZASNEGATIVAS AS N ON P.BANK_ACCOUNT_ID = N.BANK_ACCOUNT_ID AND P.CONTROL_NO = N.CONTROL_NO AND P.SITE_ID = N.SITE_ID
	WHERE N.BANK_ACCOUNT_ID IS NOT NULL AND N.CONTROL_NO IS NOT NULL
	
	-- Eliminar las polizas que han completado el traspaso de RETENCION.
	DELETE FROM #POLIZASCXP
	FROM #POLIZASCXP AS P 
		LEFT JOIN VMX_CONTOLPOLIZA_LINE AS PL 
			ON P.BANK_ACCOUNT_ID = PL.BANK_ACCOUNT_ID AND P.CONTROL_NO = PL.CONTROL_NO AND P.VAT_GL_ACCT_ID = PL.CUENTA AND P.SITE_ID = PL.SITE_ID --AND PL.CUSTOMER_ID = P.CLIENTE
	WHERE PL.BANK_ACCOUNT_ID IS NOT NULL AND PL.CONTROL_NO IS NOT NULL AND PL.CUENTA IS NOT NULL
		AND PL.TIPO_OPERACION = 'CXP'
	
	SELECT SITE_ID
		,BANK_ACCOUNT_ID AS Banco
		,CONTROL_NO AS "No. Control"
		,CLIENTE AS Proveedor
		,RFC AS "RFC"
		,NAME AS "Nombre Proveedor"
		,VAT_GL_ACCT_ID AS Cuenta
		,DESCRIPCION AS "Descripci�n"
		,TRASLADO AS "Cuenta Traslado"
		,VOUCHER AS Voucher
		,VOUCHER_LN AS "Linea Voucher"
		,INVOICE_ID AS Factura
		,AMOUNT AS Monto
		,TC_FACTURA AS "Tipo Cambio Factura"
		,TC_DEPOSITO AS "Tipo Cambio Deposito"
		,ISNULL(IVA_MXN_TRASLADAR_FACT,0.0) AS "IVA Factura"
		,ISNULL(IVA_MXN_TRASLADAR_DEP,0.0) AS "IVA Deposito"
		,ISNULL(RET_MXN_TRASLADAR_FACT,0.0) AS "Retenci�n Factura"
		,ISNULL(RET_MXN_TRASLADAR_DEP,0.0) AS "Retenci�n Deposito"
		,ISNULL(PERDIDA_GANANCIA,0.0) AS "Perdida Ganancia"
		,CUENTA_PER_GANANCIA AS "Cuenta Perdida Ganancia"
		,FECHA AS Fecha
		,PAYMENT_NO "Payment No"
	FROM #POLIZASCXP 
	ORDER BY BANK_ACCOUNT_ID,CONTROL_NO,VOUCHER,VOUCHER_LN


	DROP TABLE #POLIZASCXP                    
	DROP TABLE #POLIZASNEGATIVAS
GO