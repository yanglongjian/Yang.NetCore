# set output encoding
$OutputEncoding = [Text.UTF8Encoding]::UTF8

cd .\Yang.Web.Entry\

dotnet publish -c Release -o ..\..\publish\api --no-cache --no-restore



