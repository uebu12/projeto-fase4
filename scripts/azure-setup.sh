#!/usr/bin/env bash
# Cria toda a infraestrutura Azure usada pelo deploy: 2 Web Apps (App Service
# para Containers, um para staging e um para produção) + 1 Azure SQL Server
# lógico com 2 bancos (um por ambiente). Equivalente ao que o assistente
# "Create Web App" do Portal Azure faz (aba Basics: Publish = Container,
# OS = Linux), só que roteirizado para não precisar repetir os cliques duas
# vezes (staging e produção) e para já deixar documentado o que foi criado.
#
# Pré-requisito: Azure CLI instalada e logada (`az login`).
# Uso:
#   chmod +x azure-setup.sh
#   ./azure-setup.sh

set -euo pipefail

RESOURCE_GROUP="rg-ecocoleta"
LOCATION="brazilsouth"
PLAN_NAME="plan-ecocoleta"
APP_STAGING="ecocoleta-staging"      # vira https://ecocoleta-staging.azurewebsites.net
APP_PRODUCTION="ecocoleta-producao"  # vira https://ecocoleta-producao.azurewebsites.net
SQL_SERVER="sql-ecocoleta"           # vira sql-ecocoleta.database.windows.net
SQL_ADMIN_USER="ecocoletaadmin"

read -rsp "Senha do admin do Azure SQL (min. 8 caracteres, maiúscula+minúscula+número+símbolo): " SQL_ADMIN_PASSWORD
echo
read -rsp "Chave JWT para assinar os tokens (string longa e aleatória, 32+ caracteres): " JWT_KEY
echo

echo "==> Criando resource group '$RESOURCE_GROUP'..."
az group create --name "$RESOURCE_GROUP" --location "$LOCATION" --output none

echo "==> Criando App Service Plan ($PLAN_NAME, Linux, B1)..."
az appservice plan create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$PLAN_NAME" \
  --is-linux \
  --sku B1 \
  --output none

echo "==> Criando Web App de staging ($APP_STAGING)..."
az webapp create \
  --resource-group "$RESOURCE_GROUP" \
  --plan "$PLAN_NAME" \
  --name "$APP_STAGING" \
  --deployment-container-image-name "mcr.microsoft.com/appsvc/staticsite:latest" \
  --output none

echo "==> Criando Web App de produção ($APP_PRODUCTION)..."
az webapp create \
  --resource-group "$RESOURCE_GROUP" \
  --plan "$PLAN_NAME" \
  --name "$APP_PRODUCTION" \
  --deployment-container-image-name "mcr.microsoft.com/appsvc/staticsite:latest" \
  --output none

echo "==> Expondo a porta 8080 nos dois Web Apps (porta usada pela imagem da API)..."
az webapp config appsettings set --resource-group "$RESOURCE_GROUP" --name "$APP_STAGING" \
  --settings WEBSITES_PORT=8080 --output none
az webapp config appsettings set --resource-group "$RESOURCE_GROUP" --name "$APP_PRODUCTION" \
  --settings WEBSITES_PORT=8080 --output none

echo "==> Criando o servidor lógico do Azure SQL ($SQL_SERVER)..."
az sql server create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$SQL_SERVER" \
  --admin-user "$SQL_ADMIN_USER" \
  --admin-password "$SQL_ADMIN_PASSWORD" \
  --output none

echo "==> Liberando o firewall do SQL Server para os Web Apps (serviços Azure)..."
az sql server firewall-rule create \
  --resource-group "$RESOURCE_GROUP" \
  --server "$SQL_SERVER" \
  --name "AllowAzureServices" \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0 \
  --output none

echo "==> Criando o banco de staging (tier Basic, o mais barato)..."
az sql db create \
  --resource-group "$RESOURCE_GROUP" \
  --server "$SQL_SERVER" \
  --name "EcoColetaDb-Staging" \
  --service-objective Basic \
  --output none

echo "==> Criando o banco de produção (tier Basic, o mais barato)..."
az sql db create \
  --resource-group "$RESOURCE_GROUP" \
  --server "$SQL_SERVER" \
  --name "EcoColetaDb-Producao" \
  --service-objective Basic \
  --output none

CONN_STRING_STAGING="Server=tcp:${SQL_SERVER}.database.windows.net,1433;Initial Catalog=EcoColetaDb-Staging;User ID=${SQL_ADMIN_USER};Password=${SQL_ADMIN_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
CONN_STRING_PRODUCTION="Server=tcp:${SQL_SERVER}.database.windows.net,1433;Initial Catalog=EcoColetaDb-Producao;User ID=${SQL_ADMIN_USER};Password=${SQL_ADMIN_PASSWORD};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

echo "==> Configurando variáveis de ambiente do Web App de staging..."
az webapp config appsettings set --resource-group "$RESOURCE_GROUP" --name "$APP_STAGING" \
  --settings \
    ASPNETCORE_ENVIRONMENT="Staging" \
    "ConnectionStrings__DatabaseConnection=${CONN_STRING_STAGING}" \
    Jwt__Issuer="EcoColetaApi" \
    "Jwt__Key=${JWT_KEY}" \
  --output none

echo "==> Configurando variáveis de ambiente do Web App de produção..."
az webapp config appsettings set --resource-group "$RESOURCE_GROUP" --name "$APP_PRODUCTION" \
  --settings \
    ASPNETCORE_ENVIRONMENT="Production" \
    "ConnectionStrings__DatabaseConnection=${CONN_STRING_PRODUCTION}" \
    Jwt__Issuer="EcoColetaApi" \
    "Jwt__Key=${JWT_KEY}" \
  --output none

echo
echo "==> Recursos criados com sucesso."
echo "    Staging:    https://${APP_STAGING}.azurewebsites.net"
echo "    Produção:   https://${APP_PRODUCTION}.azurewebsites.net"
echo
echo "Próximos passos (ver seção 'Deploy em Azure App Service' do README):"
echo "  1. Baixe o publish profile de cada Web App:"
echo "       az webapp deployment list-publishing-profiles --resource-group $RESOURCE_GROUP --name $APP_STAGING --xml"
echo "       az webapp deployment list-publishing-profiles --resource-group $RESOURCE_GROUP --name $APP_PRODUCTION --xml"
echo "  2. Cole o conteúdo de cada um como os Secrets do GitHub:"
echo "       AZURE_WEBAPP_PUBLISH_PROFILE_STAGING / AZURE_WEBAPP_PUBLISH_PROFILE_PRODUCTION"
echo "  3. Torne o pacote da imagem no GHCR público (GitHub -> seu perfil -> Packages ->"
echo "     ecocoleta-api -> Package settings -> Change visibility -> Public), para o"
echo "     Web App conseguir puxar a imagem sem precisar de credenciais extras."
echo
echo "IMPORTANTE (crédito de estudante): para não gastar o crédito à toa, pare o plano"
echo "(os dois Web Apps compartilham o mesmo App Service Plan B1) quando não precisar:"
echo "  az webapp stop --resource-group $RESOURCE_GROUP --name $APP_STAGING"
echo "  az webapp stop --resource-group $RESOURCE_GROUP --name $APP_PRODUCTION"
echo "Para apagar tudo ao final do semestre:"
echo "  az group delete --name $RESOURCE_GROUP --yes --no-wait"
