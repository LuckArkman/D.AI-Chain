FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar a Solução e Projetos
COPY ["AethosLedger.sln", "./"]
COPY ["Aethos.Node/Aethos.Node.csproj", "Aethos.Node/"]
COPY ["Aethos.Application/Aethos.Application.csproj", "Aethos.Application/"]
COPY ["Aethos.Contracts.L1/Aethos.Contracts.L1.csproj", "Aethos.Contracts.L1/"]
COPY ["Aethos.Core.AI.LSTM/Aethos.Core.AI.LSTM.csproj", "Aethos.Core.AI.LSTM/"]
COPY ["Aethos.Core.Consensus/Aethos.Core.Consensus.csproj", "Aethos.Core.Consensus/"]
COPY ["Aethos.Core.EVM/Aethos.Core.EVM.csproj", "Aethos.Core.EVM/"]
COPY ["Aethos.Core.Governance/Aethos.Core.Governance.csproj", "Aethos.Core.Governance/"]
COPY ["Aethos.Core.Persistence/Aethos.Core.Persistence.csproj", "Aethos.Core.Persistence/"]
COPY ["Aethos.Core.Network.RPC/Aethos.Core.Network.RPC.csproj", "Aethos.Core.Network.RPC/"]
COPY ["Aethos.Core.SmartContracts/Aethos.Core.SmartContracts.csproj", "Aethos.Core.SmartContracts/"]
COPY ["Aethos.Domain/Aethos.Domain.csproj", "Aethos.Domain/"]
COPY ["Aethos.Infrastructure.Bridge/Aethos.Infrastructure.Bridge.csproj", "Aethos.Infrastructure.Bridge/"]
COPY ["Aethos.Infrastructure.Cache/Aethos.Infrastructure.Cache.csproj", "Aethos.Infrastructure.Cache/"]
COPY ["Aethos.Math.FixedPoint/Aethos.Math.FixedPoint.csproj", "Aethos.Math.FixedPoint/"]
COPY ["Aethos.Presentation.Admin/Aethos.Presentation.Admin.csproj", "Aethos.Presentation.Admin/"]
COPY ["Aethos.Presentation.RPC/Aethos.Presentation.RPC.csproj", "Aethos.Presentation.RPC/"]

# Propriedades Globais
COPY ["Directory.Packages.props", "./"]

RUN dotnet restore "Aethos.Node/Aethos.Node.csproj"

# Copiar o resto do código C#
COPY . .
WORKDIR "/src/Aethos.Node"
RUN dotnet build "Aethos.Node.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aethos.Node.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Criar o Volume do RocksDB Ledger State
RUN mkdir /app/AethosStateData

# Exposição das Portas JSON-RPC e SignalR
EXPOSE 80 8080 5000

ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "Aethos.Node.dll"]
