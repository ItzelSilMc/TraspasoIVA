using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Traspaso de IVA")]
[assembly: AssemblyDescription("Regionalización - Traspaso de iva entre polizas")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("VISUAL ERP SYSTEMS, S.A. DE C.V.")]
[assembly: AssemblyProduct("Traspaso de IVA")]
[assembly: AssemblyCopyright("Copyright ©  2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("c9346b4c-8dc7-4422-adcb-026574e3d276")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.1.2")]
[assembly: AssemblyFileVersion("2.0.1.2")]

/*
 * Ángel Jiménez
 * 1.0.0.2 Se configuro el calendario para mostrar siempre el ultimo dia del mes.
 * 
 * Ángel Jiménez
 * 1.0.0.3 Se modifico el codigo de traspaso de IVA para crear una sola póliza contable por movimiento.
 * Es decir una póliza de CXC y otra de CXP y no una por cada transferencia  que haga.
 * Las transferencias se almacenaran dentro de la poliza generada. Por tal motivo una poliza
 * contendra una o mas transferencias.
 * 
 * 
 * Ángel Jiménez
 * Fecha: 30/Oct/2013
 * 1.0.0.4 Quitar columna TC al exportar a excel y en la pantalla
 * 
 * Ángel Jiménez
 * Fecha: 15/Nov/2013
 * 1.0.0.5 Se realizaron cambios significativos a CXC y CXP. 
 * La tabla VMX_CONTOLPOLIZA_LINE ahora tiene dos nuevas columnas CHECK_ID y CUSTOMER_ID
 * para almacenar datos referentes a CXC.
 * 
 * Se elimino la columna CHECK_NO ya que no se utiliza.
 * 
 * Se cambiaron select e insert para soportar estas modificaciones.
 * Se corrigio la informacion en la base de datos.
 * 
 * Ángel Jiménez
 * Fecha: 20/Nov/2013
 * 1.0.0.6 Se realizaron modificaciones a las operaciones realizadas
 * con lo valores de los datatables. Anteriormente se utilizaban los indices 
 * ahora se toman los nombres de las columnas.
 * 
 * Ángel Jiménez
 * Fecha: 20/Nov/2013
 * 2.0.0.0 Se utilizo el login de la dll VKData
 * 
 * Ángel Jiménez
 * Fecha: 18/Dic/2013
 * 3.0.0.0 
 * 1.- Se utilizo la seguridad de CTECHSEGREG
 * 2.- Se utilizo la nueva ventana de Acercca de
 * 
 * Ángel Jiménez
 * 3.0.0.1
 * 27/Dic/2013
 * 
 * 1.- Se obtienen las fechas de periodos de la tabla ACCOUNT_PERIOD
 * 
 * 
 * Ángel Jiménez
 * 3.0.0.2
 * 31/Dic/2013
 * 
 * 1.- Mostrar los movimientos de una transaccion en el grid Procesados
 * 
 * Ángel Jiménez
 * 3.0.0.3
 * 31/Dic/2013
 *
 * 1.- Se agrego la descripcion de la cuenta destino en la tabla VMX_CONTOLPOLIZA_LINE en la columna Description
 * 2.- Se corrige el problema de eliminacion de poliza en Visual ERP tambien se elimina en  VMX_CONTOLPOLIZA_LINE y VMXCONTOLPOLIZA
 * 
 * Ángel Jiménez
 * 11/Feb/2014
 * 3.0.0.4
 * Ticket #369
 * 
 * En la pestaña de procesados aparecen registros que fueron traspasados. Se corrige este error modificando el metodo omititrProcesados()
 * el problema surge por la manera en que se eliminar los registros del grid.
 * 
 * 
 */
