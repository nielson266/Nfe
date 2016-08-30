using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Model
{
    public class Model_LogNfe
    {
        public void InsertErroLog(string DescErro)
        {
            BancoDados.InsertAlterarExcluir(" INSERT INTO LogNfe (DtOcor, TxOcor) \n" +
                                            " VALUES     (now()::timestamp,'" + DescErro +"')");
        }
    }
}
