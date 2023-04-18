using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace Scire.NfeDownload.BLL
{
    public static class UtilBo
    {
        public static Int64 UltimoNSU(int vez)
        {
            try
            {
                string arquivo = Application.StartupPath + "\\RegistroNSU.xml";

                DataTable dtNUSU = new DataTable("UltimoNSU");
                dtNUSU.Columns.Add("nsu", System.Type.GetType("System.Int64"));
                dtNUSU.Columns.Add("datahora", System.Type.GetType("System.DateTime"));

                if (!System.IO.File.Exists(arquivo))
                {
                    DataRow row = dtNUSU.NewRow();
                    row["nsu"] = 1;
                    row["datahora"] = DateTime.Now;
                    dtNUSU.Rows.Add(row);
                    dtNUSU.WriteXml(arquivo);
                }

                dtNUSU.Rows.Clear();
                dtNUSU.ReadXml(arquivo);

                Int64 nsu = Int64.Parse(dtNUSU.Rows[0]["nsu"].ToString());

                return nsu;
            }
            catch
            {
                return 0;
            }
        }

        public static void AtualizarNSU(Int64 NSU, DateTime datahora)
        {
            try
            {
                string arquivo = Application.StartupPath + "\\RegistroNSU.xml";

                DataTable dtNUSU = new DataTable("UltimoNSU");
                dtNUSU.Columns.Add("nsu", System.Type.GetType("System.Int64"));
                dtNUSU.Columns.Add("datahora", System.Type.GetType("System.DateTime"));

                if (!System.IO.File.Exists(arquivo))
                {
                    DataRow row = dtNUSU.NewRow();
                    row["nsu"] = NSU;
                    row["datahora"] = DateTime.Now;
                    dtNUSU.Rows.Add(row);
                    dtNUSU.WriteXml(arquivo);
                }

                dtNUSU.Rows.Clear();
                dtNUSU.ReadXml(arquivo);

                dtNUSU.Rows[0]["nsu"] = NSU;
                dtNUSU.Rows[0]["datahora"] = datahora;
                dtNUSU.AcceptChanges();

                dtNUSU.WriteXml(arquivo);                
            }
            catch
            {
                return;
            }
        }
    }
}
