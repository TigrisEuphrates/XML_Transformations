﻿<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
	<xsl:output method="xml" indent="yes"/>

	<xsl:template match="@* | node()">
		<xsl:apply-templates select="/*"/>
	</xsl:template>

	<xsl:template match="/*">
		
		<xsl:for-each select="./*">
			<xsl:text>&#xa;		</xsl:text>
			<xsl:value-of select="name()" />
			
			<xsl:for-each select="./*">
				<xsl:text>&#xa;			</xsl:text>
				<xsl:value-of select="name()" />

				<xsl:for-each select="./*">
					<xsl:text>&#xa;				</xsl:text>
					<xsl:value-of select="." />
				</xsl:for-each>
				
			</xsl:for-each>

		</xsl:for-each>
	</xsl:template>
</xsl:stylesheet>
