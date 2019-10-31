-- VMXCONTROLPOLIZA

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMXCONTROLPOLIZA]') AND type in (N'U'))
	IF(SELECT COUNT(NO_TRANSACCION) FROM VMXCONTROLPOLIZA)> 0	
		PRINT 'La tabla [VMXCONTROLPOLIZA] contiene registros. Por motivos de seguridad no se puede eliminar. '
	ELSE		
		DROP TABLE VMXCONTROLPOLIZA	
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMXCONTROLPOLIZA]') AND type in (N'U'))

CREATE TABLE [dbo].[VMXCONTROLPOLIZA](
	[NO_TRANSACCION] [nvarchar](max) NULL,
	[CUENTA] [nvarchar](50) NULL,
	[MONTO] [numeric](18, 3) NULL,
	[FECHA_PERIODO] [date] NULL,
	[FECHA_TRANSACCION] [date] NULL,
	[FECHA_CREACION] [date] NULL,
	[USUARIO] [nvarchar](50) NULL,
	[TIPO_OPERACION] [nvarchar](50) NULL,
	[SITE_ID] [nvarchar](15) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

-- VMX_IVATRASTEMP *

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_IVATRASTEMP]') AND type in (N'U'))
	IF(SELECT COUNT(*) FROM VMX_IVATRASTEMP)> 0	
		PRINT 'La tabla [VMX_IVATRASTEMP] contiene registros. Por motivos de seguridad no se puede eliminar. '
	ELSE		
		DROP TABLE VMX_IVATRASTEMP	
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_IVATRASTEMP]') AND type in (N'U'))

CREATE TABLE [dbo].[VMX_IVATRASTEMP](
	[CUENTA_ORIGEN] [nvarchar](50) NULL,
	[CUENTA_DESTINO] [nvarchar](50) NULL,
	[MONTO] [numeric](18, 3) NULL,
	[MONTO_IVA_DEPOSITO] [numeric](18, 3) NULL,
	[SITE_ID] [nvarchar](15) NULL
) ON [PRIMARY]
GO

-- VMX_DATEXTPAGOS

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_DATEXTPAGOS]') AND type in (N'U'))
	IF(SELECT COUNT(*) FROM VMX_DATEXTPAGOS)> 0	
		PRINT 'La tabla [VMX_DATEXTPAGOS] contiene registros. Por motivos de seguridad no se puede eliminar. '
	ELSE		
		DROP TABLE VMX_DATEXTPAGOS	
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_DATEXTPAGOS]'))
CREATE TABLE [dbo].[VMX_DATEXTPAGOS](
	[BANK_ACOUNT] [nvarchar](50) NULL,
	[NO_CONTROL] [nvarchar](50) NULL,
	[RFC] [nvarchar](50) NULL,
	[VENDOR_NAME] [nvarchar](max) NULL,
	[LN] [nchar](10) NULL,
	[CUENTA] [nvarchar](50) NULL,
	[DESCRIPCION] [nvarchar](max) NULL,
	[MONTO_IVA] [numeric](18, 3) NULL,
	[MONTO_BASE] [numeric](18, 3) NULL,
	[TIPO_PROVEEDOR] [nvarchar](50) NULL,
	[CAUSANTE] [nvarchar](50) NULL,
	[ID_EXTRANJERO] [nvarchar](50) NULL,
	[PAIS] [nvarchar](max) NULL,
	[NACIONALIDAD] [nvarchar](50) NULL,
	[TAX_ID] [nvarchar](50) NULL,
	[TIPO_OPE] [nvarchar](50) NULL,
	[VAT_CODE] [nvarchar](50) NULL,
	[MONTO_DEDUCIBILIDAD] [numeric](18, 2) NULL,
	[PORCENTAJE_DEDUCIBILIDAD] [numeric](18, 2) NULL,
	[IVA] [numeric](18, 2) NULL,
	[RET] [numeric](18, 2) NULL,
	[MONTO_TOTAL] [numeric](18, 2) NULL,
	[MONTO_RETENCION] [numeric](18, 2) NULL,
	[VOUCHER_ID] [nvarchar](max) NULL,
	[LN_vOUCHER] [nchar](10) NULL,
	[NOMBRE_EXTRANJERO] [nvarchar](max) NULL,
	[ENTITY_ID] [nvarchar](15) NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

-- [VMX_DATEXTPROVEEDOR]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_DATEXTPROVEEDOR]') AND type in (N'U'))
	IF(SELECT COUNT(*) FROM [VMX_DATEXTPROVEEDOR])> 0	
		PRINT 'La tabla [VMX_DATEXTPROVEEDOR] contiene registros. Por motivos de seguridad no se puede eliminar. '
	ELSE		
		DROP TABLE [VMX_DATEXTPROVEEDOR]	
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_DATEXTPROVEEDOR]'))

CREATE TABLE [dbo].[VMX_DATEXTPROVEEDOR](
	[VENDOR_ID] [nvarchar](50) NULL,
	[TIPO] [nvarchar](max) NULL,
	[CAUSANTE] [nvarchar](max) NULL,
	[ID_EXTRANJERO] [nvarchar](max) NULL,
	[PAIS] [nvarchar](max) NULL,
	[NACIONALIDAD] [nvarchar](max) NULL,
	[RFC] [nvarchar](max) NULL,
	[TIPO_OPERACION] [nvarchar](max) NULL,
	[NOMBRE_EXTRANJERO] [nvarchar](max) NULL,
	[RAZON_SOCIAL] [varchar](120) NULL
) ON [PRIMARY]
GO

--   VMX_CUENTASDESCUENTOSCXC

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_CUENTASDESCUENTOSCXC]') AND type in (N'U'))
	IF(SELECT COUNT(*) FROM VMX_CUENTASDESCUENTOSCXC)> 0	
		PRINT 'La tabla [VMX_CUENTASDESCUENTOSCXC] contiene registros. Por motivos de seguridad no se puede eliminar. '
	ELSE		
		DROP TABLE VMX_CUENTASDESCUENTOSCXC	
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_CUENTASDESCUENTOSCXC]'))
CREATE TABLE [dbo].[VMX_CUENTASDESCUENTOSCXC](
	[CUENTA] [nvarchar](max) NULL
) ON [PRIMARY]
GO

--	[VMX_CONTOLPOLIZA_LINE]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_CONTOLPOLIZA_LINE]') AND type in (N'U'))
	IF(SELECT COUNT(*) FROM VMX_CONTOLPOLIZA_LINE)> 0	
		PRINT 'La tabla [VMX_CONTOLPOLIZA_LINE] contiene registros. Por motivos de seguridad no se puede eliminar. '
	ELSE		
		DROP TABLE VMX_CONTOLPOLIZA_LINE	
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_CONTOLPOLIZA_LINE]'))

CREATE TABLE [dbo].[VMX_CONTOLPOLIZA_LINE](
	[NO_TRANSACCION] [varchar](50) NULL,
	[BANK_ACCOUNT_ID] [varchar](15) NULL,
	[CONTROL_NO] [int] NULL,
	[VAT_AMOUNT] [numeric](18, 3) NULL,
	[SELL_RATE] [numeric](18, 3) NULL,
	[MONTO] [numeric](18, 3) NULL,
	[CUENTA] [varchar](50) NULL,
	[DESCRIPCION] [varchar](50) NULL,
	[CUENTA_TRASLADO] [varchar](50) NULL,
	[DESCRIPTION] [varchar](50) NULL,
	[TIPO_OPERACION] [varchar](50) NULL,
	[CHECK_ID] [varchar](15) NULL,
	[CUSTOMER_ID] [varchar](15) NULL,
	[PERDIDA_GANACIA] [numeric](18, 3) NULL,
	[IVA_DEPOSITO] [numeric](18, 3) NULL,
	[SITE_ID] [varchar](15) NULL
) ON [PRIMARY]
GO

--	VMX_IVACUENTASCXC

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_IVACUENTASCXC]') AND type in (N'U'))
	IF(SELECT COUNT(*) FROM VMX_IVACUENTASCXC)> 0	
		PRINT 'La tabla [VMX_IVACUENTASCXC] contiene registros. Por motivos de seguridad no se puede eliminar. '
	ELSE		
		DROP TABLE VMX_IVACUENTASCXC	
GO
IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_IVACUENTASCXC]'))
CREATE TABLE [dbo].[VMX_IVACUENTASCXC](
	[Estado] [nchar](10) NULL,
	[Cuenta] [nvarchar](50) NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Percent_IVA] [numeric](18, 2) NULL,
	[Retencion] [numeric](18, 2) NULL,
	[Traslado] [nvarchar](50) NULL
) ON [PRIMARY]
GO

-- [VMX_DIOT_TIPOCAMBIO]
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VMX_DIOT_TIPOCAMBIO]') AND type in (N'U'))
	DROP TABLE [dbo].[VMX_DIOT_TIPOCAMBIO]
GO

CREATE TABLE [dbo].[VMX_DIOT_TIPOCAMBIO](
	[TIPO] [varchar](20) NULL,
	[ENTITY_ID] [nvarchar](15) NULL
) ON [PRIMARY]


IF NOT EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'VMX_CONTOLPOLIZA_LINE' AND COLUMN_NAME = 'PERDIDA_GANACIA') 
	ALTER TABLE VMX_CONTOLPOLIZA_LINE ADD PERDIDA_GANACIA NUMERIC(18, 3)
GO
	
IF NOT EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'VMX_CONTOLPOLIZA_LINE' AND COLUMN_NAME = 'IVA_DEPOSITO') 
	ALTER TABLE VMX_CONTOLPOLIZA_LINE ADD IVA_DEPOSITO NUMERIC(18, 3)	
GO

IF NOT EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'VMX_IVATRASTEMP' AND COLUMN_NAME = 'MONTO_IVA_DEPOSITO') 
	ALTER TABLE VMX_IVATRASTEMP ADD MONTO_IVA_DEPOSITO NUMERIC(18, 3) 
GO

-- Indica el tipo de cambio que se tomara para el Traspaso de IVA
INSERT VMX_DIOT_TIPOCAMBIO(TIPO)VALUES('PAGO')
GO



