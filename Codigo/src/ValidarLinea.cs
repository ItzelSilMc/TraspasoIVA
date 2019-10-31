using System;
using System.Windows.Forms;

namespace VMXTRASPIVA
{
    internal class ValidarLinea
    {
        public bool resultado_bool = true;
        public string resultado_texto = "";

        /// <summary>
        /// Valida que la cuenta origen y destino no esten vacias
        /// </summary>
        /// <param name="cuenta_origen">cuenta origen</param>
        /// <param name="cuenta_destino">cuenta destino</param>
        /// <param name="pictureBox_Negativo">pictute box con imagen positivo</param>
        /// <param name="pictureBox_Positivo">picture box con imagen negativa</param>
        /// <returns>
        /// objeto con picture box positivo o negativo.
        /// </returns>
        public object validar_linear(string cuenta_origen, string cuenta_destino, PictureBox pictureBox_Negativo, PictureBox pictureBox_Positivo)
        {
            object regreso = pictureBox_Negativo.Image;
            cuenta_destino = cuenta_destino.Trim();
            cuenta_origen = cuenta_origen.Trim();

            if (cuenta_origen != "" && cuenta_destino != "")
                return pictureBox_Positivo.Image;

            this.resultado_texto = "La cuenta origen o destino esta vacia.";
            this.resultado_bool = false;
            return regreso;
        }
    }
}

