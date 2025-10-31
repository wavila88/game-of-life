# Game of Life Project

This project implements Conway's Game of Life with a frontend in React, backend in .NET using Hexagonal Architecture, and Redis as the database. Infrastructure as Code (IaC) is used to deploy the solution on a Kubernetes service in Minikube, supporting horizontal scaling.

## Technologies

- **Frontend:** React, organized using **Feature-Based Architectur**.
- **Backend:** .NET, implemented with **Hexagonal Architecture** for maintainability and scalability.
- **Database:** Redis.
- **IaC:** Terraform for infrastructure deployment.

## Features

- API documentation included.
- WebSocket implementation for continuous communication between frontend and backend.
- Unit tests for core logic and cycle detection.

## Deployment

- Infrastructure is provisioned using Terraform.
- Services run in containers and are orchestrated with Kubernetes (Minikube).
- Horizontal Pod Autoscaler (HPA) is configured for backend scalability.

---

Feel free to ask for more details or custom sections!