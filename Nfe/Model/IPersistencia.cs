using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Model
{
    interface IPersistencia<T>
    {
        bool Incluir(T ObjDados);
        bool Salvar(T ObjDados);
        bool Deletar(T ObjDados);
        DataTable Pesquisar();
        DataTable Pesquisar(int obj);
        DataTable Pesquisar(string obj);
    }
}
