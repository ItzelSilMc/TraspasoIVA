using System;
using CTECH.Acceso_a_Datos;
using CTECH.Directorios;

namespace ConfiguracionSR
{
    public class historicoCambios
    {
        Microsoft_SQL_Server _SQL;
        LogErrores _ArchivoErrores;

        public historicoCambios(Microsoft_SQL_Server sql)
        {
            _SQL = sql;
            _ArchivoErrores = new LogErrores();
        }

        public void guardar(string aplicacion, string movimiento, string usuario)
        {
            try
            {
                //revisa si existe la tabla de cambios
                _SQL.CrearConexion();
                _SQL.AbrirConexion();

                string existe = "select COUNT(*) from sys.objects where type_desc = 'USER_TABLE' and name='VMX_SR_CAMBIOS'";
                if (_SQL.executeScalar(existe)=="0")
                {
                    //si no la crea
                    string createtable = "CREATE TABLE VMX_SR_CAMBIOS(Fecha DateTime, Usuario varchar(50), Aplicacion varchar(50), Movimiento varchar(100)) ";
                    _SQL.EjecutarDML(createtable);
                }

                //inserta el registro
                string insertar = string.Format("INSERT INTO VMX_SR_CAMBIOS(Fecha,Usuario,Aplicacion,Movimiento)VALUES(getdate(),'{0}','{1}','{2}')",usuario,aplicacion,movimiento);
                _SQL.EjecutarDML(insertar);

                _SQL.CerrarConexion();
                _SQL.DestruirConexion();

            }
            catch (Exception ex)
            {
                _ArchivoErrores.escribir("historicoCambios", "guardar", ex.InnerException + "--" + ex.Message);
                _SQL.DestruirConexion();
            }

        }

    }

}
