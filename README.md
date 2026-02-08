# CarBidder - Microservices Auction Platform

A modern, scalable microservices-based auction bidding platform built with .NET and Next.js. The system is designed for high availability and scalability using containerized deployments and cloud-native architecture patterns.

## 🎯 Overview

CarBidder is a distributed microservices application that enables users to create, search, and bid on auctions in real-time. The architecture follows modern cloud-native principles with service-oriented design, asynchronous message processing, and comprehensive API gateway patterns.

### Key Features

- **Real-time Bidding**: Live auction updates using SignalR
- **Distributed Architecture**: Independent microservices with API Gateway
- **Event-Driven Communication**: Asynchronous messaging via RabbitMQ
- **Search Capability**: Elasticsearch-powered auction search
- **Identity & Authentication**: Secure user authentication and authorization
- **Notifications**: Real-time notifications for auction events
- **Cloud-Ready**: Kubernetes deployment configurations included
- **Docker Containerized**: All services containerized for consistency

## 🏗️ Architecture

### Microservices

| Service | Purpose | Tech Stack |
|---------|---------|-----------|
| **Auction Service** | Manages auction lifecycle | .NET, PostgreSQL, Entity Framework |
| **Bidding Service** | Handles bid placement and management | .NET, PostgreSQL |
| **Search Service** | Auction search and indexing | .NET, Elasticsearch |
| **Notification Service** | Sends auction notifications | .NET, SignalR |
| **Identity Service** | User authentication & authorization | .NET, IdentityServer |
| **Gateway Service** | API Gateway | .NET, API Gateway |
| **Web App** | Frontend application | Next.js, React, TypeScript |

### Infrastructure Components

- **PostgreSQL**: Primary relational database for auctions and bids
- **MongoDB**: Document database for flexible data storage
- **RabbitMQ**: Message broker for inter-service communication
- **Elasticsearch**: Search engine for auction indexing
- **Docker**: Containerization platform
- **Kubernetes**: Container orchestration (K8S manifests included)

## 🚀 Getting Started

### Prerequisites

- Docker & Docker Compose (v20.0+)
- .NET 8 SDK (for local development)
- Node.js 18+ (for frontend development)
- PostgreSQL client (optional, for database CLI access)

### Quick Start with Docker Compose

1. **Clone the repository:**
   ```bash
   git clone https://github.com/yourusername/carbidder.git
   cd carbidder
   ```

2. **Start all services:**
   ```bash
   docker-compose up -d
   ```

   This will spin up:
   - PostgreSQL (port 5433)
   - MongoDB (port 27017)
   - RabbitMQ (ports 5672, 15672)
   - All microservices
   - Frontend application

3. **Access the application:**
   - Web App: `http://localhost:3000`
   - RabbitMQ Management: `http://localhost:15672` (guest/guest)

4. **Stop services:**
   ```bash
   docker-compose down
   ```

### Development Setup

#### Backend Services

1. **Install .NET dependencies:**
   ```bash
   # Each service has its own project file
   cd src/AuctionService
   dotnet restore
   ```

2. **Configure appsettings.Development.json** with your connection strings

3. **Run a service:**
   ```bash
   dotnet run
   ```

#### Frontend

1. **Install dependencies:**
   ```bash
   cd frontend/web-app
   npm install
   ```

2. **Run development server:**
   ```bash
   npm run dev
   ```

3. **Build for production:**
   ```bash
   npm run build
   npm start
   ```

## 📁 Project Structure

```
CarBidder/
├── src/                              # Backend microservices
│   ├── AuctionService/              # Auction management
│   ├── BiddingService/              # Bidding operations
│   ├── SearchService/               # Search & indexing
│   ├── NotificationService/         # Event notifications
│   ├── IdentityService/             # Authentication
│   ├── GatewayService/              # API Gateway
│   └── Contracts/                   # Shared message contracts
├── frontend/
│   └── web-app/                     # Next.js frontend application
├── infrastructure/
│   ├── K8S/                         # Kubernetes manifests
│   │   ├── *-deployment.yaml       # Service deployments
│   │   ├── ingress.yaml            # Ingress configuration
│   │   └── config.yaml             # ConfigMaps
│   └── dev-k8s/                     # Development K8S configs
├── tests/                           # Test suites
│   ├── AuctionService.IntegrationTests/
│   ├── AuctionService.UnitTests/
│   ├── SearchService.IntegrationTests/
│   └── SearchService.UnitTests/
├── devcerts/                        # Development SSL certificates
├── docker-compose.yaml              # Local development stack
└── CarBidder.sln                    # Solution file
```

## 🔌 API Endpoints

### Gateway Service (Main Entry Point)
- Base URL: `http://localhost:6001`

### Direct Service Endpoints (Development)
- Auction Service: `http://localhost:7001`
- Bidding Service: `http://localhost:7002`
- Search Service: `http://localhost:7003`
- Notification Service: `http://localhost:7004`
- Identity Service: `http://localhost:7005`

## 📊 Database Schemas

### PostgreSQL
- **AuctionsDB**: Auction and bidding data
  - Auctions table
  - Bids table
  - User data
  - Audit logs

### MongoDB
- Document-based storage for flexible data
- Collections for supplementary data

## 🔐 Authentication & Authorization

The platform uses **IdentityServer** for OAuth2/OpenID Connect:

1. **User Registration**: Via Identity Service
2. **Token Issuance**: JWT tokens for API access
3. **Authorization**: Role-based access control (RBAC)
4. **Frontend Integration**: NextAuth.js for session management

**Default Credentials** (Development Only):
```
Username: user@example.com
Password: Check SeedData.cs or appsettings.Development.json
```

## 📬 Message Events (RabbitMQ)

The platform publishes and consumes these domain events:

- `AuctionCreated`: When a new auction is created
- `AuctionUpdated`: When auction details change
- `AuctionDeleted`: When an auction is deleted
- `AuctionFinished`: When bidding ends
- `BidPlaced`: When a user places a bid

These are defined in the `Contracts` project and consumed by respective services.

## 🐳 Docker Build & Push

The GitHub Actions workflow automatically builds and pushes images to Docker Hub:

```yaml
# Images are tagged as:
aleksandromilenkov/auction-svc:latest
aleksandromilenkov/search-svc:latest
aleksandromilenkov/bid-svc:latest
aleksandromilenkov/notify-svc:latest
aleksandromilenkov/identity-svc:latest
aleksandromilenkov/gateway-svc:latest
aleksandromilenkov/web-app:latest
```

### Manual Build

```bash
# Build a single service
docker build -f src/AuctionService/Dockerfile -t auction-svc:latest .

# Or use Docker Compose
docker-compose build [service-name]
```

## ☁️ Kubernetes Deployment

### Prerequisites for K8S

- Kubernetes cluster (EKS, GKE, AKS, or local Minikube)
- `kubectl` configured and authenticated
- Secret management setup for sensitive data

### Deploy to K8S

1. **Create secrets** (update with real values):
   ```bash
   kubectl create secret generic db-credentials \
     --from-literal=postgres-password=yourpassword \
     --from-literal=mongo-password=yourpassword
   ```

2. **Apply manifests**:
   ```bash
   kubectl apply -f infrastructure/K8S/
   ```

3. **Verify deployment**:
   ```bash
   kubectl get deployments
   kubectl get pods
   kubectl get svc
   ```

4. **Access via Ingress**:
   ```bash
   kubectl get ingress
   # Access via the ingress IP/domain configured in ingress.yaml
   ```

### AWS EKS Deployment

Update kubeconfig:
```bash
aws eks update-kubeconfig --name your-cluster-name --region us-east-1
```

Then apply K8S manifests as above.

## 🔄 CI/CD Pipeline

### GitHub Actions Workflow

The `.github/workflows/deployment.yaml` includes:

1. **Trigger Events**:
   - Manual workflow dispatch
   - Push to `main` or `master` branches
   - Pull requests

2. **Build Job** (`build-and-push`):
   - Checks for file changes in service paths
   - Builds Docker images only for modified services
   - Pushes to Docker Hub
   - Requires: `DOCKER_USERNAME`, `DOCKER_TOKEN` secrets

3. **Deploy Job** (`deploy-to-aws-eks`):
   - Configured as dummy/simulation job
   - Sets up AWS CLI and kubectl
   - Applies K8S manifests
   - Requires: `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY` secrets

### Setting Up GitHub Secrets

Add these to your repository settings:

- `DOCKER_USERNAME`: Docker Hub username
- `DOCKER_TOKEN`: Docker Hub access token
- `AWS_ACCESS_KEY_ID`: AWS credentials (for real deployments)
- `AWS_SECRET_ACCESS_KEY`: AWS credentials (for real deployments)

## 🧪 Testing

### Run Unit Tests

```bash
# Auction Service tests
dotnet test tests/AuctionService.UnitTests

# Search Service tests
dotnet test tests/SearchService.UnitTests
```

### Run Integration Tests

```bash
# Requires docker-compose running
dotnet test tests/AuctionService.IntegrationTests
dotnet test tests/SearchService.IntegrationTests
```

## 📝 Environment Configuration

Each service has `appsettings.{Environment}.json` files:

- `appsettings.json`: Base configuration
- `appsettings.Development.json`: Local development
- `appsettings.Docker.json`: Docker container environment
- `appsettings.Production.json`: Production settings

**Common Configuration Variables**:
```json
{
  "Logging": { "LogLevel": { "Default": "Information" } },
  "ConnectionStrings": {
    "DefaultConnection": "Host=postgres:5432;..."
  },
  "RabbitMq": {
    "Host": "rabbitmq",
    "Username": "guest",
    "Password": "guest"
  },
  "IdentityService": {
    "Authority": "http://identity-svc",
    "Audience": "auction-service"
  }
}
```

## 🛠️ Troubleshooting

### Common Issues

**Services not communicating:**
- Verify all services are running: `docker-compose ps`
- Check RabbitMQ is accessible: `http://localhost:15672`
- Review service logs: `docker-compose logs [service-name]`

**Database connection errors:**
- Ensure PostgreSQL is running and healthy
- Check connection string in appsettings
- Verify credentials match docker-compose.yaml

**Frontend can't connect to API:**
- Check Gateway Service is running
- Verify CORS settings in Gateway
- Check browser console for specific error messages

**Kubernetes pod failures:**
- Inspect pod logs: `kubectl logs [pod-name]`
- Describe pod: `kubectl describe pod [pod-name]`
- Check resource limits: `kubectl top pods`

## 📚 Documentation

- [API Documentation](docs/API.md) - Detailed API specifications
- [Architecture Decisions](docs/ADR.md) - Architecture decision records
- [Contributing Guidelines](CONTRIBUTING.md) - How to contribute

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit changes: `git commit -am 'Add new feature'`
4. Push to branch: `git push origin feature/your-feature`
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💼 Author

**Neil Cummings**

## 🙏 Acknowledgments

- Built with modern cloud-native technologies
- Follows microservices best practices
- Implements event-driven architecture patterns
- Scalable and production-ready design

## 📞 Support

For issues, questions, or suggestions:
- Open an [Issue](https://github.com/yourusername/carbidder/issues)
- Check existing [Discussions](https://github.com/yourusername/carbidder/discussions)
- Review [Documentation](docs/)

---

**Last Updated**: February 2026  
**Version**: 1.0.0
