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

## Stopping the Application

```bash
docker compose down
```

To remove volumes (database data):

```bash
docker compose down -v
```
