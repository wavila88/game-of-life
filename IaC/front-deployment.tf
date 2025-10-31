#Configure fornt deployment and service
resource "kubernetes_deployment" "frontend" {
  metadata {
    name      = "frontend-deployment"
    namespace = "front-app"
    labels = {
      app = "frontend"
    }
  }

  spec {
    replicas = 2

    selector {
      match_labels = {
        app = "frontend"
      }
    }

    template {
      metadata {
        labels = {
          app = "frontend"
        }
      }

      spec {
        container {
          name  = "frontend"
          image = "front-game-of-life:v3.0"
          image_pull_policy = "Never"

          port {
            container_port = 80
          }
          resources {
            requests = {
                cpu    = "50m"
                memory = "64Mi"
            }
            limits = {
                cpu    = "100m"
                memory = "128Mi"
            }
          }

        }
      }
    }
  }
}
#configure service
resource "kubernetes_service" "frontend" {
  metadata {
    name      = "frontend-service"
    namespace = "front-app"
  }

  spec {
    selector = {
      app = "frontend"
    }

    port {
      port        = 80
      target_port = 80
    }


    type = "NodePort"
  }
}
