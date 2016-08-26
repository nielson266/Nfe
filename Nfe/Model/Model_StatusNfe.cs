using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfe.Entidade;
using System.Data;

namespace Nfe.Model
{
    class Model_StatusNfe : IPersistencia<Entidade_Status>
    {
        public bool Incluir(Entidade_Status ObjDados)
        {
            return false;
        }
        public bool Salvar(Entidade_Status ObjDados)
        {
            BancoDados.InsertAlterarExcluir("INSERT INTO StatusServNFe(tpambiente,status, dtstatus,uf) values(" + ObjDados.tpAmbiente + "," + ObjDados.cStatus + ",'" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now) +"','" + ObjDados.Uf + "')");
            return true;

        }
        public bool Deletar(Entidade_Status ObjDados)
        {
            return false;
        }
        public DataTable Pesquisar()
        {
            return null;
        }
        public DataTable Pesquisar(int obj)
        {
            return null;
        }
        public DataTable Pesquisar(string obj)
        {
            return null;
        }
    }
}
