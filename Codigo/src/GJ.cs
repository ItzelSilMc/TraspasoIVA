using CTECH.Acceso_a_Datos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMXTRASPIVA
{
    /// <summary>
    /// Stores the header of the policy journal .
    /// </summary>
    public class GJ : Journal
    {
        public string getNext_ID(string id_site, int add_Id = 0)
        {
            AdminGJ oAdmGJ = new AdminGJ();
            ID = oAdmGJ.getNext_GJ_ID(id_site,add_Id);
            return ID;
        }

        public string getMax_GJ_ID(string prefijoCfg, string id_site)
        {
            AdminGJ oAdmGJ = new AdminGJ();
            ID = oAdmGJ.getMax_GJ_ID(prefijoCfg,id_site);
            return ID;
        }

    }
}
