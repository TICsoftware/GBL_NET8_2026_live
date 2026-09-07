<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:s="http://www.sitemaps.org/schemas/sitemap/0.9"
    exclude-result-prefixes="s">

	<xsl:output method="html" encoding="UTF-8" indent="yes"/>

	<!-- Template for the whole document -->
	<xsl:template match="/">
		<html>
			<head>
				<title>Sitemap</title>
				<style>
					body { font-family: Arial, sans-serif; margin: 20px; }
					h1 { color: #333; }
					table { border-collapse: collapse; width: 100%; }
					th, td { border: 1px solid #ccc; padding: 8px; text-align: left; }
					th { background-color: #f2f2f2; }
					a { color: #0078d4; text-decoration: none; }
					a:hover { text-decoration: underline; }
				</style>
			</head>
			<body>
				<h1>Website Sitemap</h1>
				<table>
					<thead>
						<tr>
							<th>URL</th>
							<th>Last Modified</th>
							<th>Change Frequency</th>
							<th>Priority</th>
						</tr>
					</thead>
					<tbody>
						<!-- NOTE the s: prefix everywhere -->
						<xsl:for-each select="s:urlset/s:url">
							<tr>
								<td>
									<a href="{s:loc}">
										<xsl:value-of select="s:loc"/>
									</a>
								</td>
								<td>
									<xsl:value-of select="s:lastmod"/>
								</td>
								<td>
									<xsl:value-of select="s:changefreq"/>
								</td>
								<td>
									<xsl:value-of select="s:priority"/>
								</td>
							</tr>
						</xsl:for-each>
					</tbody>
				</table>
			</body>
		</html>
	</xsl:template>

</xsl:stylesheet>
