﻿<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" 	xmlns:k="urn:swift:saa:xsd:saa.2.0"
	xmlns:m="urn:iso:std:iso:20022:tech:xsd:head.001.001.01" xmlns:n="urn:iso:std:iso:20022:tech:xsd:camt.053.001.04" >
<xsl:template match="/">
  <html>
  <head>
  <link rel="stylesheet" type="text/css" href="content/sitec.css"/>
  </head>
  <body>
  <br />
  <Div class="Head">Final Settlement Report (camt.053)</Div>
  <br />
  
  <table class="LightBorderTable">
	<xsl:for-each select="//m:AppHdr">
    <tr>
	  <td class="NormalBold" width="150">Report Creation Date</td>
      <td class="Normal" width="250"><xsl:value-of select="m:CreDt" /></td>
	</tr>
    </xsl:for-each>
	<xsl:for-each select="//m:AppHdr">
    <tr>
	<td class="NormalBold" width="150">BizMsgIdr</td>	
	<td class="Normal" width="250"><xsl:value-of select="m:BizMsgIdr"/></td>
	</tr>
    </xsl:for-each>
  </table>
  <br/><span class="NormalBold">Balance</span><br/>
  <table class="LightBorderTable">
	<xsl:for-each select="//n:Stmt/n:Bal">
    <tr>
      <td class="Normal" bgcolor="aliceblue"><xsl:value-of select="n:Tp/n:CdOrPrtry/n:Cd"/></td>
	  <td class="Normal" bgcolor="aliceblue"><xsl:value-of select="n:CdtDbtInd"/></td>
      <td class="Normal" bgcolor="aliceblue"><xsl:value-of select="n:Amt"/></td>
	</tr>
    </xsl:for-each>
  </table>
  <br/><span class="NormalBold">Entries</span><br/>
  <table  class="LightBorderTable">
    <tr bgcolor="aliceblue">
      <th class="NormalBold" width="80">Ref</th>
      <th class="NormalBold" width="150">DB/CD</th>
      <th class="NormalBold" width="80">Amount</th>
      <th class="NormalBold" width="150">Status</th>
      <th class="NormalBold" width="150">InstrID</th>
      <th class="NormalBold" width="150">End ID</th>
      <th class="NormalBold" width="150">TX ID</th>
   </tr>
    <xsl:for-each select="//n:Ntry">
    <tr>
      <td class="Normal" align="center"><xsl:value-of select="n:NtryRef"/></td>
      <td class="Normal" align="center"><xsl:value-of select="n:CdtDbtInd"/></td>
      <td class="Normal" align="center"><xsl:value-of select="n:Amt"/></td>
      <td class="Normal" align="center"><xsl:value-of select="n:Sts"/></td>
      <td class="Normal" align="center"><xsl:value-of select="n:NtryDtls//n:InstrId"/></td>
      <td class="Normal" align="center"><xsl:value-of select="n:NtryDtls//n:EndToEndId"/></td> 
      <td class="Normal" align="center"><xsl:value-of select="n:NtryDtls//n:TxId"/></td>
	</tr>
    </xsl:for-each>
  </table>
  </body>
  </html>
</xsl:template>

</xsl:stylesheet>