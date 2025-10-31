# Terraform provider configuration and version
terraform {
  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.29"
    }
  }
}

# Configuration of the Kubernetes provider
provider "kubernetes" {
  config_path = pathexpand("~/.kube/config")
}


# --- Namespace ---
resource "kubernetes_namespace" "app_ns" {
  metadata {
    # Namespace Name: back-game-app
    name = "back-game-app"
  }
}
# --- Namespace for front---
resource "kubernetes_namespace" "frontend" {
  metadata {
    name = "front-app"
  }
}

# --- Deployment ---
resource "kubernetes_deployment" "app" {
  depends_on = [kubernetes_namespace.app_ns]

  metadata {
    name      = "back-deployment"
    namespace = kubernetes_namespace.app_ns.metadata[0].name
    labels = {
      app = "back"
    }
  }

  spec {
    # Initial number of replicas
    replicas = 2 

    selector {
      match_labels = {
        app = "back"
      }
    }

    template {
      metadata {
        labels = {
          app = "back"
        }
      }

      spec {
        container {
          name  = "back-container"
          # CRITICAL: Tells Kubernetes to only use the local image (from Minikube's internal registry)
          image_pull_policy = "IfNotPresent" 
           # Custom image for your backend
          image = "back-game-of-life/backend:v3.0" 
          port {
            container_port = 80
          }
          # Requirements for HPA to function properly
          resources { 
            requests = {
              cpu    = "100m"
              memory = "128Mi"
            }
            limits = {
              cpu    = "250m"
              memory = "256Mi"
            }
          }
        }
      }
    }
  }
}

# --- Service (NodePort) ---
resource "kubernetes_service" "app_svc" {
  metadata {
    # Service Name: back-service
    name      = "back-service" 
    namespace = kubernetes_namespace.app_ns.metadata[0].name
  }

  spec {
    selector = {
      # Selector: back
      app = "back"
    }
    port {
      port        = 80
      target_port = 80
    }
    # NodePort is a must for accessing the service externally in Minikube
    type = "NodePort" 
  }
}

# --- Horizontal Pod Autosacling (HPA) ---
resource "kubernetes_horizontal_pod_autoscaler_v2" "app_hpa" {
  metadata {
    # HPA Name: back-hpa
    name      = "back-hpa"
    namespace = kubernetes_namespace.app_ns.metadata[0].name
  }

  spec {
    scale_target_ref {
      api_version = "apps/v1"
      kind        = "Deployment"
      name        = kubernetes_deployment.app.metadata[0].name
    }

    min_replicas = 2
    max_replicas = 5

    metric {
      type = "Resource"

      resource {
        name = "cpu"

        target {
          type              = "Utilization"
          # Scale if average CPU usage exceeds 50% of the requested amount
          average_utilization = 50 
        }
      }
    }
  }
}

# --- Output to get access URL ---
output "backend_service_access_command" {
  description = "Run this command to get the access URL for your back-service in Minikube"
  value       = "minikube service ${kubernetes_service.app_svc.metadata[0].name} --url --namespace=${kubernetes_namespace.app_ns.metadata[0].name}"
}
