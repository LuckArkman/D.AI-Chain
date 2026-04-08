using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Aethos.Node;
using Aethos.Presentation.RPC;
using Aethos.Core.Consensus;
using Aethos.Node.Health;
using Aethos.Presentation.RPC.Hubs;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Sprint 29: Injeta Serviços Base da L2 Aethos (Worker L2 Engine + EVM + RocksDB)
builder.Services.AddAethosNode("Data/RocksDb");

// Sprint 48: Camada de Observabilidade e Métricas (Prometheus)
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("Aethos.L2.Node"))
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddPrometheusExporter());

// Sprint 49: Health Checks e Swagger
builder.Services.AddHealthChecks()
    .AddCheck<RocksDbHealthCheck>("rocksdb");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Sprint 51: Seguranca de Administracao L2 (gRPC/SignalR JWT)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Aethos-Core-DAO",
            ValidAudience = "L2-Admin-Node",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AethosSoberana64bitEncryptionVaultKey!!!!"))
        };
    });

builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// Sprint 50: APIs interativas ao painel do Admin L2 (SignalR)
builder.Services.AddSignalR()
    .AddStackExchangeRedis("localhost:6379"); // Sincronização multi-nó via Redis (Sprint 43/50)

var app = builder.Build();

// Sprint 49: Pipeline de Diagnóstico e Documentação
app.MapHealthChecks("/health");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Sprint 48: Endpoint de raspagem de métricas para Grafana/Prometheus
app.UseOpenTelemetryPrometheusScrapingEndpoint();

// Sprint 51: Middlewares de Seguranca Soberana
app.UseAuthentication();
app.UseAuthorization();

// Sprint 29: Setup WebSockets Pipeline Pleno (MetaMask ws://)
app.UseAethosWebSockets();

// Sprint 29: Setup JSON-RPC Pipeline Pleno (MetaMask http://)
app.UseAethosJsonRpc();

// Sprint 29: Setup Servidor de Assinatura P2P Consenso
app.MapGrpcService<ConsensusService>();

// Sprint 50: Endpoints interativos SignalR
app.MapHub<NetworkHealthHub>("/hubs/health");
app.MapHub<TransactionFeedHub>("/hubs/transactions");

app.MapControllers();

await app.RunAsync();
