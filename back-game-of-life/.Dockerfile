# ==========================================================
# STAGE 1: BUILD - Copies the entire solution and publishes the executable project
# ==========================================================
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copies all solution files to the working directory
COPY . . 

# Restores dependencies for the entire solution
RUN dotnet restore "back-game-of-life.sln"

# Navigate to the executable project folder (assuming 'Api' is the main project)
# If your main project has another name (e.g., 'Infra' or 'Web'), change it here.
WORKDIR /src/Api

# Publish the final application to /app/publish
# We will use the project directory name (Api) for publishing, unless the output name is overwritten
RUN dotnet publish -c Release -o /app/publish

# ==========================================================
# STAGE 2: FINAL - Runtime image (smaller and safer)
# ==========================================================
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Forces the application to listen on Port 80 inside the container
ENV ASPNETCORE_URLS=http://+:80

# Copies the publish result from the 'build' stage
COPY --from=build /app/publish . 

# Defines the entry point to run the main DLL file.
# ATTENTION: The DLL name MUST match your executable project name.
# If your project in /Api is called 'Api.csproj', the DLL will be 'Api.dll'.
# If the project name is different, adjust "Api.dll" here.
ENTRYPOINT ["dotnet", "Api.dll"] 
