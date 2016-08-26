using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Negocio.Geral
{
    public class UrlNfesEstados
    {
        Configuration Config;

        #region Enum Estados
        public enum Estado
        {
            AC,
            AL,
            AP,
            AM,
            BA,
            CE,
            DF,
            ES,
            GO,
            MA,
            MT,
            MS,
            MG,
            PA,
            PB,
            PR,
            PE,
            PI,
            RJ,
            RN,
            RS,
            RO,
            RR,
            SC,
            SP,
            SE,
            TO
        }
        #endregion

        #region Metodo de Retorno de Enum Estado
        public Estado Uf(int cUf)
        {
            switch (cUf)
            {
                case 11:
                    return Estado.RO;
                case 12:
                    return Estado.AC;
                case 13:
                    return Estado.AM;
                case 14:
                    return Estado.RR;
                case 15:
                    return Estado.PA;
                case 16:
                    return Estado.AM;
                case 17:
                    return Estado.TO;
                case 21:
                    return Estado.MA;
                case 22:
                    return Estado.PI;
                case 23:
                    return Estado.CE;
                case 24:
                    return Estado.RN;
                case 25:
                    return Estado.PA;
                case 26:
                    return Estado.PE;
                case 27:
                    return Estado.AL;
                case 28:
                    return Estado.SE;
                case 29:
                    return Estado.BA;
                case 31:
                    return Estado.MG;
                case 32:
                    return Estado.ES;
                case 33:
                    return Estado.RJ;
                case 35:
                    return Estado.SP;
                case 41:
                    return Estado.PR;
                case 42:
                    return Estado.SC;
                case 43:
                    return Estado.RS;
                case 50:
                    return Estado.MS;
                case 51:
                    return Estado.MT;
                case 52:
                    return Estado.GO;
                case 53:
                    return Estado.DF;
                default:
                    return Estado.CE;
            }
        }
        public string UfsCodigo(int cUf)
        {
            switch (cUf)
            {
                case 11:
                    return "RO";
                case 12:
                    return "AC";
                case 13:
                    return "AM";
                case 14:
                    return "RR";
                case 15:
                    return "PA";
                case 16:
                    return "AM";
                case 17:
                    return "TO";
                case 21:
                    return "MA";
                case 22:
                    return "PI";
                case 23:
                    return "CE";
                case 24:
                    return "RN";
                case 25:
                    return "PA";
                case 26:
                    return "PE";
                case 27:
                    return "AL";
                case 28:
                    return "SE";
                case 29:
                    return "BA";
                case 31:
                    return "MG";
                case 32:
                    return "ES";
                case 33:
                    return "RJ";
                case 35:
                    return "SP";
                case 41:
                    return "PR";
                case 42:
                    return "SC";
                case 43:
                    return "RS";
                case 50:
                    return "MS";
                case 51:
                    return "MT";
                case 52:
                    return "GO";
                case 53:
                    return "DF";
                default:
                    return "CE";
            }
        }
        #endregion
        public enum tbAmbiente
        {
            HOM,
            PROD
        }

        public enum TipoUrlEnvio
        {
            StatusServico,
            Inutilizacao,
            Cancelamento,
            NotaFiscal,
            Autorizacao,
            RetAutorizacao,
            RecepcaoEvento,
            ConsultaSitNfe

        }
        public string SetarUrlEstado(Estado UF, tbAmbiente Amb, TipoUrlEnvio tpUrlEnvio)
        {
            switch (UF)
            {
                case Estado.AL:
                    {
                        if (tpUrlEnvio == TipoUrlEnvio.StatusServico)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/NfeStatusServico/NfeStatusServico2.asmx";
                            else
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/NfeStatusServico/NfeStatusServico2.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Autorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                            {
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx";
                            }
                            else
                            {
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx";
                            }
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RetAutorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                            {
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao.asmx";
                            }
                            else
                            {
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao.asmx";
                            }
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Inutilizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao2.asmx";
                            else
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao2.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.ConsultaSitNfe)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx";
                            else
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Cancelamento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";
                            else
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RecepcaoEvento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";
                            else
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";

                        }
                        else
                            return string.Empty;
                    }
                case Estado.BA:
                    {
                        if (tpUrlEnvio == TipoUrlEnvio.StatusServico)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://hnfe.sefaz.ba.gov.br/webservices/NfeStatusServico/NfeStatusServico.asmx";
                            else
                                return "https://nfe.sefaz.ba.gov.br/webservices/NfeStatusServico/NfeStatusServico.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Inutilizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://hnfe.sefaz.ba.gov.br/webservices/nfenw/nfeinutilizacao2.asmx";
                            else
                                return "https://nfe.sefaz.ba.gov.br/webservices/NfeInutilizacao/NfeInutilizacao.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Autorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx";
                            else
                                return "https://nfe.sefaz.ba.gov.br/webservices/NfeAutorizacao/NfeAutorizacao.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RetAutorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao.asmx";
                            else
                                return "https://nfe.sefaz.ba.gov.br/webservices/NfeRetAutorizacao/NfeRetAutorizacao.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.ConsultaSitNfe)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfe.sefaz.ba.gov.br/webservices/NfeConsulta/NfeConsulta.asmx";
                            else
                                return "https://nfe.sefaz.ba.gov.br/webservices/NfeConsulta/NfeConsulta.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Cancelamento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx";
                            else
                                return "https://hnfe.sefaz.ba.gov.br/webservices/sre/recepcaoevento.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RecepcaoEvento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://hnfe.sefaz.ba.gov.br/webservices/sre/recepcaoevento.asmx";
                            else
                                return "https://nfe.sefaz.ba.gov.br/webservices/sre/recepcaoevento.asmx";

                        }
                        else
                            return string.Empty;
                    }
                case Estado.CE:
                    {
                        if (tpUrlEnvio == TipoUrlEnvio.StatusServico)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfeh.sefaz.ce.gov.br/nfe2/services/NfeStatusServico2?wsdl";
                            else
                                return "https://nfe.sefaz.ce.gov.br/nfe2/services/NfeStatusServico2?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Autorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfeh.sefaz.ce.gov.br/nfe2/services/NfeAutorizacao?wsdl";
                            else
                                return "https://nfe.sefaz.ce.gov.br/nfe2/services/NfeAutorizacao?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RetAutorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfeh.sefaz.ce.gov.br/nfe2/services/NfeRetAutorizacao?wsdl";
                            else
                                return "https://nfe.sefaz.ce.gov.br/nfe2/services/NfeRetAutorizacao?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Inutilizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfeh.sefaz.ce.gov.br/nfe2/services/NfeInutilizacao2?wsdl";
                            else
                                return "https://nfe.sefaz.ce.gov.br/nfe2/services/NfeInutilizacao2?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.ConsultaSitNfe)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfeh.sefaz.ce.gov.br/nfe2/services/NfeConsulta2?wsdl";
                            else
                                return "https://nfe.sefaz.ce.gov.br/nfe2/services/NfeConsulta2?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Cancelamento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfeh.sefaz.ce.gov.br/nfe2/services/RecepcaoEvento?wsdl";
                            else
                                return "https://nfe.sefaz.ce.gov.br/nfe2/services/RecepcaoEvento?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RecepcaoEvento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfeh.sefaz.ce.gov.br/nfe2/services/RecepcaoEvento?wsdl";
                            else
                                return "https://nfe.sefaz.ce.gov.br/nfe2/services/RecepcaoEvento?wsdl";
                        }
                        else
                            return string.Empty;
                    }
                case Estado.PA:
                    {
                        if (tpUrlEnvio == TipoUrlEnvio.StatusServico)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://hom.sefazvirtual.fazenda.gov.br/NfeStatusServico2/NfeStatusServico2.asmx";
                            else
                                return "https://www.sefazvirtual.fazenda.gov.br/NfeStatusServico2/NfeStatusServico2.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Autorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfeh.sefaz.ce.gov.br/nfe2/services/NfeAutorizacao?wsdl";
                            else
                                return "https://www.sefazvirtual.fazenda.gov.br/NfeAutorizacao/NfeAutorizacao.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RetAutorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfeh.sefaz.ce.gov.br/nfe2/services/NfeRetAutorizacao?wsdl";
                            else
                                return "https://www.sefazvirtual.fazenda.gov.br/NfeRetAutorizacao/NfeRetAutorizacao.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Inutilizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://hom.sefazvirtual.fazenda.gov.br/NfeInutilizacao2/NfeInutilizacao2.asmx";
                            else
                                return "https://www.sefazvirtual.fazenda.gov.br/NfeInutilizacao2/NfeInutilizacao2.asmx ";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.ConsultaSitNfe)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://hom.sefazvirtual.fazenda.gov.br/NfeConsulta2/NfeConsulta2.asmx";
                            else
                                return "https://www.sefazvirtual.fazenda.gov.br/NfeConsulta2/NfeConsulta2.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Cancelamento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://hom.sefazvirtual.fazenda.gov.br/RecepcaoEvento/RecepcaoEvento.asmx";
                            else
                                return "https://www.sefazvirtual.fazenda.gov.br/RecepcaoEvento/RecepcaoEvento.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RecepcaoEvento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://hom.sefazvirtual.fazenda.gov.br/RecepcaoEvento/RecepcaoEvento.asmx";
                            else
                                return "https://www.sefazvirtual.fazenda.gov.br/RecepcaoEvento/RecepcaoEvento.asmx";
                        }
                        else
                            return string.Empty;
                    }
                case Estado.PE:
                    {
                        if (tpUrlEnvio == TipoUrlEnvio.StatusServico)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NfeStatusServico2?wsdl";
                            else
                                return "https://nfe.sefaz.pe.gov.br/nfe-service/services/NfeStatusServico2?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Autorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfeh.sefaz.ce.gov.br/nfe2/services/NfeAutorizacao?wsdl";
                            else
                                return "https://nfe.sefaz.pe.gov.br/nfe-service/services/NfeAutorizacao?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RetAutorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfeh.sefaz.ce.gov.br/nfe2/services/NfeRetAutorizacao?wsdl";
                            else
                                return "https://nfe.sefaz.pe.gov.br/nfe-service/services/NfeRetAutorizacao?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Inutilizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NfeInutilizacao2?wsdl";
                            else
                                return "https://nfe.sefaz.pe.gov.br/nfe-service/services/NfeInutilizacao2?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.ConsultaSitNfe)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/NfeConsulta2?wsdl";
                            else
                                return "https://nfe.sefaz.pe.gov.br/nfe-service/services/NfeConsulta2?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Cancelamento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/RecepcaoEvento?wsdl";
                            else
                                return "https://nfe.sefaz.pe.gov.br/nfe-service/services/RecepcaoEvento?wsdl";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RecepcaoEvento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://nfehomolog.sefaz.pe.gov.br/nfe-service/services/RecepcaoEvento?wsdl";
                            else
                                return "https://nfe.sefaz.pe.gov.br/nfe-service/services/RecepcaoEvento?wsdl";
                        }
                        else
                            return string.Empty;
                    }
                case Estado.SE:
                    {
                        if (tpUrlEnvio == TipoUrlEnvio.StatusServico)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/NfeStatusServico/NfeStatusServico2.asmx";
                            else
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/NfeStatusServico/NfeStatusServico2.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Autorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                            {
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx";
                            }
                            else
                            {
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/NfeAutorizacao/NFeAutorizacao.asmx";
                            }
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RetAutorizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao.asmx";
                            else
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/NfeRetAutorizacao/NFeRetAutorizacao.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Inutilizacao)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao2.asmx";
                            else
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/nfeinutilizacao/nfeinutilizacao2.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.ConsultaSitNfe)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx";
                            else
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/NfeConsulta/NfeConsulta2.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.Cancelamento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";
                            else
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";
                        }
                        else if (tpUrlEnvio == TipoUrlEnvio.RecepcaoEvento)
                        {
                            if (Amb == tbAmbiente.HOM)
                                return "https://homologacao.nfe.sefazvirtual.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";
                            else
                                return "https://nfe.sefazvirtual.rs.gov.br/ws/recepcaoevento/recepcaoevento.asmx";
                        }
                        else
                            return string.Empty;
                    }
                default:
                    return string.Empty;
            }
        }
    }
}
