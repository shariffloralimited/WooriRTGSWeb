﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RTGS.Entity
{
    public class FiToFiCustomerCredit
    {
        public string FiToFiCrdTransferID { get; set; }
        public string FromBICFI { get; set; }
        public string ToBICFI { get; set; }
        public string BizMessageIdr { get; set; }
        public string MsgDefIdr { get; set; }
        public string BizSvc { get; set; }
        public DateTime HeadCreationDtTime { get; set; }
        public string GrpHdrMsgId { get; set; }
        public DateTime CreationDtTime { get; set; }
        public int NoOfTrans { get; set; }
        public string SettelmentMethod { get; set; }
        public string PIInstructionId { get; set; }
        public string PIEndToEndId { get; set; }
        public string PITransactionId { get; set; }
        public string PTIClearingChannel { get; set; }
        public string PTIServiceLevelProprietary { get; set; }
        public string PTILocalInstrumentProprietary { get; set; }
        public string PTICategoryPurposeProprietary { get; set; }
        public string SettlementAmount { get; set; }
        public string SettlementCurrency { get; set; }
        public DateTime SettlementDate { get; set; }
        public string ChargeBearer { get; set; }
        public string InstructingAgentBICFI { get; set; }
        public string InstructingAgentName { get; set; }
        public string InstructingAgentBranchId { get; set; }
        public string InstructedAgentBICFI { get; set; }
        public string InstructedAgentName { get; set; }
        public string InstructedAgentBranchId { get; set; }
        public string DebtorName { get; set; }
        public string DebtorStreetName { get; set; }
        public string DebtorTownName { get; set; }
        public string DebtorCountry { get; set; }
        public string DebtorAddressLine { get; set; }
        public string DebtorAccountId { get; set; }
        public string DebtorAgentBICFI { get; set; }
        public string DebtorAgentName { get; set; }
        public string DebtorAgenId { get; set; }
        public string DebtorAgentAccountId { get; set; }
        public string DebtorAgentAccountProprietary { get; set; }
        public string CreditorAgentBICFI { get; set; }
        public string CreditorAgentName { get; set; }
        public string CreditorAgentId { get; set; }
        public string CreditorAgentAccountId { get; set; }
        public string CreditorAgentAccountProprietary { get; set; }
        public string CreditorName { get; set; }
        public string CreditorAddressLine { get; set; }
        public string CreditorAccountId { get; set; }
        public string CreditorAccountProprietary { get; set; }
        public string InstructionInformation { get; set; }
        public string Unstructured { get; set; }
        public int MakerID { get; set; }
        public string MakerName { get; set; }
        public int CheckerID { get; set; }
        public string CheckerName { get; set; }
        public int ReceivingBankID { get; set; }
        public int RoutingNo { get; set; }
        public string  PaymentReason { get; set; }
        public int SendingBranchID { get; set; }

    }
}