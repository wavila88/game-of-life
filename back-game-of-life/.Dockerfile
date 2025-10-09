# ==========================================================
# STAGE 1: BUILD - Copia toda la solución y publica el proyecto ejecutable
# ==========================================================
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copia los archivos de toda la solución al directorio de trabajo
COPY . . 

# Restaura las dependencias de toda la solución
RUN dotnet restore "back-game-of-life.sln"

# Navega a la carpeta del proyecto ejecutable (asumimos 'Api' es el proyecto principal)
# Si tu proyecto principal tiene otro nombre (por ejemplo, 'Infra' o 'Web'), cámbialo aquí.
WORKDIR /src/Api

# Publica la aplicación final a /app/publish
# Usaremos el nombre del directorio del proyecto (Api) para la publicación, si el nombre de salida no se sobreescribe
RUN dotnet publish -c Release -o /app/publish

# ==========================================================
# STAGE 2: FINAL - Imagen de Runtime (más pequeña y segura)
# ==========================================================
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Fuerza la aplicación a escuchar en el Puerto 80 dentro del contenedor
ENV ASPNETCORE_URLS=http://+:80

# Copia el resultado de la publicación de la etapa 'build'
COPY --from=build /app/publish . 

# Define el punto de entrada para ejecutar el archivo DLL principal.
# ATENCIÓN: El nombre de la DLL DEBE coincidir con el nombre de tu proyecto ejecutable. 
# Si tu proyecto en /Api se llama 'Api.csproj', el DLL será 'Api.dll'.
# Si el nombre del proyecto fuera diferente, ajusta "Api.dll" aquí.
ENTRYPOINT ["dotnet", "Api.dll"] 
