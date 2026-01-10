# FIAP.Games

API de gerenciamento de jogos desenvolvida como parte do projeto FIAP.

## 📋 Descrição

Microserviço responsável pelo gerenciamento de jogos, biblioteca de usuários, promoções e estatísticas do sistema.

## 🏗️ Arquitetura

Este projeto segue uma arquitetura em camadas:

- **Games.API**: Camada de apresentação (Controllers, Program.cs)
- **Games.Domain**: Camada de domínio (Entidades, Serviços, Interfaces)
- **Games.Infrastructure**: Camada de infraestrutura (Repositórios, Migrations, Banco de dados)
- **Games.Domain.Shared**: DTOs e contratos compartilhados entre serviços

## 🚀 Tecnologias

- .NET (C#)
- Entity Framework Core
- RabbitMQ (Message Bus)
- Prometheus (Métricas)
- Grafana (Visualização)
- Docker
- Kubernetes (K8s) na AWS

## ☸️ Infraestrutura

Esta aplicação é implantada em um **cluster Kubernetes (K8s) na AWS**, utilizando:

- ConfigMaps para configurações
- Secrets para informações sensíveis
- Deployments para orquestração de containers
- Services para descoberta e balanceamento de carga
- HPA (Horizontal Pod Autoscaler) para escalonamento automático
- Prometheus para coleta de métricas
- Grafana para visualização e dashboards de monitoramento
- RabbitMQ para comunicação assíncrona entre microserviços

Os arquivos de configuração do Kubernetes estão localizados na pasta `kubernetes/`.

## 📦 Build e Deploy

O projeto possui configuração de CI/CD através de GitHub Actions para deploy automático no Amazon ECR e Kubernetes.

## 🔧 Requisitos

- .NET SDK
- Docker (para containerização)
- Acesso ao cluster Kubernetes na AWS (para deploy)

## 📝 Licença

Este projeto faz parte do projeto acadêmico FIAP.
