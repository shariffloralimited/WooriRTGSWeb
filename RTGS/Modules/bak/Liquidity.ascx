<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="true" CodeBehind="Liquidity.ascx.cs" Inherits="RTGS.modules.Liquidity" %>
<div class="row">
    <div class="col-md-12">
        <div class="table-responsive">
            <table Class="table  table-bordered table-striped table-hover margin-bottome-0">
                <tr>
                    <td width="30%">CBS Connectivity</td>
                    <td style="color:green" width="20%"><asp:Label id="lblCBS" runat="server" class="label label-danger" Text="OFF" />&nbsp;&nbsp;(<asp:Label ID="lblCBSTM" runat="server" Text="0" /> min)</td>
                    <td style="color:green"><asp:Label id="lblCBSService" runat="server"  class="label label-danger" Text="OFF" /></td>
                    <td width="35%">STP Connectivity</td>
                    <td style="color:green" width="15%"><asp:Label id="lblSTP" runat="server"  class="label label-danger" Text="OFF" /></td>
                </tr> 
                 <tr>
                    <td>RTSX Connectivity</td>
                    <td style="color:green"><asp:Label id="lblBB" runat="server"  class="label label-danger" Text="OFF" />&nbsp;&nbsp;(<asp:Label ID="lblBBTM" runat="server" Text="0" /> min)</td>
                    <td><a href='http://169.254.223.131/FloraLog/<% = System.DateTime.Today.ToString("yyyyMMdd") %>.log' target= "_blank">Flora Import/Export Service </a></td>
                    <td style="color:green"><asp:Label id="LblImporter" runat="server"  class="label label-danger" Text="OFF" /></td>
                </tr> 
                <tr>
                    <td><asp:LinkButton ID="LiquidityBtn" runat="server" OnClick="LiquidityBtn_Click">Liquidity Balance</asp:LinkButton></td>
                    <td>
                        <asp:DropDownList ID="CCYList" DataTextField="CCY" runat="server" Style="height: 15px; font-size: xx-small" AutoPostBack="true"></asp:DropDownList>
                    </td>
                    <td style="color:green;font-weight:bold" colspan="2">
                            <asp:Label id="lblLIQ" runat="server" Text="0.00" />
                            &nbsp;&nbsp;&nbsp;<asp:LinkButton ID="Camt03Btn" runat="server" Text="(camt.003)" OnClick="Camt03Btn_Click" />
                    </td>
                </tr>
                <tr>
                    <td>STP Error Folder</td>
                    <td><asp:Label ID="LblErrFileCnt" runat="server" /></td>
                    <td colspan="2"><asp:Label ID="LblErrFile" runat="server" /></td>
                </tr>                 
            </table>
        </div>
    </div>
</div>






















