"""
Gera a documentação técnica (PDF) exigida na entrega do desafio DevOps.
Rodar de dentro de docs/:  python3 build_pdf.py
Saída: docs/EcoColeta_Documentacao_DevOps.pdf
"""
from reportlab.lib.pagesizes import A4
from reportlab.lib.units import mm
from reportlab.lib import colors
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.enums import TA_LEFT, TA_CENTER
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle,
    PageBreak, HRFlowable, Flowable
)
from reportlab.pdfgen import canvas as pdfcanvas

# ---------------------------------------------------------------- paleta ----
INK = colors.HexColor("#1b2b22")        # verde quase-preto, texto principal
GREEN = colors.HexColor("#2f6b4f")      # verde ESG, destaques
GREEN_SOFT = colors.HexColor("#e7f0ea") # fundo suave para tabelas/caixas
GREY = colors.HexColor("#5b6b63")       # texto secundário
LINE = colors.HexColor("#c7d6cd")       # linhas finas
CODE_BG = colors.HexColor("#f4f6f4")

PAGE_W, PAGE_H = A4
MARGIN = 20 * mm

styles = getSampleStyleSheet()

title_style = ParagraphStyle(
    "TitleCustom", parent=styles["Title"], fontName="Helvetica-Bold",
    fontSize=24, leading=28, textColor=INK, spaceAfter=4,
)
subtitle_style = ParagraphStyle(
    "SubtitleCustom", parent=styles["Normal"], fontName="Helvetica",
    fontSize=12.5, leading=17, textColor=GREY, spaceAfter=2,
)
h1 = ParagraphStyle(
    "H1Custom", parent=styles["Heading1"], fontName="Helvetica-Bold",
    fontSize=16, leading=20, textColor=INK, spaceBefore=0, spaceAfter=10,
)
h2 = ParagraphStyle(
    "H2Custom", parent=styles["Heading2"], fontName="Helvetica-Bold",
    fontSize=11.5, leading=15, textColor=GREEN, spaceBefore=12, spaceAfter=6,
)
body = ParagraphStyle(
    "BodyCustom", parent=styles["Normal"], fontName="Helvetica",
    fontSize=10, leading=14.5, textColor=INK, spaceAfter=6,
)
small = ParagraphStyle(
    "SmallCustom", parent=styles["Normal"], fontName="Helvetica",
    fontSize=8.5, leading=12, textColor=GREY,
)
code = ParagraphStyle(
    "CodeCustom", parent=styles["Normal"], fontName="Courier",
    fontSize=8.3, leading=12, textColor=INK, backColor=CODE_BG,
    borderPadding=(6, 8, 6, 8), spaceAfter=8,
)
bullet = ParagraphStyle(
    "BulletCustom", parent=body, leftIndent=12, bulletIndent=0, spaceAfter=4,
)
footer_style = ParagraphStyle(
    "FooterCustom", parent=styles["Normal"], fontName="Helvetica",
    fontSize=8, textColor=GREY,
)


def code_block(text: str):
    escaped = text.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
    escaped = escaped.replace("\n", "<br/>")
    return Paragraph(escaped, code)


cell_style = ParagraphStyle("Cell", parent=body, fontSize=8.7, leading=11.5, spaceAfter=0)
cell_header_style = ParagraphStyle("CellHeader", parent=cell_style, fontName="Helvetica-Bold", textColor=colors.white)


def section_table(rows, col_widths, header=True):
    wrapped = []
    for ridx, row in enumerate(rows):
        style = cell_header_style if (header and ridx == 0) else cell_style
        wrapped.append([Paragraph(str(cell), style) for cell in row])
    t = Table(wrapped, colWidths=col_widths, hAlign="LEFT")
    style = [
        ("FONTNAME", (0, 0), (-1, -1), "Helvetica"),
        ("FONTSIZE", (0, 0), (-1, -1), 9),
        ("TEXTCOLOR", (0, 0), (-1, -1), INK),
        ("VALIGN", (0, 0), (-1, -1), "TOP"),
        ("LEFTPADDING", (0, 0), (-1, -1), 7),
        ("RIGHTPADDING", (0, 0), (-1, -1), 7),
        ("TOPPADDING", (0, 0), (-1, -1), 5),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 5),
        ("LINEBELOW", (0, 0), (-1, -1), 0.5, LINE),
    ]
    if header:
        style += [
            ("BACKGROUND", (0, 0), (-1, 0), GREEN),
            ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
            ("FONTNAME", (0, 0), (-1, 0), "Helvetica-Bold"),
        ]
    t.setStyle(TableStyle(style))
    return t


class PlaceholderBox(Flowable):
    """Caixa tracejada para onde o print real deve ser colado."""
    def __init__(self, width, height, label):
        super().__init__()
        self.width = width
        self.height = height
        self.label = label

    def draw(self):
        c = self.canv
        c.saveState()
        c.setDash(3, 3)
        c.setStrokeColor(LINE)
        c.setLineWidth(1)
        c.setFillColor(colors.HexColor("#fbfcfb"))
        c.roundRect(0, 0, self.width, self.height, 4, stroke=1, fill=1)
        c.setDash()
        c.setFillColor(GREY)
        c.setFont("Helvetica-Oblique", 9)
        c.drawCentredString(self.width / 2, self.height / 2 - 3, self.label)
        c.restoreState()


def header_footer(c: pdfcanvas.Canvas, doc):
    c.saveState()
    c.setStrokeColor(LINE)
    c.setLineWidth(0.6)
    c.line(MARGIN, PAGE_H - 14 * mm, PAGE_W - MARGIN, PAGE_H - 14 * mm)
    c.setFont("Helvetica-Bold", 8.5)
    c.setFillColor(GREEN)
    c.drawString(MARGIN, PAGE_H - 12 * mm, "ECOCOLETA API")
    c.setFont("Helvetica", 8.5)
    c.setFillColor(GREY)
    c.drawRightString(PAGE_W - MARGIN, PAGE_H - 12 * mm, "DevOps · CI/CD · Containerização")

    c.line(MARGIN, 14 * mm, PAGE_W - MARGIN, 14 * mm)
    c.setFont("Helvetica", 8)
    c.setFillColor(GREY)
    c.drawString(MARGIN, 10 * mm, "FIAP · Fase 4 · 2TDS · Gabriel Furquim")
    c.drawRightString(PAGE_W - MARGIN, 10 * mm, f"{doc.page}")
    c.restoreState()


def cover_page_content():
    story = []
    story.append(Spacer(1, 55 * mm))
    story.append(Paragraph("EcoColeta API", title_style))
    story.append(Paragraph("Pipeline CI/CD, Containerização e Deploy Multiambiente", subtitle_style))
    story.append(Spacer(1, 4 * mm))
    story.append(HRFlowable(width="35%", thickness=1.4, color=GREEN, spaceAfter=10, hAlign="LEFT"))
    story.append(Paragraph(
        "Desafio “Navegando pelo mundo DevOps” &nbsp;—&nbsp; projeto ESG "
        "“Gestão de Resíduos e Reciclagem”, construído sobre a API da Fase 4.",
        body))
    story.append(Spacer(1, 30 * mm))
    meta_rows = [
        ["Integrante", "Gabriel Furquim"],
        ["Curso / Turma", "FIAP — 2TDS"],
        ["Projeto base", "EcoColeta API (ASP.NET Core 8 + SQL Server)"],
        ["Ferramenta de CI/CD", "GitHub Actions"],
        ["Orquestração (local)", "Docker Compose (API + SQL Server)"],
        ["Registro de imagem", "GitHub Container Registry (GHCR)"],
        ["Infraestrutura de deploy", "Azure App Service + Azure SQL Database (Azure for Students)"],
    ]
    t = Table(meta_rows, colWidths=[48 * mm, 110 * mm], hAlign="LEFT")
    t.setStyle(TableStyle([
        ("FONTNAME", (0, 0), (0, -1), "Helvetica-Bold"),
        ("FONTNAME", (1, 0), (1, -1), "Helvetica"),
        ("FONTSIZE", (0, 0), (-1, -1), 9.5),
        ("TEXTCOLOR", (0, 0), (0, -1), GREEN),
        ("TEXTCOLOR", (1, 0), (1, -1), INK),
        ("TOPPADDING", (0, 0), (-1, -1), 4),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 4),
        ("LINEBELOW", (0, 0), (-1, -2), 0.4, LINE),
    ]))
    story.append(t)
    return story


def pipeline_section():
    story = [Paragraph("1. Descrição do pipeline", h1)]
    story.append(Paragraph(
        "O pipeline é implementado em <b>GitHub Actions</b> "
        "(<font face='Courier'>.github/workflows/ci-cd.yml</font>) e roda em quatro jobs "
        "encadeados, cada um só avançando se o anterior terminar com sucesso.",
        body))

    rows = [
        ["Job", "Gatilho", "O que faz"],
        ["build-and-test", "push, pull request, tag ou manual",
         "dotnet restore -> build -> test (xUnit). Publica o .trx como artefato do run."],
        ["docker-build-push", "idem, exceto em pull request",
         "Builda a imagem a partir do Dockerfile e publica no GHCR com tags sha-<commit>, "
         "staging (push na main) e production + vX.Y.Z (tag de versão)."],
        ["deploy-staging", "push na branch main",
         "Aponta o Azure Web App ecocoleta-staging para a imagem :staging do GHCR "
         "(azure/webapps-deploy). Health check contra /swagger antes de concluir."],
        ["deploy-production", "push de uma tag vX.Y.Z",
         "Mesmo deploy, Web App ecocoleta-producao, imagem :production. Fica retido "
         "aguardando aprovação manual do Environment “production” antes de rodar."],
    ]
    story.append(Spacer(1, 2))
    story.append(section_table(rows, col_widths=[32 * mm, 30 * mm, 106 * mm]))

    story.append(Paragraph("Por que staging é automático e produção não", h2))
    story.append(Paragraph(
        "Os dois últimos jobs usam os <b>Environments</b> do GitHub "
        "(Settings -> Environments). O Environment <font face='Courier'>production</font> "
        "tem um “Required reviewer” configurado — o pipeline chega até ele, "
        "mostra o deploy pendente na aba Actions, e só executa depois de um clique de "
        "aprovação manual. O Environment <font face='Courier'>staging</font> não tem essa "
        "trava, permitindo deploy contínuo a cada push na main.",
        body))

    story.append(Paragraph("Para onde os deploys apontam: Azure App Service", h2))
    story.append(Paragraph(
        "Os jobs <font face='Courier'>deploy-staging</font> e "
        "<font face='Courier'>deploy-production</font> rodam em runner hospedado "
        "(<font face='Courier'>ubuntu-latest</font>) e usam a action "
        "<font face='Courier'>azure/webapps-deploy</font> para apontar cada "
        "<b>Azure Web App</b> (App Service para Containers) para a imagem correspondente no "
        "GHCR, autenticando com o publish profile do Web App (Secret do repositório). Não há "
        "servidor ou VM próprios para manter — o Azure é quem executa o container.",
        body))
    story.append(Paragraph(
        "Staging e produção são dois Web Apps separados "
        "(<font face='Courier'>ecocoleta-staging</font> / "
        "<font face='Courier'>ecocoleta-producao</font>), cada um com seu próprio "
        "<b>Azure SQL Database</b> — mesmo provider EF Core já usado no projeto "
        "(<font face='Courier'>Microsoft.EntityFrameworkCore.SqlServer</font>), sem alterar "
        "código. Os recursos são criados uma única vez com "
        "<font face='Courier'>scripts/azure-setup.sh</font>, usando o crédito da conta "
        "<b>Azure for Students</b>.",
        body))
    return story


def docker_section():
    story = [Paragraph("2. Docker: arquitetura", h1)]
    story.append(Paragraph(
        "A API roda em container, conectada a um SQL Server também em container — "
        "substituindo o SQL Server LocalDB usado na Fase 4, que é específico do Windows e "
        "não existe dentro de um container Linux. Essa dupla (API + SQL Server) via "
        "Docker Compose é o ambiente local/de desenvolvimento; em staging e produção na "
        "nuvem, a mesma imagem da API roda no Azure App Service, conectada a um Azure SQL "
        "Database (ver seção 1).",
        body))

    story.append(Paragraph("Dockerfile — build multi-stage", h2))
    story.append(Paragraph(
        "Três estágios: <b>build</b> (SDK .NET 8, restaura e compila), <b>publish</b> "
        "(gera o artefato de publicação) e <b>final</b> (runtime ASP.NET, bem mais leve que "
        "o SDK). Só o resultado do publish entra na imagem final.",
        body))
    story.append(code_block(
        "FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build\n"
        "WORKDIR /src\n"
        'COPY ["Fiap.EcoColeta.Api.csproj", "./"]\n'
        'RUN dotnet restore "Fiap.EcoColeta.Api.csproj"\n'
        "COPY . .\n"
        'RUN dotnet build "Fiap.EcoColeta.Api.csproj" -c Release -o /app/build\n\n'
        "FROM build AS publish\n"
        'RUN dotnet publish "Fiap.EcoColeta.Api.csproj" -c Release -o /app/publish '
        "/p:UseAppHost=false\n\n"
        "FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final\n"
        "WORKDIR /app\n"
        "EXPOSE 8080\n"
        "COPY --from=publish /app/publish .\n"
        'ENTRYPOINT ["dotnet", "Fiap.EcoColeta.Api.dll"]'
    ))

    story.append(Paragraph("Orquestração — docker-compose.yml + overlays", h2))
    story.append(Paragraph(
        "Um arquivo base define o que é comum aos dois ambientes (serviços "
        "<font face='Courier'>api</font> e <font face='Courier'>db</font>, rede, variáveis "
        "parametrizadas, health check do banco). Dois overlays definem só o que muda: nome "
        "dos containers, porta publicada e volume do banco.",
        body))
    arch_rows = [
        ["Arquivo", "Papel"],
        ["docker-compose.yml", "Serviços api + db, rede ecocoleta-net, variáveis via ${...}, health check do SQL Server (sqlcmd)"],
        ["docker-compose.staging.yml", "Container *-staging, porta 8081:8080, volume ecocoleta-db-staging-data"],
        ["docker-compose.production.yml", "Container *-production, porta 8080:8080, volume ecocoleta-db-production-data"],
        [".env.staging / .env.production", "SA_PASSWORD, ASPNETCORE_ENVIRONMENT, JWT_ISSUER, JWT_KEY — nunca versionados"],
    ]
    story.append(section_table(arch_rows, col_widths=[55 * mm, 113 * mm]))

    story.append(Paragraph("Comandos usados", h2))
    story.append(code_block(
        "# build + validação da imagem\n"
        "docker build -t ecocoleta-api -f Fiap.EcoColeta.Api/Dockerfile Fiap.EcoColeta.Api\n\n"
        "# subir staging (porta 8081)\n"
        "docker compose -f docker-compose.yml -f docker-compose.staging.yml \\\n"
        "  --env-file .env.staging up -d --build\n\n"
        "# subir produção (porta 8080)\n"
        "docker compose -f docker-compose.yml -f docker-compose.production.yml \\\n"
        "  --env-file .env.production up -d --build"
    ))
    return story


def desafios_section():
    story = [Paragraph("3. Desafios encontrados e como foram resolvidos", h1)]
    items = [
        ("LocalDB não existe em container Linux",
         "A Fase 4 rodava contra SQL Server LocalDB (processo nativo do Windows). "
         "Resolvido trocando o destino, nos ambientes de staging/produção, para um "
         "container oficial do SQL Server 2022, com a connection string injetada por "
         "variável de ambiente (ConnectionStrings__DatabaseConnection) — sem alterar o "
         "fluxo local em Development, que continua usando LocalDB normalmente."),
        ("Condição de corrida na subida do banco",
         "Mesmo com o health check do Compose (service_healthy), a API podia tentar "
         "conectar no instante exato em que o SQL Server ainda estava finalizando a "
         "inicialização interna. Resolvido adicionando, no Program.cs, uma rotina de "
         "retry (até 8 tentativas, com espera crescente) em torno da aplicação das "
         "migrações no startup."),
        ("Isolar staging de produção na mesma máquina",
         "Resolvido com overlays de Compose separados: cada ambiente tem seu próprio "
         "nome de container, porta publicada e volume nomeado do banco, então os dois "
         "podem rodar ao mesmo tempo sem conflito nem risco de staging sobrescrever dados "
         "de produção."),
        ("Separar o que exige aprovação do que não exige",
         "Resolvido usando os Environments do GitHub: production tem um required "
         "reviewer configurado, então o pipeline fica retido até alguém aprovar "
         "manualmente; staging não tem essa trava e recebe deploy contínuo."),
        ("Onde rodar o deploy sem manter servidor próprio",
         "Deploy automatizado em dois ambientes pedia um destino real na nuvem, não só o "
         "docker-compose.yml local. Resolvido usando Azure App Service (Web App for "
         "Containers): o Azure passa a executar a imagem publicada no GHCR diretamente, sem "
         "precisar provisionar ou manter VM, usando o crédito da conta Azure for Students."),
        ("Trocar o banco sem reescrever a camada de dados",
         "Staging/produção precisavam de um banco real na nuvem, não o SQL Server LocalDB da "
         "Fase 4. Resolvido com Azure SQL Database — compatível com o mesmo provider "
         "Microsoft.EntityFrameworkCore.SqlServer já usado no projeto, então a troca foi só "
         "de connection string (variável de ambiente no Web App), sem alterar Models, "
         "Repository ou migrações."),
    ]
    for titulo, texto in items:
        story.append(Paragraph(titulo, h2))
        story.append(Paragraph(texto, body))
    return story


def prints_section():
    story = [Paragraph("4. Prints do pipeline e dos ambientes", h1)]
    story.append(Paragraph(
        "Espaços reservados abaixo — substituir por capturas reais após rodar o pipeline "
        "de ponta a ponta (ver checklist ao final).",
        body))

    captions = [
        "Pipeline no GitHub Actions — jobs build-and-test e docker-build-push concluídos",
        "Imagem publicada no GHCR, visibilidade Public (aba Packages do repositório)",
        "Job deploy-staging concluído + Web App ecocoleta-staging no Portal Azure mostrando a imagem :staging",
        "Swagger respondendo em https://ecocoleta-staging.azurewebsites.net/swagger (staging)",
        "Aprovação manual do Environment production + job deploy-production concluído",
        "Swagger respondendo em https://ecocoleta-producao.azurewebsites.net/swagger (produção)",
    ]
    for cap in captions:
        story.append(PlaceholderBox(width=173 * mm, height=42 * mm, label="[ print ]"))
        story.append(Paragraph(cap, small))
        story.append(Spacer(1, 7))
    return story


def checklist_section():
    story = [Paragraph("5. Checklist de entrega", h1)]
    rows = [
        ["Item", "OK"],
        ["Projeto compactado em .ZIP com estrutura organizada", "[ ]"],
        ["Dockerfile funcional", "OK"],
        ["docker-compose.yml ou arquivos Kubernetes", "OK"],
        ["Pipeline com etapas de build, teste e deploy", "OK"],
        ["README.md com instruções e prints", "[ ] prints pendentes"],
        ["Documentação técnica com evidências (PDF ou PPT)", "[ ] prints pendentes"],
        ["Deploy realizado nos ambientes staging e produção", "[ ] rodar o pipeline"],
    ]
    story.append(section_table(rows, col_widths=[128 * mm, 40 * mm]))
    return story


def build():
    doc = SimpleDocTemplate(
        "EcoColeta_Documentacao_DevOps.pdf",
        pagesize=A4,
        leftMargin=MARGIN, rightMargin=MARGIN,
        topMargin=22 * mm, bottomMargin=18 * mm,
        title="EcoColeta API - Documentacao DevOps",
        author="Gabriel Furquim",
    )

    story = []
    story += cover_page_content()
    story.append(PageBreak())
    story += pipeline_section()
    story.append(PageBreak())
    story += docker_section()
    story.append(PageBreak())
    story += desafios_section()
    story.append(PageBreak())
    story += prints_section()
    story.append(PageBreak())
    story += checklist_section()

    doc.build(story, onFirstPage=header_footer, onLaterPages=header_footer)


if __name__ == "__main__":
    build()
    print("OK: EcoColeta_Documentacao_DevOps.pdf gerado")
