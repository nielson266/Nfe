using Nfe.Entidade;
using Nfe.Model;
using Nfe.Negocio.Geral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.envConfRecebto
{
    public partial class TEnvEvento
    {

        public TEnvEvento()
        {

        }
        public TEnvEvento(Entidade_Manifestacao objManifestacao)
        {

            List<TEvento> ListEvento = new List<TEvento>();


            foreach (var item in objManifestacao.ListNfeManifestacao)
            {
                ListEvento.Add(new TEvento(item, objManifestacao.dtManifestacao));
            }
            this.idLote = objManifestacao.id.ToString();
            this.versao = "1.00";
            this.evento = ListEvento.ToArray();
        }
    }

    public partial class TEvento
    {
        public TEvento()
        {

        }
        public TEvento(Entidade_ItemManifestacao objitem,DateTime DtEnv)
        {
            this.versao = "1.00";
            this.infEvento = new TEventoInfEvento(1,DtEnv, objitem);
        }
    }
    public partial class TEventoInfEvento
    {
        public TEventoInfEvento()
        {

        }
        public TEventoInfEvento(int loja, DateTime DtEvento,Entidade_ItemManifestacao objitem)
        {

            var DtEmpresa = FuncoesGerais.Loja(loja);

            this.Id = "ID"+ objitem.codmanifestacao + objitem.chaveacesso + objitem.idseq.ToString().PadLeft(2, '0');
            this.chNFe = objitem.chaveacesso;
            this.dhEvento = string.Format("{0:s}", Convert.ToDateTime(DtEvento)) + "-03:00";
            this.cOrgao = TCOrgaoIBGE.Item91; //(TCOrgaoIBGE)NegocioFuncoesGerais.RetornoCodigoIbge(Convert.ToInt32(FuncoesGerais.UfIbgeEmpresa(loja)));
            if (FuncoesGerais.TipoAmbiente() == "PROD")
                this.tpAmb = TAmb.Item1;                        // 1- Produção | 2 - Homologação
            else
                this.tpAmb = TAmb.Item2;
            this.ItemElementName = ItemChoiceType.CNPJ;     // DA EMPRESA EMITENTE
            this.Item = DtEmpresa.Rows[0]["cnpj"].ToString();

            switch (objitem.codmanifestacao)
            {
                case 210200:
                    this.tpEvento = TEventoInfEventoTpEvento.Item210200;
                    break;
                case 210210:
                    this.tpEvento = TEventoInfEventoTpEvento.Item210210;
                    break;
                case 210220:
                    this.tpEvento = TEventoInfEventoTpEvento.Item210220;
                    break;
                case 210240:
                    this.tpEvento = TEventoInfEventoTpEvento.Item210240;
                    break;
            }

            this.nSeqEvento = objitem.idseq.ToString();
            this.verEvento = "1.00";
            this.detEvento = new TEventoInfEventoDetEvento(objitem);


        }
    }
    public partial class TEventoInfEventoDetEvento
    {
        public TEventoInfEventoDetEvento()
        {

        }
        public TEventoInfEventoDetEvento(Entidade_ItemManifestacao ObjItem)
        {
            this.versao = TEventoInfEventoDetEventoVersao.Item100;
            if(ObjItem.codmanifestacao == 210240)
            {
                this.xJust = "OPERAÇÃO NÃO REALIZADA DEVIDO AO UM PROBLEMA!";
            }
            switch (ObjItem.codmanifestacao)
            {
                case 210200:
                    this.descEvento = TEventoInfEventoDetEventoDescEvento.ConfirmacaodaOperacao;
                    break;
                case 210210:
                    this.descEvento = TEventoInfEventoDetEventoDescEvento.CienciadaOperacao;
                    break;
                case 210220:
                    this.descEvento = TEventoInfEventoDetEventoDescEvento.DesconhecimentodaOperacao;
                    break;
                case 210240:
                    this.descEvento = TEventoInfEventoDetEventoDescEvento.OperacaonaoRealizada;
                    break;
            }
        }
    }
}

