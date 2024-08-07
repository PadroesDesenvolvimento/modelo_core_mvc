# Gerar um novo par de chaves RSA
$rsa = New-Object System.Security.Cryptography.RSACryptoServiceProvider 2048

# Exportar a chave privada em formato XML
$privateKey = $rsa.ToXmlString($true)
Set-Content -Path "privateKey.xml" -Value $privateKey

# Exportar a chave pública em formato XML
$publicKey = $rsa.ToXmlString($false)
Set-Content -Path "publicKey.xml" -Value $publicKey

