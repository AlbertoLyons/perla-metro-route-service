# Makefile for windows

# Setup for dotnet
setup:
	@echo "$(GREEN)Setting up project...$(NC)"
	@echo "$(YELLOW)1. Restoring packages...$(NC)"
	@dotnet restore
	@echo "$(YELLOW)2. Installing tools...$(NC)"
	@dotnet tool install --global dotnet-ef || echo "$(YELLOW)EF Core tools already installed$(NC)"
	@echo "$(YELLOW)3. Building project...$(NC)"
	@dotnet build
	@echo "$(GREEN)Project setup completed!$(NC)"
	@echo "$(YELLOW)Run 'make dev' to start development$(NC)"

# Build the project
build:
	@echo "$(GREEN)Building project...$(NC)"
	@dotnet build
	@echo "$(GREEN)Build completed$(NC)"

# Restore NuGet packages
restore:
	@echo "$(GREEN)Restoring NuGet packages...$(NC)"
	@dotnet restore
	@echo "$(GREEN)Packages restored$(NC)"

# Start Neo4j container
db-up:
	@echo "$(GREEN)Starting Neo4j container...$(NC)"
	@docker run --name db-neo4j -e NEO4J_AUTH=neo4j/12345678 -p 7687:7687 -d neo4j:latest || \
	(echo "$(YELLOW)Container already exists, starting...$(NC)" && docker start db-neo4j)
	@echo "$(YELLOW)Waiting for Neo4j to be ready...$(NC)"
	@$(SLEEP) 5
	@echo "$(GREEN)Neo4j is ready$(NC)"

# Stop Neo4j container
db-down:
	@echo "$(GREEN)Stopping Neo4j container...$(NC)"
	@docker stop db-neo4j 2>/dev/null || echo "$(YELLOW)Container was not running$(NC)"
	@echo "$(GREEN)Neo4j stopped$(NC)"

# Run the application
run:
	@echo "$(GREEN)Starting application with watch (no hot reload)...$(NC)"
	@cd $(HOST_PATH) && dotnet watch run --no-hot-reload