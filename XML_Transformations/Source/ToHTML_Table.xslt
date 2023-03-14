<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>

    <xsl:template match="/">
		<html>
			<head>
				<meta charset="utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1"/>
				<style>
table, th, td {
  border:1px solid black;
}
</style>
			
			</head>
		
			<body>
				<table>
					<tr>
						<th colspan="2">CPUs</th>
					</tr>
					<xsl:apply-templates select="/hardware/CPUs"/>
				<tr>
						<th colspan="2">GCards</th>
					</tr>
					<xsl:apply-templates select="/hardware/GCards"/>
				<tr>
						<th colspan="2">SSDs</th>
					</tr>
					<xsl:apply-templates select="/hardware/SSDs"/>
				</table>
			</body>
		</html>
		
    </xsl:template>

	<!--<xsl:template match="CPUs">
		<xsl:for-each select="//CPUs/CPU">
			<xsl:value-of select="./name"/>
			<xsl:text>&#10;</xsl:text>
		</xsl:for-each>
	</xsl:template>-->


	<xsl:template match="CPUs">
		<xsl:for-each select="CPU">
			<tr>
				<td><xsl:value-of select="name" /></td>
				<td><xsl:value-of select="price" /></td>
			</tr>
		</xsl:for-each>
	</xsl:template>
<xsl:template match="GCards">
		<xsl:for-each select="Graphics_Card">
			<tr>
				<td><xsl:value-of select="name" /></td>
				<td><xsl:value-of select="price" /></td>
			</tr>
		</xsl:for-each>
	</xsl:template>
<xsl:template match="SSDs">
		<xsl:for-each select="SSD">
			<tr>
				<td><xsl:value-of select="name" /></td>
				<td><xsl:value-of select="price" /></td>
			</tr>
		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>

