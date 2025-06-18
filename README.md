# Microservices Project

A containerized application with:
- Angular frontend
- ASP.NET Core backend with Entity Framework Core
- PostgreSQL database
- Caddy as reverse proxy/ingress

## Architecture

```
Client Request → Caddy Reverse Proxy → Angular Frontend / ASP.NET Core API → PostgreSQL
```

## Services

- **Frontend**: Angular application served on port 4200 inside the container
- **Backend**: ASP.NET Core API with Entity Framework Core on port 5000
- **Database**: PostgreSQL 14 for data persistence
- **Caddy**: Reverse proxy handling routing and SSL
- **Docker**: Docker daemon service that exposes the Docker API on port 2375
- **Docker Client**: Service that can create and manage Docker containers on the host machine
- **Simulator**: Simulation service that connects to the Docker daemon

## Getting Started

### Prerequisites

- Docker and Docker Compose installed on your system

### Running the Application

1. Clone this repository
2. Navigate to the project directory
3. Run the application with Docker Compose:

```bash
docker compose up -d
```

4. Access the application at http://localhost

### Development

- Frontend code is in the `frontend` directory
- Backend code is in the `backend` directory
- Database configuration is in the `compose.yml` file
- Routing configuration is in the `Caddyfile`

## Using Docker Services

This project includes two Docker-related services:

1. **Docker Daemon Service**: Exposes the Docker API on port 2375, allowing other services to create and manage Docker containers.
2. **Docker Client Service**: Provides a command-line interface to interact with Docker.

### Using the Docker Client with Host Docker

The Docker client service allows you to create and manage Docker containers directly on your Windows host machine. This is achieved by mounting the Docker socket from your host into the Docker client container.

You can access the Docker client service to interact with your host's Docker daemon:

```bash
docker compose exec docker-client sh
```

Once inside the container, you can use standard Docker commands to create containers on your host machine:

```bash
# List running containers
docker ps

# Pull an image
docker pull nginx

# Run a container
docker run -d --name test-nginx nginx

# Stop a container
docker stop test-nginx
```

Any containers you create will be visible on your Windows host machine and can be managed using Docker commands from your Windows command prompt/PowerShell or Docker Desktop.

### Using the Docker Daemon Service

The Docker daemon service exposes the Docker API on port 2375, allowing other services (like the simulator service) to create and manage Docker containers within the Docker environment.

Services that need to connect to the Docker daemon service should include the following environment variable:

```yaml
environment:
  - DOCKER_HOST=tcp://docker:2375
```

### Security Considerations

- Mounting the Docker socket gives containers full access to your host's Docker daemon, which is a significant security risk. Only use this approach in trusted environments and with containers you control.
- The Docker daemon service exposes the Docker API without TLS encryption, which is also a security risk. In a production environment, you should enable TLS encryption for the Docker API.

## Stopping the Application

```bash
docker compose down
```

To remove volumes (database data and other persistent data):

```bash
docker compose down -v
```
