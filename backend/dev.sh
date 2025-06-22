#!/bin/bash

set -e

# Configuration
CONTAINER_NAME="psychologist-sqlserver"
SA_PASSWORD="YourStrongPassword1@"
DB_NAME="PsychologistDb"
IMAGE="mcr.microsoft.com/mssql/server:2022-latest"
PORT=1433

# Ensure the correct number of args for migration
MIGRATION_NAME=$2

# Utility: Wait for SQL Server inside container
wait_for_sqlserver() {
  echo "Waiting for SQL Server to be ready..."
  sleep 10
  until docker exec $CONTAINER_NAME /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P "$SA_PASSWORD" -Q "SELECT 1" -C -N &> /dev/null
  do
    echo -n "."
    sleep 2
  done
  echo ""
}

# Command: Start DB + migrate + seed
setup() {
  if docker ps -a --format '{{.Names}}' | grep -Eq "^${CONTAINER_NAME}\$"; then
    echo "Container $CONTAINER_NAME already exists. Removing it..."
    docker rm -f $CONTAINER_NAME
  fi


  echo "Starting SQL Server container..."
  docker run -d \
    --name $CONTAINER_NAME \
    -e "ACCEPT_EULA=Y" \
    -e "MSSQL_SA_PASSWORD=$SA_PASSWORD" \
    -p $PORT:1433 \
    $IMAGE

  wait_for_sqlserver

  echo "Creating database $DB_NAME..."
  DOTNET_ENVIRONMENT=Development docker exec $CONTAINER_NAME /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U SA -P "$SA_PASSWORD" \
    -Q "IF DB_ID('$DB_NAME') IS NULL CREATE DATABASE [$DB_NAME]" -C -N

  echo "Running EF Core migrations..."
  DOTNET_ENVIRONMENT=Development dotnet ef database update \
    --project PsychologistBooking.Infrastructure \
    --startup-project PsychologistBooking.Seeder \
    --context AppDbContext

  echo "Seeding data..."
  DOTNET_ENVIRONMENT=Development dotnet run --project PsychologistBooking.Seeder

  echo "Setup complete. SQL Server running at localhost:$PORT"
}

# Command: Add migration
add_migration() {
  if [ -z "$MIGRATION_NAME" ]; then
    echo "Please provide a migration name."
    echo "Usage: ./dev.sh add-migration <MigrationName>"
    exit 1
  fi

  echo "Adding migration: $MIGRATION_NAME"
  dotnet ef migrations add "$MIGRATION_NAME" \
    --project PsychologistBooking.Infrastructure \
    --startup-project PsychologistBooking.Seeder \
    --context AppDbContext
}

# Command: Apply migration
apply_migration() {
  echo "Applying EF migrations..."
  dotnet ef database update \
    --project PsychologistBooking.Infrastructure \
    --startup-project PsychologistBooking.Seeder \
    --context AppDbContext
}

# Command: Run the azure function locally
run() {
  echo "Running Azure function"
  cd PsychologistBooking.Functions
  func start --port 7001
}

# Command: Test azure function endpoints
integration_test() {
  echo "Testing Azure endpoints"
  dotnet test
}

# Command: Stop
stop() {
  echo "Stopping docker container"
  docker rm -f $CONTAINER_NAME
}

# Main logic
case "$1" in
  setup)
    setup
    ;;
  run)
    run
    ;;
  test)
    integration_test
    ;;
  add-migration)
    add_migration
    ;;
  apply-migration)
    apply_migration
    ;;
  stop)
    stop
    ;;
  *)
    echo "Usage:"
    echo "  ./dev.sh setup                    # Start Docker DB + migrate + seed"
    echo "  ./dev.sh run                      # Run the azure function locally"
    echo "  ./dev.sh test                     # Test the azure function locally"
    echo "  ./dev.sh add-migration <Name>     # Add EF Core migration"
    echo "  ./dev.sh apply-migration          # Apply EF Core migration"
    echo "  ./dev.sh stop                     # Stop service"
    exit 1
    ;;
esac
