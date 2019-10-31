﻿IF(SELECT COUNT(PROGRAM_ID) FROM APPLICATION WHERE PROGRAM_ID = 'VKIVA') > 0
BEGIN
	UPDATE APPLICATION SET LANGUAGE_ID = 'USA', MENU_STRING = 'vk-Traspaso de IVA', TYPE ='V', MODULE = 'V' 
	WHERE PROGRAM_ID = 'VKIVA'	
END
ELSE
BEGIN 
	INSERT APPLICATION(PROGRAM_ID, LANGUAGE_ID, MENU_STRING, TYPE, MODULE) 
	VALUES ('VKIVA', 'USA', 'vk-Traspaso de IVA', 'V', 'V')
END
GO

-- Indica el tipo de cambio que se tomara para el Traspaso de IVA
INSERT VMX_DIOT_TIPOCAMBIO(TIPO)VALUES('PAGO')
GO